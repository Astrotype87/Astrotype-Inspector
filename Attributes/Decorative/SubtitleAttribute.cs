using System;
using System.Diagnostics;
using UnityEngine;

namespace AstrotypeInspector
{
    [Conditional(Symbols.UNITY_EDITOR), Conditional(Symbols.INCLUDE_IN_BUILD)]
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public sealed class SubtitleAttribute : DecorativeAttribute
    {
        public readonly string Subtitle = null;
        public readonly Icon Icon = Icon._NONE;
        
        public readonly FontStyle Style = FontStyle.Normal;
        public readonly Align Align = Align.Left;
        
        
        public SubtitleAttribute(string subtitle,
            FontStyle style = FontStyle.Normal, Align align = Align.Left) : base(true)
        {
            Subtitle = subtitle;
            
            Style = style;
            Align = align;
        }
        
        public SubtitleAttribute(string subtitle, Icon icon,
            FontStyle style = FontStyle.Normal, Align align = Align.Left) : base(true)
        {
            Subtitle = subtitle;
            Icon = icon;
            
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
    
    using Align = AstrotypeInspector.Align;
    
    [CustomPropertyDrawer(typeof(SubtitleAttribute))]
    public class SubtitleDrawer : PropertyDrawer
    {
        // IMGUI height settings
        private float subtitleHeight = EditorGUIUtility.singleLineHeight - 3f - 2f;
        private float subtitleMarginBottom = 1f + 1f;
        
        private float iconMarginTop = 0f;
        private float iconSize = EditorGUIUtility.singleLineHeight - 3f - 2f;
        private float iconMarginRight = 1f;
        
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return subtitleHeight + subtitleMarginBottom
                + EditorGUI.GetPropertyHeight(property, label, true);
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attribute = this.attribute as SubtitleAttribute;
            Texture iconTexture = IconDictionary.GetIconTexture(attribute.Icon);
            float offsetIfBottom = attribute.bottom
                ? EditorGUI.GetPropertyHeight(property) + EditorGUIUtility.standardVerticalSpacing
                : 0;
            
            // Create subtitle style
            GUIStyle subtitleStyle = new(EditorStyles.miniLabel);
            subtitleStyle.fontStyle = attribute.Style;
            subtitleStyle.alignment = GetMiddleAnchorByAlign(attribute.Align);  // EditorStyles.boldLabel style uses Middle for text anchor
            subtitleStyle.normal.textColor = SetAlpha(subtitleStyle.normal.textColor, 0.6f);
            subtitleStyle.hover.textColor = SetAlpha(subtitleStyle.hover.textColor, 0.6f);
            
            // Calculate side offset, used to reduce remaining margin based on alignment
            float sideOffset = attribute.Align == Align.Left ? -0.5f :
                attribute.Align == Align.Right ? 0.5f : 0f;
            subtitleStyle.contentOffset = new(sideOffset, subtitleStyle.contentOffset.y);
            
            // Draw subtitle label
            Rect subtitleRect = EditorGUI.IndentedRect(position);
            subtitleRect.y += offsetIfBottom;
            subtitleRect.height = subtitleHeight;
            if (iconTexture != null) // Leave space for icon
            {
                subtitleRect.x += iconSize + iconMarginRight;
                subtitleRect.width += -iconSize + iconMarginRight;
            }
            GUI.Label(subtitleRect, new GUIContent(attribute.Subtitle), subtitleStyle);
            
            // Draw icon image
            if (iconTexture != null)
            {
                // Create icon style
                GUIStyle iconStyle = new(EditorStyles.miniLabel);
                iconStyle.alignment = GetMiddleAnchorByAlign(attribute.Align); // EditorStyles.miniLabel style uses Middle for text anchor
                iconStyle.fixedHeight = iconSize; // reduce height to make icon smaller
                iconStyle.contentOffset = new(sideOffset, iconMarginTop); // recenter after reducing height
            
                // Draw icon image
                Rect iconRect = EditorGUI.IndentedRect(position);
                iconRect.y += offsetIfBottom;
                iconRect.height = subtitleHeight;
                GUI.Label(iconRect, new GUIContent(iconTexture), iconStyle);
            }
            
            // Draw property field
            Rect propertyRect = position;
            if (!attribute.bottom)
                propertyRect.y += subtitleHeight + subtitleMarginBottom;
            
            EditorGUI.PropertyField(propertyRect, property, label, true);
        }
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var attribute = this.attribute as SubtitleAttribute;
            Texture iconTexture = IconDictionary.GetIconTexture(attribute.Icon);
            
            // UI Toolkit height settings
            float subtitleMarginTop = 2f; // mini version based on standard header top margin
            float subtitleHeight = EditorGUIUtility.singleLineHeight - 3f;
            float subtitleMarginBottom = 1f;
            float iconSize = subtitleHeight - 2f; // based on EditorGUIUtility.singleLineHeight - 3f - 2f
            float iconMarginLeft = 1f; // based on IMGUI where icon has little margin to the left
            
            // Color settings
            Color subtitleColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
            subtitleColor.a *= 0.4f;
            
            
            // Create decorator wrapper
            var subtitleWrapper = AstrotypeEditorGUI.CreateDecoratorWrapper("subtitle-wrapper");
            
            // Create subtitle label
            var subtitleLabel = new Label(attribute.Subtitle);
            subtitleLabel.style.fontSize = EditorStyles.miniLabel.fontSize;
            subtitleLabel.style.unityFontStyleAndWeight = attribute.Style;
            subtitleLabel.style.unityTextAlign = GetLowerAnchorByAlign(attribute.Align);
            subtitleLabel.style.color = subtitleColor;
            
            subtitleLabel.style.marginTop = subtitleMarginTop;
            subtitleLabel.style.marginBottom = subtitleMarginBottom;
            if (iconTexture != null)
                subtitleLabel.style.marginLeft = iconSize + iconMarginLeft;
            
            subtitleWrapper.Add(subtitleLabel);
            
            // Create icon image
            if (iconTexture != null)
            {
                var iconImage = new Image();
                iconImage.image = iconTexture;
                iconImage.style.width = iconSize;
                iconImage.style.height = iconSize;
                iconImage.style.flexGrow = 0;
                iconImage.style.flexShrink = 0;
                
                iconImage.style.position = Position.Absolute;
                iconImage.style.marginTop = subtitleMarginTop;
                iconImage.style.marginLeft = iconMarginLeft;
                
                subtitleWrapper.Add(iconImage);
            }
            
            // Create property field
            var propertyField = new PropertyField(property);
            propertyField.schedule.Execute(() =>
            {
                // Unwrap property field wrapper
                propertyField.UnwrapElement(out var parent);
                
                // Add decorative elements inside top/bottom decorator drawers container
                if (attribute.bottom)
                    parent.AddToBottomDecoratorContainer(subtitleWrapper);
                else
                    parent.AddToDecoratorContainer(subtitleWrapper);
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
        
        private static TextAnchor GetLowerAnchorByAlign(Align align)
        {
            return align switch
            {
                Align.Left => TextAnchor.LowerLeft,
                Align.Center => TextAnchor.LowerCenter,
                Align.Right => TextAnchor.LowerRight,
                _ => TextAnchor.LowerLeft
            };
        }
        
        private static Color SetAlpha(Color color, float alpha)
        {
            return new(color.r, color.g, color.b, color.a * alpha);
        }
        
    }
}
#endif
