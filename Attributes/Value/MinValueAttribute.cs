using System;
using System.Diagnostics;
using UnityEngine;

namespace AstrotypeInspector
{
    [Conditional(Symbols.UNITY_EDITOR), Conditional(Symbols.INCLUDE_IN_BUILD)]
    [AttributeUsage(AttributeTargets.Field)]
    public class MinValueAttribute : PropertyAttribute
    {
        public readonly float Min;
        
        public MinValueAttribute(float min)
        {
            Min = min;
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
    
    [CustomPropertyDrawer(typeof(MinValueAttribute))]
    public class MinValueDrawer : PropertyDrawer
    {
        private bool isFocused;
        private bool isHold;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, property, label, true);
            if (EditorGUI.EndChangeCheck())
            {
                var attribute = this.attribute as MinValueAttribute;
                
                if (property.propertyType == SerializedPropertyType.Float)
                {
                    property.floatValue = Mathf.Max(attribute.Min, property.floatValue);
                }
                else if (property.propertyType == SerializedPropertyType.Integer)
                {
                    property.intValue = Mathf.Max((int)attribute.Min, property.intValue);
                }
                else
                {
                    EditorGUI.LabelField(position, label.text, "Other types not implemented.");
                }
            }
        }
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var attribute = this.attribute as MinValueAttribute;
            
            // Create property field
            var propertyField = new PropertyField(property);
            propertyField.schedule.Execute(() =>
            {
                // Unwrap property field wrapper
                propertyField.UnwrapElement(out var parent);
                
                if (property.propertyType == SerializedPropertyType.Float)
                {
                    if (property.type == "float")
                    {
                        var field = parent.Q<FloatField>();
                        RegisterFocusBinding(field, property);
                        RegisterPointerCapture(field);
                        
                        field.RegisterCallback<InputEvent>(_ =>
                        {
                            if (isFocused)
                            {
                                property.floatValue = Mathf.Max(attribute.Min, field.value);
                                property.serializedObject.ApplyModifiedProperties();
                                property.serializedObject.Update();
                                if (isHold) field.value = property.floatValue;
                            }
                        });
                    }
                }
            });
            
            return propertyField;
        }
        
        
        private void RegisterFocusBinding(BindableElement element, SerializedProperty property)
        {
            element.RegisterCallback<FocusInEvent>(_ =>
            {
                isFocused = true;
                element.Unbind();
            });
            element.RegisterCallback<FocusOutEvent>(_ =>
            {
                isFocused = false;
                element.Bind(property.serializedObject);
            });
        }
        
        private void RegisterPointerCapture(VisualElement element)
        {
            var textFields = element.Query<VisualElement>(className: "unity-base-text-field").ToList();
            foreach (var textField in textFields)
            {
                Debug.Log($"{element.name} > {textField.name}");
                var label = textField.Q<Label>(className: "unity-base-text-field__label");
                if (label != null)
                {
                    label.RegisterCallback<PointerCaptureEvent>(_ => isHold = true);
                    label.RegisterCallback<PointerCaptureOutEvent>(_ => isHold = false);
                }
            }
        }
        
    }
}
#endif
