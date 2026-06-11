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
        private static float lastViewWidth;
        private float cachedPositionWidth;
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var attribute = this.attribute as TextAttribute;
            
            // Create text content, style, and calculate height
            GUIContent textContent = new(attribute.Text);
            GUIStyle textStyle = CreateTextStyle(attribute);
            float textHeight = CalculateTextHeight(textContent, textStyle, PredictPositionWidth());
            
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
            
            // Create text content, style, and calculate height
            GUIContent textContent = new(attribute.Text);
            GUIStyle textStyle = CreateTextStyle(attribute);
            float textHeight = CalculateTextHeight(textContent, textStyle, position.width);
            
            // Cache position.width in Repaint event, used in GetPropertyHeight() to calculate paragraph height
            if (Event.current.type == EventType.Repaint)
                cachedPositionWidth = position.width;
            
            // Draw text label
            Rect textRect = EditorGUI.IndentedRect(position);
            textRect.y += offsetIfBottom;
            textRect.height = textHeight;
            GUI.Label(textRect, textContent, textStyle);
            
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
        
        
        private float PredictPositionWidth()
        {
            // Detect change in view width
            float currentViewWidth = EditorGUIUtility.currentViewWidth;
            bool hasCurrentViewWidthChanged = lastViewWidth != currentViewWidth;
            lastViewWidth = EditorGUIUtility.currentViewWidth;
            
            // Use cached position width if view width doesn't update
            if (!hasCurrentViewWidthChanged)
                return cachedPositionWidth;
            
            // If current view width has changed, predict position.width
            const float IndentWidth = 15f;
            const float LeftPadding = 18f; // IMGUI inspector left padding
            const float RightPadding = 4f; // IMGUI inspector right padding
            
            // NOTE: Can't detect if scroll bar is present in the inspector
            float currentIndentWidth = EditorGUI.indentLevel * IndentWidth; // * InspectorState.IndentDecimalOffset
            return currentViewWidth - currentIndentWidth - LeftPadding - RightPadding;
        }
        
        
        private static GUIStyle CreateTextStyle(TextAttribute attribute)
        {
            return new GUIStyle(EditorStyles.label)
            {
                fontSize = attribute.Size <= 0 ? EditorStyles.label.fontSize : attribute.Size,
                fontStyle = attribute.Style,
                alignment = GetMiddleAnchorByAlign(attribute.Align),
            };
        }
        
        private static float CalculateTextHeight(GUIContent content, GUIStyle style, float width)
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
