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
    using System.Collections.Generic;
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
        private static readonly HashSet<(Object, Object)> targetNestedPairs = new(); 
        
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
                
                // Move foldout toggle to the left of object field
                var toggle = foldout.Q<Toggle>(className: Foldout.toggleUssClassName);
                toggle.style.position = Position.Absolute;
                toggle.style.translate = new Vector3(0, -18, 0);
                
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
                    if (editorContainer.childCount == 0)
                        editorContainer.Add(CreateNestedInspector());
                });
                
                // Refresh nested editor if object field value is changed
                var objectField = parent.Q<ObjectField>();
                objectField.RegisterValueChangedCallback(e =>
                {
                    // Clear container and destroy cached editor
                    editorContainer.Clear();
                    if (cachedEditor)
                        Object.DestroyImmediate(cachedEditor);
                    cachedEditor = null;
                    
                    // Try create nested inspector
                    editorContainer.Add(CreateNestedInspector());
                });
                
                VisualElement CreateNestedInspector()
                {
                    // Create target-nested pair for tracking recursion
                    Object target = property.serializedObject.targetObject;
                    Object nested = property.objectReferenceValue;
                    (Object, Object) targetNestedPair = (target, nested);
                    
                    // Reset foldout toggle visibility
                    foldout.style.display = DisplayStyle.Flex;
                    
                    if (property.objectReferenceValue != null) Debug.Log($"New target-nested pair: {PrintTargetNestedPair(target, nested)}");
                    
                    // If object reference is null or recursion is possible
                    if (property.objectReferenceValue == null || targetNestedPairs.Contains(targetNestedPair))
                    {
                        if (targetNestedPairs.Contains(targetNestedPair))
                            Debug.Log($"Recursion detected {PrintTargetNestedPair(target, nested)}! Avoiding nested editor creation.");
                        
                        // Hide foldout toggle and do not create nested inspector
                        foldout.style.display = DisplayStyle.None;
                        return null;
                    }
                    
                    // If foldout is closed
                    if (!foldout.value)
                        return null;
                    
                    // Create cached editor, abort if null
                    Editor.CreateCachedEditor(property.objectReferenceValue, null, ref cachedEditor);
                    if (cachedEditor == null)
                        return null;
                    
                    // Create UIToolkit inspector GUI
                    var inspector = cachedEditor.CreateInspectorGUI();
                    inspector?.Bind(cachedEditor.serializedObject);
                    
                    // Create IMGUI container if no UIToolkit implementation
                    if (inspector == null)
                    {
                        inspector = CreateInspectorIMGUIContainer(cachedEditor, property.depth);
                        if (inspector != null)
                        {
                            inspector.style.marginTop = 1;
                            inspector.style.marginBottom = 1;
                            inspector.style.marginLeft = 3;
                            inspector.style.marginRight = -2;
                        }
                    }
                    
                    // Set inspector name and add target-nested pair to hash set
                    if (inspector != null)
                    {
                        inspector.name = GetInlineEditorName(property);
                        targetNestedPairs.Add(targetNestedPair);
                        Debug.Log($"Added target-nested pair: {PrintTargetNestedPair(target, nested)} from hash set!");
                        
                        // Remove target-nested pair from hash set when inspector is destroyed
                        inspector.RegisterCallback<DetachFromPanelEvent>(_ =>
                        {
                            targetNestedPairs.Remove(targetNestedPair);
                            Debug.Log($"Removed target-nested pair: {PrintTargetNestedPair(target, nested)} from hash set!");
                        });
                    }
                    
                    return inspector;
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
        
        private static IMGUIContainer CreateInspectorIMGUIContainer(Editor cachedEditor, int propertyDepth)
        {
            return new IMGUIContainer(() =>
            {
                // Enables auto-adjusting label width and better foldouts.
                bool hierarchyMode = EditorGUIUtility.hierarchyMode;
                EditorGUIUtility.hierarchyMode = true;
                
                // Wrap input fields for Vector2, Vector3, etc. when current inspector window width reaches below threshold.
                bool wideMode = EditorGUIUtility.wideMode; // threshold = 330
                EditorGUIUtility.wideMode = EditorGUIUtility.currentViewWidth > 330;
                
                // Reduce label width by indent
                float labelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = labelWidth - (15f * (propertyDepth + 1));
                
                // Draw inspector
                cachedEditor.OnInspectorGUI();
                
                // Restore modes
                EditorGUIUtility.hierarchyMode = hierarchyMode;
                EditorGUIUtility.wideMode = wideMode;
                EditorGUIUtility.labelWidth = labelWidth;
            });
        }
        
        private static string PrintTargetNestedPair(Object target, Object nested)
        {
            return $"{(target ? target.GetType().Name : "")}:{(target ? target.GetInstanceID() : "")} -> {(nested ? nested.GetType().Name : "")}:{(nested ? nested.GetInstanceID() : "")}";
        }
        
    }
}
#endif
