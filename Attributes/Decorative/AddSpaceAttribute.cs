using System;
using System.Diagnostics;
using UnityEngine;

namespace AstrotypeInspector
{
    [Conditional(Symbols.UNITY_EDITOR), Conditional(Symbols.INCLUDE_IN_BUILD)]
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public sealed class AddSpaceAttribute : DecorativeAttribute
    {
        public readonly float Height;
        
        public AddSpaceAttribute(float height = 8f)
        {
            Height = height;
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
    
    [CustomPropertyDrawer(typeof(AddSpaceAttribute))]
    public class AddSpaceDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var attribute = this.attribute as AddSpaceAttribute;
            return attribute.Height + EditorGUI.GetPropertyHeight(property, label, true);
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attribute = this.attribute as AddSpaceAttribute;
            
            // Draw property field
            Rect propertyRect = position;
            if (!attribute.bottom)
                propertyRect.y += attribute.Height;
            
            EditorGUI.PropertyField(propertyRect, property, label, true);
        }
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var attribute = this.attribute as AddSpaceAttribute;
            
            // Create empty visual element
            var emptySpace = new VisualElement();
            emptySpace.AddToClassList("unity-space-drawer");
            emptySpace.style.height = attribute.Height;
            
            // Create property field
            var propertyField = new PropertyField(property);
            propertyField.schedule.Execute(() =>
            {
                // Unwrap property field wrapper
                propertyField.UnwrapElement(out var parent);
                
                // Add decorative elements inside top/bottom decorator drawers container
                if (attribute.bottom)
                    parent.AddToBottomDecoratorContainer(emptySpace);
                else
                    parent.AddToDecoratorContainer(emptySpace);
            });
            
            return propertyField;
        }
    }
}
#endif
