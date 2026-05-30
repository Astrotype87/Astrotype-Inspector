using System;
using System.Diagnostics;
using UnityEngine;

namespace AstrotypeInspector
{
    [Conditional(Symbols.UNITY_EDITOR), Conditional(Symbols.INCLUDE_IN_BUILD)]
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public sealed class TitleAttribute : DecorativeAttribute
    {
        public readonly string Title = null;
        public readonly string Subtitle = null;
        public readonly Icon Icon = Icon._NONE;
        
        public readonly TitleStyle Style = TitleStyle.Default;
        public readonly Align Align = Align.Left;
        public readonly bool Separator = false;
        
        
        public TitleAttribute(string title,
            TitleStyle style = TitleStyle.Default, Align align = Align.Left, bool separator = false) : base(true)
        {
            Title = title;
            
            Style = style;
            Align = align;
            Separator = separator;
        }
        
        public TitleAttribute(string title, Icon icon,
            TitleStyle style = TitleStyle.Default, Align align = Align.Left, bool separator = false) : base(true)
        {
            Title = title;
            Icon = icon;
            
            Style = style;
            Align = align;
            Separator = separator;
        }
        
        public TitleAttribute(string title, string subtitle,
            TitleStyle style = TitleStyle.Default, Align align = Align.Left, bool separator = false) : base(true)
        {
            Title = title;
            Subtitle = subtitle;
            
            Style = style;
            Align = align;
            Separator = separator;
        }
        
        public TitleAttribute(string title, string subtitle, Icon icon,
            TitleStyle style = TitleStyle.Default, Align align = Align.Left, bool separator = false) : base(true)
        {
            Title = title;
            Subtitle = subtitle;
            Icon = icon;
            
            Style = style;
            Align = align;
            Separator = separator;
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
    
    [CustomPropertyDrawer(typeof(TitleAttribute))]
    public class TitleDrawer : PropertyDrawer
    {
        // IMGUI height settings
        private float titleMarginTop = EditorGUIUtility.singleLineHeight / 2;
        private float titleHeight = EditorGUIUtility.singleLineHeight;
        
        private float iconMarginTop = 1f;
        private float iconSize = EditorGUIUtility.singleLineHeight - 2f;
        private float iconMarginRight = 1f;
        
        private float subtitleHeight = EditorGUIUtility.singleLineHeight - 3f - 2f;
        private float subtitleMarginBottom = 1f + 1f;
        
        private float separatorMarginTop = 3f - EditorGUIUtility.standardVerticalSpacing;
        private float separatorHeight = 1.5f;
        private float separatorMarginBottom = 3.5f;
        
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            TitleAttribute attribute = this.attribute as TitleAttribute;
            
            float height = EditorGUI.GetPropertyHeight(property, label, true);
            height += titleMarginTop + titleHeight;
            
            if (!string.IsNullOrWhiteSpace(attribute.Subtitle))
                height += subtitleHeight + subtitleMarginBottom;
            if (attribute.Separator)
                height += separatorMarginTop + separatorHeight + separatorMarginBottom;
            
            return height;
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            TitleAttribute attribute = this.attribute as TitleAttribute;
            Texture iconTexture = IconDictionary.GetIconTexture(attribute.Icon);
            float offsetIfBottom = attribute.bottom
                ? EditorGUI.GetPropertyHeight(property) + EditorGUIUtility.standardVerticalSpacing
                : 0;
            
            // Create title style
            GUIStyle titleStyle = new(EditorStyles.boldLabel);
            titleStyle.fontStyle = GetFontStyleByTitleStyle(attribute.Style, titleStyle.fontStyle);
            titleStyle.alignment = GetMiddleAnchorByAlign(attribute.Align); // EditorStyles.boldLabel style uses Middle for text anchor
            
            // Draw title label
            Rect titleRect = position;
            titleRect.y += titleMarginTop + offsetIfBottom;
            titleRect.height = titleHeight;
            if (iconTexture != null) // Leave space for icon
            {
                titleRect.x += iconSize + iconMarginRight;
                titleRect.width += -iconSize + iconMarginRight;
            }
            GUI.Label(titleRect, new GUIContent(attribute.Title), titleStyle);
            
            // Draw icon image
            if (iconTexture != null)
            {
                // Create icon style
                GUIStyle iconStyle = new(EditorStyles.boldLabel);
                iconStyle.alignment = GetMiddleAnchorByAlign(attribute.Align); // EditorStyles.boldLabel style uses Middle for text anchor
                iconStyle.fixedHeight = iconSize; // reduce height to make icon smaller
                iconStyle.contentOffset = new(iconStyle.contentOffset.x, iconMarginTop); // recenter after reducing height
                
                // Draw icon image
                Rect iconRect = position;
                iconRect.y += titleMarginTop + offsetIfBottom;
                iconRect.height = titleHeight;
                GUI.Label(iconRect, new GUIContent(iconTexture), iconStyle);
            }
            
            // Draw subtitle label
            bool hasSubtitle = !string.IsNullOrWhiteSpace(attribute.Subtitle);
            if (hasSubtitle)
            {
                // Create subtitle style
                GUIStyle subtitleStyle = new(EditorStyles.miniLabel);
                subtitleStyle.fontStyle = GetFontStyleByTitleStyle(attribute.Style, EditorStyles.miniLabel.fontStyle);
                subtitleStyle.alignment = GetMiddleAnchorByAlign(attribute.Align);  // EditorStyles.boldLabel style uses Middle for text anchor
                subtitleStyle.normal.textColor = SetAlpha(subtitleStyle.normal.textColor, 0.6f);
                subtitleStyle.hover.textColor = SetAlpha(subtitleStyle.hover.textColor, 0.6f);
                
                // Calculate side offset, used to reduce remaining margin based on alignment
                float sideOffset = attribute.Align == Align.Left ? -0.5f :
                    attribute.Align == Align.Right ? 0.5f : 0f;
                subtitleStyle.contentOffset = new(sideOffset, subtitleStyle.contentOffset.y);
                
                // Draw subtitle label
                Rect subtitleRect = position;
                subtitleRect.y += titleMarginTop + titleHeight + offsetIfBottom;
                subtitleRect.height = subtitleHeight;
                GUI.Label(subtitleRect, new GUIContent(attribute.Subtitle), subtitleStyle);
            }
            
            // Draw separator line
            if (attribute.Separator)
            {
                // Separator color
                Color separatorColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
                separatorColor.a *= 0.15f;
                
                // Draw separator line
                Rect separatorRect = position;
                separatorRect.y += titleMarginTop + titleHeight + separatorMarginTop + offsetIfBottom;
                if (hasSubtitle)
                    separatorRect.y += subtitleHeight + subtitleMarginBottom;
                separatorRect.height = separatorHeight;
                
                EditorGUI.DrawRect(separatorRect, separatorColor);
            }
            
            // Draw property field
            Rect propertyRect = position;
            if (!attribute.bottom)
            {
                propertyRect.y += titleMarginTop + titleHeight;
                if (hasSubtitle)
                    propertyRect.y += subtitleHeight + subtitleMarginBottom;
                if (attribute.Separator)
                    propertyRect.y += separatorMarginTop + separatorHeight + separatorMarginBottom;
            }
            
            EditorGUI.PropertyField(propertyRect, property, label, true);
        }
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            TitleAttribute attribute = this.attribute as TitleAttribute;
            Texture iconTexture = IconDictionary.GetIconTexture(attribute.Icon);
            
            // UI Toolkit height settings
            float titleMarginTop = 13f; // based on standard header top margin
            float iconSize = 16f; // based on EditorGUIUtility.singleLineHeight - 2f
            float iconMarginLeft = 1f; // based on IMGUI where icon has little margin to the left
            
            // Create title wrapper
            var titleWrapper = AstrotypeEditorGUI.CreateDecoratorWrapper("title-wrapper");
            
            // Create title label
            var titleLabel = new Label(attribute.Title);
            titleLabel.AddToClassList("unity-header-drawer__label"); // contains top margin from standard Header attribute drawer
            
            titleLabel.style.unityFontStyleAndWeight = GetFontStyleByTitleStyle(attribute.Style, EditorStyles.boldLabel.fontStyle); // standard header uses FontStyle.Bold
            titleLabel.style.unityTextAlign = GetLowerAnchorByAlign(attribute.Align); // standard header uses Lower for text anchor
            if (iconTexture != null)
                titleLabel.style.marginLeft = iconSize + iconMarginLeft; // make space for icon
            titleWrapper.Add(titleLabel);
            
            // Create icon image
            Image iconImage = null;
            if (iconTexture != null)
            {
                iconImage = new();
                iconImage.image = iconTexture;
                iconImage.style.width = iconSize;
                iconImage.style.height = iconSize;
                iconImage.style.flexGrow = 0;
                iconImage.style.flexShrink = 0;
                
                iconImage.style.position = Position.Absolute;
                iconImage.style.marginTop = titleMarginTop;
                iconImage.style.marginLeft = iconMarginLeft;
                
                titleWrapper.Add(iconImage);
            }
            
            // Create subtitle label
            if (!string.IsNullOrWhiteSpace(attribute.Subtitle))
            {
                var subtitleLabel = new Label(attribute.Subtitle);
                subtitleLabel.style.fontSize = EditorStyles.miniLabel.fontSize;
                subtitleLabel.style.unityFontStyleAndWeight = GetFontStyleByTitleStyle(attribute.Style, EditorStyles.miniLabel.fontStyle);
                subtitleLabel.style.unityTextAlign = GetLowerAnchorByAlign(attribute.Align);
                
                Color subtitleColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
                subtitleColor.a *= 0.4f;
                subtitleLabel.style.color = subtitleColor;
                
                subtitleLabel.style.marginTop = 2f;
                subtitleLabel.style.marginBottom = 1f;
                
                titleWrapper.Add(subtitleLabel);
            }
            
            // Create separator line
            if (attribute.Separator)
            {
                var separatorLine = new VisualElement();
                Color separatorColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
                separatorColor.a *= 0.15f;
                separatorLine.style.backgroundColor = separatorColor;
                
                float separatorHeight = 2;
                separatorLine.style.borderTopWidth = Mathf.Ceil(separatorHeight / 2);
                separatorLine.style.borderBottomWidth = Mathf.Floor(separatorHeight / 2);
                separatorLine.style.marginTop = 2;
                separatorLine.style.marginBottom = 2;
                
                titleWrapper.Add(separatorLine);
            }
            
            
            // Create property field
            var propertyField = new PropertyField(property);
            propertyField.schedule.Execute(() =>
            {
                // Unwrap property field wrapper
                propertyField.UnwrapElement(out var parent);
                
                // Add decorative elements inside top/bottom decorator drawers container
                if (attribute.bottom)
                    parent.AddToBottomDecoratorContainer(titleWrapper);
                else
                    parent.AddToDecoratorContainer(titleWrapper);
            });
            
            return propertyField;
        }
        
        
        private static FontStyle GetFontStyleByTitleStyle(TitleStyle titleStyle, FontStyle defaultFontStyle)
        {
            return titleStyle switch
            {
                TitleStyle.Normal => FontStyle.Normal,
                TitleStyle.Bold => FontStyle.Bold,
                TitleStyle.Italic => FontStyle.Italic,
                TitleStyle.BoldAndItalic => FontStyle.BoldAndItalic,
                
                TitleStyle.DefaultItalic =>
                    defaultFontStyle == FontStyle.Normal ? FontStyle.Italic
                    : defaultFontStyle == FontStyle.Bold ? FontStyle.BoldAndItalic
                    : defaultFontStyle,
                
                _ => defaultFontStyle
            };
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
