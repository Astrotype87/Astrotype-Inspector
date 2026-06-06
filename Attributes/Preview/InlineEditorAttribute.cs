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
                
                // When foldout is expanded or collapsed
                foldout.RegisterValueChangedCallback(e =>
                {
                    Debug.Log($"Foldout isExpanded = {foldout.value}");
                    // Update property isExpanded state
                    property.isExpanded = foldout.value;
                    
                    // HOW DO YOU KNOW IF EDITOR IS NOT CREATED YET? editorContainer is empty
                    // IS THIS A RELIABLE BASIS? yes, because editorContainer is the proof that it contains the nested editor, and it is displayed in UI.
                    // When foldout is opened and editorContainer is empty, create nested editor
                    if (foldout.value && editorContainer.childCount == 0)
                    {
                        Debug.Log($"Foldout is open, and editor container is empty. Attempting to create nested editor.");
                        
                        // Abort if object reference is null
                        if (property.objectReferenceValue == null)
                        {
                            Debug.Log($"The object reference is empty. Aborting creation");
                            return;
                        }
                        
                        // Create cached editor, abort if null
                        Editor.CreateCachedEditor(property.objectReferenceValue, null, ref cachedEditor);
                        if (cachedEditor == null)
                            return;
                        
                        // Create inspector GUI and add to editor container
                        var inspector = cachedEditor.CreateInspectorGUI();
                        if (inspector != null)
                        {
                            inspector.name = GetInlineEditorName(property);
                            inspector.Bind(cachedEditor.serializedObject);
                            editorContainer.Add(inspector);
                            Debug.Log($"Nested editor is created for {GetInlineEditorName(property)}!");
                        }
                        else
                        {
                            Debug.Log($"{GetInlineEditorName(property)} doesn't have CreateInspectorGUI() implementation.");
                            // TODO: Implement IMGUIContainer and call editor.OnInspectorGUI();
                        }
                    }
                });
                
                // Refresh nested editor if object field value is changed
                var objectField = parent.Q<ObjectField>();
                objectField.RegisterValueChangedCallback(e =>
                {
                    Debug.Log($"Object reference value is changed! Attempting to create nested editor.");
                    
                    // Hide foldout if object reference is null
                    foldout.style.display = property.objectReferenceValue == null ? DisplayStyle.None : DisplayStyle.Flex;
                    if (!foldout.value)
                    {
                        Debug.Log($"The foldout is closed. Aborting creation.");
                        return;
                    }
                    
                    // Clear container and destroy cached editor
                    editorContainer.Clear();
                    if (cachedEditor)
                        Object.DestroyImmediate(cachedEditor);
                    cachedEditor = null;
                    
                    // Abort if object reference is null
                    if (property.objectReferenceValue == null)
                    {
                        Debug.Log($"The object reference is empty. Aborting creation.");
                        return;
                    }
                    
                    // Create cached editor, abort if null
                    Editor.CreateCachedEditor(property.objectReferenceValue, null, ref cachedEditor);
                    if (cachedEditor == null)
                        return;
                    
                    // Create inspector GUI and add to editor container
                    var inspector = cachedEditor.CreateInspectorGUI();
                    if (inspector != null)
                    {
                        inspector.name = GetInlineEditorName(property);
                        inspector.Bind(cachedEditor.serializedObject);
                        editorContainer.Add(inspector);
                        Debug.Log($"Nested editor is created for {GetInlineEditorName(property)}!");
                    }
                    else
                    {
                        Debug.Log($"{GetInlineEditorName(property)} doesn't have CreateInspectorGUI() implementation.");
                        // TODO: Implement IMGUIContainer and call editor.OnInspectorGUI();
                    }
                });
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
        
    }
}
#endif
