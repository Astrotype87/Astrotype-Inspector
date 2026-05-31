using System;
using System.Diagnostics;
using UnityEngine;

namespace AstrotypeInspector
{
    [Conditional(Symbols.UNITY_EDITOR), Conditional(Symbols.INCLUDE_IN_BUILD)]
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public sealed class ParagraphAttribute : DecorativeAttribute
    {
        public readonly string Paragraph = null;
        public readonly int Size = -1;
        public readonly FontStyle Style = FontStyle.Normal;
        public readonly Align Align = Align.Left;
        
        
        public ParagraphAttribute(string paragraph, Align align)
        {
            Paragraph = paragraph;
            Align = align;
        }
        
        public ParagraphAttribute(string paragraph, FontStyle style, Align align = Align.Left)
        {
            Paragraph = paragraph;
            Style = style;
            Align = align;
        }
        
        public ParagraphAttribute(string paragraph, int size = -1, FontStyle style = FontStyle.Normal, Align align = Align.Left)
        {
            Paragraph = paragraph;
            Size = size;
            Style = style;
            Align = align;
        }
        
    }
}

#if UNITY_EDITOR
namespace AstrotypeInspector.Editor
{
    using UnityEngine;
    using UnityEditor;
    using UnityEngine.UIElements;
    using UnityEditor.UIElements;
    
    using Align = Align;
    
    [CustomPropertyDrawer(typeof(ParagraphAttribute))]
    public class ParagraphDrawer : PropertyDrawer
    {
        private float cachedPositionWidth;
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var attribute = this.attribute as ParagraphAttribute;
            
            // Create paragraph content, style, and calculate height
            GUIContent paragraphContent = new(attribute.Paragraph);
            GUIStyle paragraphStyle = CreateParagraphStyle(attribute);
            float paragraphHeight = CalculateParagraphHeight(paragraphContent, paragraphStyle, cachedPositionWidth);
            
            float height = EditorGUI.GetPropertyHeight(property, label, true);
            height += paragraphHeight + EditorGUIUtility.standardVerticalSpacing;
            
            return height;
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attribute = this.attribute as ParagraphAttribute;
            float offsetIfBottom = attribute.bottom
                ? EditorGUI.GetPropertyHeight(property) + EditorGUIUtility.standardVerticalSpacing
                : 0;
            
            // Create paragraph content, style, and calculate height
            GUIContent paragraphContent = new(attribute.Paragraph);
            GUIStyle paragraphStyle = CreateParagraphStyle(attribute);
            float paragraphHeight = CalculateParagraphHeight(paragraphContent, paragraphStyle, position.width);
            
            // Cache position.width in Repaint event, used in GetPropertyHeight() to calculate paragraph height
            if (Event.current.type == EventType.Repaint)
                cachedPositionWidth = position.width;
            
            // Draw paragraph label
            Rect paragraphRect = position;
            paragraphRect.y += offsetIfBottom;
            paragraphRect.height = paragraphHeight;
            GUI.Label(paragraphRect, paragraphContent, paragraphStyle);
            
            // Draw property field
            Rect propertyRect = position;
            if (!attribute.bottom)
                propertyRect.y += paragraphHeight + EditorGUIUtility.standardVerticalSpacing;
            
            EditorGUI.PropertyField(propertyRect, property, label, true);
        }
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var attribute = this.attribute as ParagraphAttribute;
            
            // Create paragraph label
            var paragraphLabel = new Label();
            paragraphLabel.name = "paragraph-label";
            paragraphLabel.AddToClassList("unity-base-field__label");
            
            paragraphLabel.text = attribute.Paragraph;
            paragraphLabel.style.unityFontStyleAndWeight = attribute.Style;
            paragraphLabel.style.unityTextAlign = GetMiddleAnchorByAlign(attribute.Align);
            
            paragraphLabel.style.whiteSpace = WhiteSpace.Normal;
            paragraphLabel.style.flexShrink = 0;
            if (attribute.Size >= 0)
                paragraphLabel.style.fontSize = attribute.Size;
            
            // Margin top and bottom based on property field vertical spacing
            paragraphLabel.style.marginBottom = 1;
            paragraphLabel.style.marginTop = 1;
            paragraphLabel.style.marginRight = 0; // marginRight not needed because no input field to the right
            
            // Create property field
            var propertyField = new PropertyField(property);
            propertyField.schedule.Execute(() =>
            {
                // Unwrap property field wrapper
                propertyField.UnwrapElement(out var parent);
                
                // Add decorative elements inside top/bottom decorator drawers container
                if (attribute.bottom)
                    parent.AddToBottomDecoratorContainer(paragraphLabel);
                else
                    parent.AddToDecoratorContainer(paragraphLabel);
            });
            
            return propertyField;
        }
        
        
        private static GUIStyle CreateParagraphStyle(ParagraphAttribute attribute)
        {
            return new GUIStyle(EditorStyles.label)
            {
                fontSize = attribute.Size <= 0 ? EditorStyles.label.fontSize : attribute.Size,
                fontStyle = attribute.Style,
                alignment = GetMiddleAnchorByAlign(attribute.Align),
                wordWrap = true,
            };
        }
        
        private static float CalculateParagraphHeight(GUIContent content, GUIStyle style, float width)
        {
            const float ExtraLineHeight = 3f; // EditorGUIUtility.singleLineHeight - EditorStyles.label.lineHeight (18f - 15f)
            return Mathf.Round(style.CalcHeight(content, width)) + ExtraLineHeight;
        }
        
        private static TextAnchor GetMiddleAnchorByAlign(Align align)
        {
            return align switch
            {
                Align.Left => TextAnchor.MiddleLeft,
                Align.Center => TextAnchor.MiddleCenter,
                Align.Right => TextAnchor.MiddleRight,
                _ => TextAnchor.MiddleLeft
            };
        }
        
    }
}
#endif
