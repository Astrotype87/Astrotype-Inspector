using System;
using System.Diagnostics;
using UnityEngine;

namespace AstrotypeInspector
{
    [Conditional(Symbols.UNITY_EDITOR), Conditional(Symbols.INCLUDE_IN_BUILD)]
    [AttributeUsage(AttributeTargets.Field)]
    public class MinValueAttribute : PropertyAttribute
    {
        public readonly float MinX;
        public readonly float MinY;
        public readonly float MinZ;
        public readonly float MinW;
        
        public MinValueAttribute(float min)
        {
            MinX = min;
            MinY = min;
            MinZ = min;
            MinW = min;
        }
        
        public MinValueAttribute(float minX, float minY)
        {
            MinX = minX;
            MinY = minY;
            MinZ = 0;
            MinW = 0;
        }
        
        public MinValueAttribute(float minX, float minY, float minZ)
        {
            MinX = minX;
            MinY = minY;
            MinZ = minZ;
            MinW = 0;
        }
        
        public MinValueAttribute(float minX, float minY, float minZ, float minW)
        {
            MinX = minX;
            MinY = minY;
            MinZ = minZ;
            MinW = minW;
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
        private const string InvalidTypeMessage = "Use MinValue with float, int or Vector.";
        
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
                    property.floatValue = Mathf.Max(attribute.MinX, property.floatValue);
                }
                else if (property.propertyType == SerializedPropertyType.Integer)
                {
                    property.intValue = Mathf.Max((int)attribute.MinX, property.intValue);
                }
                else if (property.propertyType == SerializedPropertyType.Vector2)
                {
                    Vector2 value = property.vector2Value;
                    property.vector2Value = new(
                        Mathf.Max(attribute.MinX, value.x),
                        Mathf.Max(attribute.MinY, value.y));
                }
                else if (property.propertyType == SerializedPropertyType.Vector2Int)
                {
                    Vector2Int value = property.vector2IntValue;
                    property.vector2IntValue = new(
                        Mathf.Max((int)attribute.MinX, value.x),
                        Mathf.Max((int)attribute.MinY, value.y));
                }
                else if (property.propertyType == SerializedPropertyType.Vector3)
                {
                    Vector3 value = property.vector3Value;
                    property.vector3Value = new(
                        Mathf.Max(attribute.MinX, value.x),
                        Mathf.Max(attribute.MinY, value.y),
                        Mathf.Max(attribute.MinZ, value.z));
                }
                else if (property.propertyType == SerializedPropertyType.Vector3Int)
                {
                    Vector3Int value = property.vector3IntValue;
                    property.vector3IntValue = new(
                        Mathf.Max((int)attribute.MinX, value.x),
                        Mathf.Max((int)attribute.MinY, value.y),
                        Mathf.Max((int)attribute.MinZ, value.z));
                }
                else if (property.propertyType == SerializedPropertyType.Vector4)
                {
                    Vector4 value = property.vector4Value;
                    property.vector4Value = new(
                        Mathf.Max(attribute.MinX, value.x),
                        Mathf.Max(attribute.MinY, value.y),
                        Mathf.Max(attribute.MinZ, value.z),
                        Mathf.Max(attribute.MinW, value.w));
                }
                else
                {
                    EditorGUI.LabelField(position, label.text, InvalidTypeMessage);
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
                                property.floatValue = Mathf.Max(attribute.MinX, field.value);
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
