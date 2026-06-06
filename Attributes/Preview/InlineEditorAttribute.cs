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
        private Editor editor = null;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
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
            if (editor == null || editor.target != property.objectReferenceValue)
                Editor.CreateCachedEditor(property.objectReferenceValue, null, ref editor);
            
            if (editor == null)
                return;
            
            // Create box style
            GUIStyle boxStyle = new(GUI.skin.box);
            boxStyle.margin = new(3, 3, 3, 3);
            boxStyle.padding = new(3, 3, 3, 3);
            
            // Draw editor
            EditorGUILayout.BeginVertical(boxStyle);
            EditorGUI.indentLevel++;
            
            editor.OnInspectorGUI();
            
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);;
        }
    }
}
#endif
