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
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using UnityEngine.UIElements;
    using UnityEditor.UIElements;
    using System.Linq;

    [CustomPropertyDrawer(typeof(MinValueAttribute))]
    public class MinValueDrawer : PropertyDrawer
    {
        private const string InvalidTypeMessage = "Use MinValue with float, int or Vector.";
        
        private bool isFocused;
        private bool isDragging;
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
        
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
                        HandleFocusAndDragging(field, property.serializedObject);
                        field.RegisterCallback<InputEvent>(_ =>
                        {
                            Debug.Log($"[{property.name}] InputEvent");
                            if (isFocused)
                            {
                                property.floatValue = Mathf.Max(attribute.MinX, field.value);
                                property.serializedObject.ApplyModifiedProperties();
                                property.serializedObject.Update();
                                if (isDragging)
                                {
                                    Debug.Log($"UI field updated");
                                    field.value = property.floatValue;
                                }
                            }
                        });
                    }
                    else if (property.type == "double")
                    {
                        var field = parent.Q<DoubleField>();
                        HandleFocusAndDragging(field, property.serializedObject);
                        field.RegisterCallback<InputEvent>(_ =>
                        {
                            if (isFocused)
                            {
                                property.doubleValue = Math.Max(attribute.MinX, field.value);
                                property.serializedObject.ApplyModifiedProperties();
                                property.serializedObject.Update();
                                if (isDragging) field.value = property.doubleValue;
                            }
                        });
                    }
                }
                else if (property.propertyType == SerializedPropertyType.Integer)
                {
                    if (property.type == "int")
                    {
                        var field = parent.Q<IntegerField>();
                        HandleFocusAndDragging(field, property.serializedObject);
                        field.RegisterCallback<InputEvent>(_ =>
                        {
                            if (isFocused)
                            {
                                property.intValue = Mathf.Max((int)attribute.MinX, field.value);
                                property.serializedObject.ApplyModifiedProperties();
                                property.serializedObject.Update();
                                if (isDragging) field.value = property.intValue;
                            }
                        });
                    }
                    else if (property.type == "long")
                    {
                        var field = parent.Q<LongField>();
                        HandleFocusAndDragging(field, property.serializedObject);
                        field.RegisterCallback<InputEvent>(_ =>
                        {
                            if (isFocused)
                            {
                                property.longValue = Math.Max((long)attribute.MinX, field.value);
                                property.serializedObject.ApplyModifiedProperties();
                                property.serializedObject.Update();
                                if (isDragging) field.value = property.longValue;
                            }
                        });
                    }
                    else if (property.type == "uint")
                    {
                        var field = parent.Q<UnsignedIntegerField>();
                        HandleFocusAndDragging(field, property.serializedObject);
                        field.RegisterCallback<InputEvent>(_ =>
                        {
                            if (isFocused)
                            {
                                property.uintValue = Math.Max((uint)attribute.MinX, field.value);
                                property.serializedObject.ApplyModifiedProperties();
                                property.serializedObject.Update();
                                if (isDragging) field.value = property.uintValue;
                            }
                        });
                    }
                    else if (property.type == "ulong")
                    {
                        var field = parent.Q<UnsignedLongField>();
                        HandleFocusAndDragging(field, property.serializedObject);
                        field.RegisterCallback<InputEvent>(_ =>
                        {
                            if (isFocused)
                            {
                                property.ulongValue = Math.Max((ulong)attribute.MinX, field.value);
                                property.serializedObject.ApplyModifiedProperties();
                                property.serializedObject.Update();
                                if (isDragging) field.value = property.ulongValue;
                            }
                        });
                    }
                }
                else if (property.propertyType == SerializedPropertyType.Vector3)
                {
                    var field = parent.Q<Vector3Field>();
                    HandleFocusAndDragging(field, property.serializedObject);
                    
                    var floatFields = parent.Query<FloatField>().ToList();
                    foreach (var floatField in floatFields)
                    {
                        floatField.RegisterCallback<ChangeEvent<float>>(_ =>
                        {
                            Debug.Log($"[{field.name}] InputEvent");
                            if (isFocused)
                            {
                                property.vector3Value = new(
                                    Mathf.Max(attribute.MinX, field.value.x),
                                    Mathf.Max(attribute.MinY, field.value.y),
                                    Mathf.Max(attribute.MinZ, field.value.z));
                                property.serializedObject.ApplyModifiedProperties();
                                property.serializedObject.Update();
                                
                                if (isDragging)
                                {
                                    Debug.Log($"UI field updated");
                                    field.value = property.vector3Value;
                                }
                            }
                        });
                    }
                }
            });
            
            return propertyField;
        }
        
        
        private void HandleFocusAndDragging(BindableElement element, SerializedObject serializedObject)
        {
            // Detect property field focus
            element.RegisterCallback<FocusInEvent>(_ =>
            {
                Debug.Log($"{element.name} => FocusInEvent --- isFocused = true, Unbind()");
                isFocused = true;
                element.Unbind();
            });
            element.RegisterCallback<FocusOutEvent>(_ =>
            {
                Debug.Log($"{element.name} => FocusOutEvent --- isFocused = false, Bind()");
                isFocused = false;
                element.Bind(serializedObject);
            });
            
            // Detect numeric field label dragging
            var labels = element.Query<Label>(className: "unity-base-text-field__label").ToList();
            foreach (var label in labels)
            {
                label.RegisterCallback<PointerCaptureEvent>(_ =>
                {
                    Debug.Log($"{element.name} => PointerCaptureEvent --- isDragging = true");
                    isDragging = true;
                });
                label.RegisterCallback<PointerCaptureOutEvent>(_ =>
                {
                    Debug.Log($"{element.name} => PointerCaptureOutEvent --- isDragging = false");
                    isDragging = false;
                });
            }
        }
        
    }
}
#endif
