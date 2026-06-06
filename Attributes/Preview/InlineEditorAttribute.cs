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
        public readonly bool UseMargin;
        public InlineEditorAttribute() { }
        public InlineEditorAttribute(bool useMargin) => UseMargin = true;
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
            
            // Draw editor
            var attribute = (InlineEditorAttribute)this.attribute; 
            
            if (attribute.UseMargin)
            {
                EditorGUILayout.BeginVertical(boxStyle);
                EditorGUILayout.Space(-2, false); // top margin
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space(-3, false); // left margin
                EditorGUILayout.BeginVertical();
            }
            
            EditorGUI.indentLevel++;
            editor.OnInspectorGUI();
            EditorGUI.indentLevel--;
            
            if (attribute.UseMargin)
            {
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(-3, false); // right margin
                EditorGUILayout.EndHorizontal();
                // EditorGUILayout.Space(2, false); // bottom margin
                EditorGUILayout.EndVertical();
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float totalHeight = EditorGUI.GetPropertyHeight(property, label, true);
            
            if (property.isExpanded && property.objectReferenceValue != null)
            {
                if (editor == null || editor.target != property.objectReferenceValue)
                    Editor.CreateCachedEditor(property.objectReferenceValue, null, ref editor);
                
                if (editor != null)
                {
                    // totalHeight += EditorGUIUtility.standardVerticalSpacing + GetEditorHeight(editor);
                }
            }
            
            return totalHeight;
        }
        
        
        private float GetEditorHeight(Editor editor)
        {
            float height = 0f;
            SerializedObject serializedEditor = editor.serializedObject;
            
            SerializedProperty iterator = serializedEditor.GetIterator();
            iterator.NextVisible(true); // Skip "m_Script"
            
            while (iterator.NextVisible(false))
            {
                height += EditorGUI.GetPropertyHeight(iterator, true) + EditorGUIUtility.standardVerticalSpacing;
            }
            
            return height + EditorGUIUtility.standardVerticalSpacing; // Padding for safety
        }
        
        private void BeginMargin(float marginTop, float marginLeft)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space(marginTop);
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(marginLeft);
            
            EditorGUILayout.BeginVertical();
        }
        
        private void EndMargin(float marginRight, float marginBottom)
        {
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(marginRight);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(marginBottom);
            EditorGUILayout.EndVertical();
        }
        
        private void DrawWithMargin(Action draw, float marginTop, float marginLeft, float marginRight, float marginBottom)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space(marginTop);
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(marginLeft);
            
            EditorGUILayout.BeginVertical();
            draw();
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(marginRight);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(marginBottom);
            EditorGUILayout.EndVertical();
        }
        
    }
}
#endif
