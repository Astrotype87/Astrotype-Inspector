using System;
using System.Diagnostics;
using UnityEngine;

namespace AstrotypeInspector
{
    [Conditional(Symbols.UNITY_EDITOR), Conditional(Symbols.INCLUDE_IN_BUILD)]
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public sealed class SeparatorAttribute : DecorativeAttribute
    {
        public readonly float Height;
        public readonly float MarginTop;
        public readonly float MarginBottom;
        
        public SeparatorAttribute(float height = -1, float marginTop = -1, float marginBottom = -1) : base(true)
        {
            Height = height < 0 ? 2f : height;
            MarginTop = marginTop < 0 ? 2f : marginTop;
            MarginBottom = marginBottom < 0 ? 2f : marginBottom;
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
    
    [CustomPropertyDrawer(typeof(SeparatorAttribute))]
    public class SeparatorDrawer : PropertyDrawer
    {
        // IMGUI calculated height
        private float separatorMarginTop; // 1.5f;
        private float separatorHeight; // 2.75f - EditorGUIUtility.standardVerticalSpacing;
        private float separatorMarginBottom; // 2.75f;
        
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            UpdateIMGUISeparatorHeight();
            
            return separatorMarginTop + separatorHeight + separatorMarginBottom
                + EditorGUI.GetPropertyHeight(property, label, true);
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            UpdateIMGUISeparatorHeight();
            
            SeparatorAttribute attribute = this.attribute as SeparatorAttribute;
            float offsetIfBottom = attribute.bottom
                ? EditorGUI.GetPropertyHeight(property) + EditorGUIUtility.standardVerticalSpacing
                : 0;
            
            // Separator color
            Color separatorColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
            separatorColor.a *= 0.15f;
            
            // Draw separator line
            Rect separatorRect = position;
            separatorRect.y += separatorMarginTop + offsetIfBottom;
            separatorRect.height = separatorHeight;
            EditorGUI.DrawRect(separatorRect, separatorColor);
            
            // Draw property field
            Rect propertyRect = position;
            if (!attribute.bottom)
                propertyRect.y += separatorMarginTop + separatorHeight + separatorMarginBottom;
            
            EditorGUI.PropertyField(propertyRect, property, label, true);
        }
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SeparatorAttribute attribute = this.attribute as SeparatorAttribute;
            
            // UI Toolkit height settings
            float separatorMarginTop = attribute.MarginTop;
            float separatorHeight = attribute.Height;
            float separatorMarginBottom = attribute.MarginBottom;
            
            // Separator color
            Color separatorColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
            separatorColor.a *= 0.15f;
            
            // Create separator line
            var separatorLine = new VisualElement();
            separatorLine.name = "separator";
            separatorLine.style.backgroundColor = separatorColor;
            
            separatorLine.style.borderTopWidth = Mathf.Ceil(separatorHeight / 2);
            separatorLine.style.borderBottomWidth = Mathf.Floor(separatorHeight / 2);
            // separatorLine.style.height = separatorHeight;
            
            separatorLine.style.marginTop = separatorMarginTop;
            separatorLine.style.marginBottom = separatorMarginBottom;
            
            // Create property field
            var propertyField = new PropertyField(property);
            propertyField.schedule.Execute(() =>
            {
                // Unwrap property field wrapper
                propertyField.UnwrapElement(out var parent);
                
                // Add decorative elements inside top/bottom decorator drawers container
                if (attribute.bottom)
                    parent.AddToBottomDecoratorContainer(separatorLine);
                else
                    parent.AddToDecoratorContainer(separatorLine);
            });
            
            return propertyField;
        }
        
        
        private void UpdateIMGUISeparatorHeight()
        {
            SeparatorAttribute attribute = this.attribute as SeparatorAttribute;
            
            separatorHeight = attribute.Height;
            separatorMarginTop = attribute.MarginTop + 1f - EditorGUIUtility.standardVerticalSpacing;
            separatorMarginBottom = attribute.MarginBottom + 1f;
            
            // For default separator height of 2f
            // Make pixel consistent in IMGUI and UI Toolkit in 100% and 125% display scaling
            if (separatorHeight >= 2f && separatorHeight < 3f)
            {
                separatorHeight = 1.5f;
                separatorMarginBottom += 0.5f;
            }
        }
        
        private static float MapRangeUnclamped(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            return toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
        }
        
        
    }
    
}
#endif
