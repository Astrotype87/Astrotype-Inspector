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
    using Unity.Properties;
    using Editor = UnityEditor.Editor;
    
    [CustomPropertyDrawer(typeof(InlineEditorAttribute))]
    public class InlineEditorDrawer: PropertyDrawer
    {
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
            EditorGUILayout.BeginVertical(boxStyle);
            EditorGUI.indentLevel++;
            
            cachedEditor.OnInspectorGUI();
            
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
                foldout.name = "inline-editor-foldout";
                foldout.value = property.isExpanded;
                foldout.style.display = property.objectReferenceValue == null ? DisplayStyle.None : DisplayStyle.Flex;
                parent.Add(foldout);
                
                // Create editor container
                var editorContainer = new VisualElement();
                editorContainer.name =  "inline-editor-container";
                foldout.Add(editorContainer);
                
                // Local variable for cached editor
                Editor cachedEditor = null;
                
                
                // When foldout is expanded or collapsed, create nested inspector if editor container is empty
                foldout.RegisterValueChangedCallback(e =>
                {
                    property.isExpanded = foldout.value;
                    if (foldout.value && editorContainer.childCount == 0)
                        editorContainer.Add(CreateNestedInspector());
                });
                
                // Refresh nested editor if object field value is changed
                var objectField = parent.Q<ObjectField>();
                objectField.RegisterValueChangedCallback(e =>
                {
                    // Hide foldout if object reference is null
                    foldout.style.display = property.objectReferenceValue == null ? DisplayStyle.None : DisplayStyle.Flex;
                    if (!foldout.value)
                        return;
                    
                    // Clear container and destroy cached editor
                    editorContainer.Clear();
                    if (cachedEditor)
                        Object.DestroyImmediate(cachedEditor);
                    cachedEditor = null;
                    
                    // Create nested inspector and add to container
                    editorContainer.Add(CreateNestedInspector());
                });
                
                VisualElement CreateNestedInspector()
                {
                    // Abort if object reference is null
                    if (property.objectReferenceValue == null)
                        return null;
                    
                    // Create cached editor, abort if null
                    Editor.CreateCachedEditor(property.objectReferenceValue, null, ref cachedEditor);
                    if (cachedEditor == null)
                        return null;
                    
                    // Create inspector GUI
                    var inspector = cachedEditor.CreateInspectorGUI();
                    if (inspector != null)
                    {
                        inspector.Bind(cachedEditor.serializedObject);
                        inspector.name = GetInlineEditorName(property);
                        foldout.contentContainer.style.marginLeft = 15f;
                        return inspector;
                    }
                    
                    // Create IMGUI container if no UIToolkit implementation
                    inspector = CreateInspectorIMGUIContainer(cachedEditor);
                    if (inspector != null)
                    {
                        inspector.name = GetInlineEditorName(property);
                        foldout.contentContainer.style.marginLeft = 0f;
                        return inspector;
                    }
                    
                    return null;
                }
            });
            
            return propertyField;
        }
        
        
        private string GetInlineEditorName(SerializedProperty property)
        {
            string propertyName = property.displayName;
            string objectName = property.objectReferenceValue.name;
            string typeDisplayName = TypeUtility.GetTypeDisplayName(property.objectReferenceValue.GetType());
            
            return $"{propertyName}:{objectName} ({typeDisplayName})";
        }
        
        private static IMGUIContainer CreateInspectorIMGUIContainer(Editor cachedEditor)
        {
            return new IMGUIContainer(() =>
            {
                // Enables auto-adjusting label width and better foldouts.
                bool hierarchyMode = EditorGUIUtility.hierarchyMode;
                EditorGUIUtility.hierarchyMode = true;
                
                // Wrap input fields for Vector2, Vector3, etc. when current inspector window width reaches below threshold.
                bool wideMode = EditorGUIUtility.wideMode; // threshold = 330
                EditorGUIUtility.wideMode = EditorGUIUtility.currentViewWidth > 330;
                
                // Change label width
                float labelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = labelWidth - 15f;;
                
                // Start padding
                EditorGUILayout.BeginVertical();
                GUILayout.Space(0); // top padding = 4
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(18); // left padding = 18
                EditorGUILayout.BeginVertical();
                
                // Draw inspector
                cachedEditor.OnInspectorGUI();
                
                // End padding
                EditorGUILayout.EndVertical();
                GUILayout.Space(0); // right padding = 4
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(0); // bottom padding = 2
                EditorGUILayout.EndVertical();
                
                // Restore modes
                EditorGUIUtility.hierarchyMode = hierarchyMode;
                EditorGUIUtility.wideMode = wideMode;
                EditorGUIUtility.labelWidth = labelWidth;
            });
        }
        
        
    }
}
#endif
