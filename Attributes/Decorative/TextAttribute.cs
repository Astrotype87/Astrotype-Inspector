using System;
using System.Diagnostics;
using UnityEngine;

namespace AstrotypeInspector
{
    [Conditional(Symbols.UNITY_EDITOR), Conditional(Symbols.INCLUDE_IN_BUILD)]
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public sealed class TextAttribute : DecorativeAttribute
    {
        public readonly string Text = null;
        public readonly int Size = -1;
        public readonly FontStyle Style = FontStyle.Normal;
        public readonly Align Align = Align.Left;
        
        
        public TextAttribute(string text, Align align)
        {
            Text = text;
            Align = align;
        }
        
        public TextAttribute(string text, FontStyle style, Align align = Align.Left)
        {
            Text = text;
            Style = style;
            Align = align;
        }
        
        public TextAttribute(string text, int size = -1, FontStyle style = FontStyle.Normal, Align align = Align.Left)
        {
            Text = text;
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
    
    [CustomPropertyDrawer(typeof(TextAttribute))]
    public class TextDrawer : PropertyDrawer
    {
        private int DefaultFontSize => EditorStyles.label.fontSize;
        private float SingleLineHeight => EditorGUIUtility.singleLineHeight;
        
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var attribute = this.attribute as TextAttribute;
            int textFontSize = attribute.Size <= 0 ? DefaultFontSize : attribute.Size;
            float textHeight = SingleLineHeight / DefaultFontSize * textFontSize;
            
            float height = EditorGUI.GetPropertyHeight(property, label, true);
            height += textHeight + EditorGUIUtility.standardVerticalSpacing;
            
            return height;
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attribute = this.attribute as TextAttribute;
            float offsetIfBottom = attribute.bottom
                ? EditorGUI.GetPropertyHeight(property) + EditorGUIUtility.standardVerticalSpacing
                : 0;
            
            int textFontSize = attribute.Size <= 0 ? DefaultFontSize : attribute.Size;
            float textHeight = SingleLineHeight / DefaultFontSize * textFontSize;
            
            // Create text style
            GUIStyle textStyle = new(EditorStyles.label);
            textStyle.fontSize = textFontSize;
            textStyle.fontStyle = attribute.Style;
            textStyle.alignment = GetMiddleAnchorByAlign(attribute.Align);
            
            // Draw text label
            Rect textRect = position;
            textRect.y += offsetIfBottom;
            textRect.height = textHeight;
            GUI.Label(textRect, attribute.Text, textStyle);
            
            // Draw property field
            Rect propertyRect = position;
            if (!attribute.bottom)
                propertyRect.y += textHeight + EditorGUIUtility.standardVerticalSpacing;
            
            EditorGUI.PropertyField(propertyRect, property, label, true);
        }
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var attribute = this.attribute as TextAttribute;
            
            // Create text label
            var textLabel = new Label();
            textLabel.name = "text-label";
            textLabel.AddToClassList("unity-base-field__label");
            
            textLabel.text = attribute.Text;
            textLabel.style.unityFontStyleAndWeight = attribute.Style;
            textLabel.style.unityTextAlign = GetMiddleAnchorByAlign(attribute.Align);
            if (attribute.Size >= 0)
                textLabel.style.fontSize = attribute.Size;
            
            // Margin top and bottom based on property field vertical spacing
            textLabel.style.marginBottom = 1;
            textLabel.style.marginTop = 1;
            textLabel.style.marginRight = 0; // marginRight not needed because no input field to the right
            
            // Create property field
            var propertyField = new PropertyField(property);
            propertyField.schedule.Execute(() =>
            {
                // Unwrap property field wrapper
                propertyField.UnwrapElement(out var parent);
                
                // Add decorative elements inside top/bottom decorator drawers container
                if (attribute.bottom)
                    parent.AddToBottomDecoratorContainer(textLabel);
                else
                    parent.AddToDecoratorContainer(textLabel);
            });
            
            return propertyField;
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
