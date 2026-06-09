using System;
using UnityEngine;

namespace AstrotypeInspector
{
    /// <summary>
    /// Draws the inspector editor of a referenced UnityEngine.Object inside a foldout.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class InlineEditorAttribute : PropertyAttribute
    {
        public InlineEditorAttribute() { }
    }
}

#if UNITY_EDITOR
namespace AstrotypeInspector.Editor
{
    using UnityEngine;
    using UnityEditor;
    using UnityEngine.UIElements;
    using UnityEditor.UIElements;
    using Editor = UnityEditor.Editor;
    
    [CustomPropertyDrawer(typeof(InlineEditorAttribute))]
    public class InlineEditorDrawer: PropertyDrawer
    {
        public const string InlineEditorFoldoutName = "inline-editor-foldout";
        private const string InvalidTypeMessage = "Use InlineEditor with object reference types.";
        
        private Editor cachedEditor;
        
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference)
                return EditorGUIUtility.singleLineHeight;
            
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Display error message if property is not object reference
            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                EditorGUI.LabelField(position, label.text, InvalidTypeMessage);
                return;
            }
            
            // Draw the property field (object reference)
            position.height = EditorGUI.GetPropertyHeight(property, label, true);
            EditorGUI.PropertyField(position, property, label, true);
            
            if (property.objectReferenceValue == null)
                return;
            
            // Draw foldout toggle
            Rect foldoutRect = position;
            foldoutRect.height = EditorGUIUtility.singleLineHeight;
            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, GUIContent.none);
            
            if (!property.isExpanded)
                return;
            
            // Create cached editor
            if (cachedEditor == null || cachedEditor.target != property.objectReferenceValue)
                Editor.CreateCachedEditor(property.objectReferenceValue, null, ref cachedEditor);
            
            if (cachedEditor == null)
                return;
            
            // Create box style
            GUIStyle boxStyle = new(GUI.skin.box);
            boxStyle.margin = new(3, 3, 3, 3);
            boxStyle.padding = new(3, 3, 3, 3);
            
            // Draw editor
            float labelWidth = EditorGUIUtility.labelWidth;
            
            EditorGUILayout.BeginVertical(boxStyle);
            EditorGUI.indentLevel++;
            EditorGUIUtility.labelWidth -= boxStyle.margin.left;
            
            cachedEditor.OnInspectorGUI();
            
            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                var label = new Label(InvalidTypeMessage);
                label.AddToClassList("unity-base-field__label");
                label.style.marginLeft = 3;
                label.style.marginBottom = 1;
                label.style.marginTop = 1;
                label.style.marginRight = 0;
                return label;
            }
            
            // Create property field
            var propertyField = new PropertyField(property);
            propertyField.schedule.Execute(() =>
            {
                // Unwrap property field wrapper
                propertyField.UnwrapElement(out var parent);
                
                // Create foldout
                var foldout = new Foldout();
                foldout.name = InlineEditorFoldoutName;
                foldout.value = property.isExpanded;
                foldout.style.display = property.objectReferenceValue == null ? DisplayStyle.None : DisplayStyle.Flex;
                parent.Add(foldout);
                
                // Move foldout toggle to the left of object field
                var toggle = foldout.Q<Toggle>(className: Foldout.toggleUssClassName);
                toggle.style.position = Position.Absolute;
                toggle.style.translate = new Vector3(0, -18, 0);
                
                // Add dark background and margin
                var container = foldout.contentContainer;
                container.style.backgroundColor = new Color(0, 0, 0, 0.08f);
                container.style.paddingBottom = 2;
                
                container.style.marginTop = 2;
                container.style.marginLeft = 3;
                container.style.marginRight = -2;
                container.style.marginBottom = 2;
                
                
                // When foldout is expanded or collapsed
                foldout.RegisterValueChangedCallback(e =>
                {
                    property.isExpanded = foldout.value;
                    if (foldout.childCount == 0)
                        foldout.Add(CreateNestedInspector());
                });
                
                // When object field value is changed
                var objectField = parent.Q<ObjectField>();
                objectField.RegisterValueChangedCallback(e =>
                {
                    foldout.Clear();
                    foldout.Add(CreateNestedInspector());
                });
                
                VisualElement CreateNestedInspector()
                {
                    Object objectReference = property.objectReferenceValue;
                    bool isRecursive = IsRecursiveInlineEditor(foldout, property.objectReferenceValue);
                    
                    // Hide foldout if object field is empty, or editor is recursive
                    foldout.style.display = objectReference == null || isRecursive
                        ? DisplayStyle.None : DisplayStyle.Flex;
                    
                    // Cancel creation if foldout is closed, object field is empty, or editor is recursive
                    if (!foldout.value || objectReference == null || isRecursive)
                        return null;
                    
                    
                    // Create cached editor, abort if null
                    Editor.CreateCachedEditor(property.objectReferenceValue, null, ref cachedEditor);
                    if (cachedEditor == null)
                        return null;
                    
                    // Create new inspector element
                    var inspector = new InspectorElement(cachedEditor); // WE NOW USE CACHED EDITOR INSTEAD OF OBJECT REFERENCE VALUE
                    if (inspector == null)
                        return inspector;
                    inspector.style.marginRight = -1;
                    
                    
                    // Detect if it uses IMGUI Container
                    var IMGUIContainer = inspector.Q<IMGUIContainer>();
                    if (IMGUIContainer == null)
                        return inspector;
                    
                    // Replace IMGUIContainer with your own implementation
                    inspector.Remove(IMGUIContainer);
                    inspector.schedule.Execute(_ =>
                    {
                        int foldoutDepth = CountFoldoutDepth(inspector);
                        int nestedEditorDepth = CountNestedEditorDepth(foldout);
                        float marginLeft = container.style.marginLeft.value.value;
                        IMGUIContainer = CreateInspectorIMGUIContainer(cachedEditor, foldoutDepth, nestedEditorDepth, marginLeft);
                        
                        inspector.Add(IMGUIContainer);
                    });
                    
                    return inspector;
                }
            });
            
            return propertyField;
        }
        
        
        private static bool IsRecursiveInlineEditor(Foldout inlineEditorFoldout, Object objectReference)
        {
            // Traverse parent by parent
            for (var current = inlineEditorFoldout.parent; current != null; current = current.parent)
            {
                // Stop search when you reach the root component
                if (current.ClassListContains("unity-inspector-editors-list"))
                    break;
                
                // Check for possible recursion
                if (current is Foldout && current.name == InlineEditorFoldoutName)
                {
                    // Assume that the Foldout has ObjectField sibling
                    var objectField = current.parent?.Q<ObjectField>();
                    
                    // Potentially recursive if the same object reference is found at the top of hierarchy
                    if (objectField?.value == objectReference)
                        return true;
                }
            }
            return false;
        }
        
        private static IMGUIContainer CreateInspectorIMGUIContainer(Editor cachedEditor, int foldoutDepth, int editorDepth, float marginLeft)
        {
            return new IMGUIContainer(() =>
            {
                // Enables auto-adjusting label width and better foldouts.
                bool hierarchyMode = EditorGUIUtility.hierarchyMode;
                EditorGUIUtility.hierarchyMode = true;
                
                // Wrap input fields for Vector2, Vector3, etc. when current inspector window width reaches below threshold.
                bool wideMode = EditorGUIUtility.wideMode; // threshold = 330
                EditorGUIUtility.wideMode = EditorGUIUtility.currentViewWidth > 330;
                
                // Reduce label width by foldout depth, nested editor depth, and nested editor margin left
                float labelWidth = EditorGUIUtility.labelWidth;
                float foldoutIndentBuildup = 15f * foldoutDepth;
                float nestedEditorMarginLeftBuildup = marginLeft * (1 + editorDepth);
                EditorGUIUtility.labelWidth = labelWidth - foldoutIndentBuildup - nestedEditorMarginLeftBuildup;
                
                // Draw inspector with default margins
                EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins);
                cachedEditor.OnInspectorGUI();
                EditorGUILayout.EndVertical();
                
                // Restore modes
                EditorGUIUtility.hierarchyMode = hierarchyMode;
                EditorGUIUtility.wideMode = wideMode;
                EditorGUIUtility.labelWidth = labelWidth;
            });
        }
        
        private static int CountFoldoutDepth(VisualElement fromElement)
        {
            int foldoutDepth = 0;
            for (var current = fromElement; current != null; current = current.parent)
            {
                // Stop search when you reach the root component
                if (current.ClassListContains("unity-inspector-editors-list"))
                    break;
                
                // Check for foldouts
                if (current is Foldout)
                    foldoutDepth ++;
            }
            return foldoutDepth;
        }
        
        private static int CountNestedEditorDepth(Foldout inlineEditorFoldout)
        {
            // Traverse parent by parent
            int nestedEditorDepth = 0;
            for (var current = inlineEditorFoldout.parent; current != null; current = current.parent)
            {
                // Stop search when you reach the root component
                if (current.ClassListContains("unity-inspector-editors-list"))
                    break;
                
                // Check for inline editor foldouts
                if (current is Foldout && current.name == InlineEditorFoldoutName)
                    nestedEditorDepth ++;
            }
            return nestedEditorDepth;
        }
        
    }
}
#endif
