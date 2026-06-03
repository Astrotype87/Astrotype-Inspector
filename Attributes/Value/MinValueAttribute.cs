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
                            if (!isFocused) return;
                            property.floatValue = Mathf.Max(attribute.MinX, field.value);
                            property.serializedObject.ApplyModifiedProperties();
                            property.serializedObject.Update();
                            if (isDragging) field.value = property.floatValue;
                        });
                    }
                    else if (property.type == "double")
                    {
                        var field = parent.Q<DoubleField>();
                        HandleFocusAndDragging(field, property.serializedObject);
                        field.RegisterCallback<InputEvent>(_ =>
                        {
                            if (!isFocused) return;
                            property.doubleValue = Math.Max(attribute.MinX, field.value);
                            property.serializedObject.ApplyModifiedProperties();
                            property.serializedObject.Update();
                            if (isDragging) field.value = property.doubleValue;
                        });
                    }
                }
                else if (property.propertyType == SerializedPropertyType.Integer)
                {
                    if (property.type is "int" or "short" or "ushort" or "sbyte" or "byte")
                    {
                        var field = parent.Q<IntegerField>();
                        HandleFocusAndDragging(field, property.serializedObject);
                        field.RegisterCallback<InputEvent>(_ =>
                        {
                            if (!isFocused) return;
                            property.intValue = Mathf.Max((int)attribute.MinX, field.value);
                            property.serializedObject.ApplyModifiedProperties();
                            property.serializedObject.Update();
                            if (isDragging) field.value = property.intValue;
                        });
                    }
                    else if (property.type == "long")
                    {
                        var field = parent.Q<LongField>();
                        HandleFocusAndDragging(field, property.serializedObject);
                        field.RegisterCallback<InputEvent>(_ =>
                        {
                            if (!isFocused) return;
                            property.longValue = Math.Max((long)attribute.MinX, field.value);
                            property.serializedObject.ApplyModifiedProperties();
                            property.serializedObject.Update();
                            if (isDragging) field.value = property.longValue;
                        });
                    }
                    else if (property.type == "uint")
                    {
                        var field = parent.Q<UnsignedIntegerField>();
                        HandleFocusAndDragging(field, property.serializedObject);
                        field.RegisterCallback<InputEvent>(_ =>
                        {
                            if (!isFocused) return;
                            property.uintValue = Math.Max((uint)attribute.MinX, field.value);
                            property.serializedObject.ApplyModifiedProperties();
                            property.serializedObject.Update();
                            if (isDragging) field.value = property.uintValue;
                        });
                    }
                    else if (property.type == "ulong")
                    {
                        var field = parent.Q<UnsignedLongField>();
                        HandleFocusAndDragging(field, property.serializedObject);
                        field.RegisterCallback<InputEvent>(_ =>
                        {
                            if (!isFocused) return;
                            property.ulongValue = Math.Max((ulong)attribute.MinX, field.value);
                            property.serializedObject.ApplyModifiedProperties();
                            property.serializedObject.Update();
                            if (isDragging) field.value = property.ulongValue;
                        });
                    }
                }
                else if (property.propertyType == SerializedPropertyType.Vector2)
                {
                    var field = parent.Q<Vector2Field>();
                    HandleFocusAndDragging(field, property.serializedObject);
                    
                    var floatFields = parent.Query<FloatField>().ToList();
                    foreach (var floatField in floatFields)
                    {
                        floatField.RegisterCallback<ChangeEvent<float>>(_ =>
                        {
                            if (!isFocused) return;
                            property.vector2Value = new(
                                Mathf.Max(attribute.MinX, floatFields[0].value),
                                Mathf.Max(attribute.MinY, floatFields[1].value));
                            property.serializedObject.ApplyModifiedProperties();
                            property.serializedObject.Update();
                            
                            for (int i = 0; i < floatFields.Count; i++)
                                if (floatField != floatFields[i] || isDragging)
                                    floatFields[i].SetValueWithoutNotify(property.vector2Value[i]);
                        });
                    }
                }
                else if (property.propertyType == SerializedPropertyType.Vector2Int)
                {
                    var field = parent.Q<Vector2IntField>();
                    HandleFocusAndDragging(field, property.serializedObject);
                    
                    var intFields = parent.Query<IntegerField>().ToList();
                    foreach (var intField in intFields)
                    {
                        intField.RegisterCallback<ChangeEvent<int>>(_ =>
                        {
                            if (!isFocused) return;
                            property.vector2IntValue = new(
                                Mathf.Max((int)attribute.MinX, intFields[0].value),
                                Mathf.Max((int)attribute.MinY, intFields[1].value));
                            property.serializedObject.ApplyModifiedProperties();
                            property.serializedObject.Update();
                            
                            for (int i = 0; i < intFields.Count; i++)
                                if (intField != intFields[i] || isDragging)
                                    intFields[i].SetValueWithoutNotify(property.vector2IntValue[i]);
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
                            if (!isFocused) return;
                            property.vector3Value = new(
                                Mathf.Max(attribute.MinX, floatFields[0].value),
                                Mathf.Max(attribute.MinY, floatFields[1].value),
                                Mathf.Max(attribute.MinZ, floatFields[2].value));
                            property.serializedObject.ApplyModifiedProperties();
                            property.serializedObject.Update();
                            
                            for (int i = 0; i < floatFields.Count; i++)
                                if (floatField != floatFields[i] || isDragging)
                                    floatFields[i].SetValueWithoutNotify(property.vector3Value[i]);
                        });
                    }
                }
                else if (property.propertyType == SerializedPropertyType.Vector3Int)
                {
                    var field = parent.Q<Vector3IntField>();
                    HandleFocusAndDragging(field, property.serializedObject);
                    
                    var intFields = parent.Query<IntegerField>().ToList();
                    foreach (var intField in intFields)
                    {
                        intField.RegisterCallback<ChangeEvent<int>>(_ =>
                        {
                            if (!isFocused) return;
                            property.vector3IntValue = new(
                                Mathf.Max((int)attribute.MinX, intFields[0].value),
                                Mathf.Max((int)attribute.MinY, intFields[1].value),
                                Mathf.Max((int)attribute.MinZ, intFields[2].value));
                            property.serializedObject.ApplyModifiedProperties();
                            property.serializedObject.Update();
                            
                            for (int i = 0; i < intFields.Count; i++)
                                if (intField != intFields[i] || isDragging)
                                    intFields[i].SetValueWithoutNotify(property.vector3IntValue[i]);
                        });
                    }
                }
                else if (property.propertyType == SerializedPropertyType.Vector4)
                {
                    // Vector4 is usually displayed as foldout, but check for Vector4Field as well
                    BindableElement field = parent.Q<Foldout>();
                    field ??= parent.Q<Vector4Field>();
                    
                    var floatFields = parent.Query<FloatField>().ToList();
                    foreach (var floatField in floatFields)
                    {
                        HandleFocusAndDragging(floatField, property.serializedObject);
                        if (field is Foldout)
                        {
                            floatField.RegisterCallback<ChangeEvent<float>>(_ =>
                            {
                                if (!isFocused) return;
                                property.vector4Value = new(
                                    Mathf.Max(attribute.MinX, floatFields[0].value),
                                    Mathf.Max(attribute.MinY, floatFields[1].value),
                                    Mathf.Max(attribute.MinZ, floatFields[2].value),
                                    Mathf.Max(attribute.MinW, floatFields[3].value));
                                property.serializedObject.ApplyModifiedProperties();
                                property.serializedObject.Update();
                                if (isDragging)
                                {
                                    floatFields[0].value = property.vector4Value.x;
                                    floatFields[1].value = property.vector4Value.y;
                                    floatFields[2].value = property.vector4Value.z;
                                    floatFields[3].value = property.vector4Value.w;
                                }
                            });
                        }
                        else if (field is Vector4Field vector4Field)
                        {
                            floatField.RegisterCallback<ChangeEvent<float>>(_ =>
                            {
                                if (!isFocused) return;
                                property.vector4Value = new(
                                    Mathf.Max(attribute.MinX, vector4Field.value.x),
                                    Mathf.Max(attribute.MinY, vector4Field.value.y),
                                    Mathf.Max(attribute.MinZ, vector4Field.value.z),
                                    Mathf.Max(attribute.MinW, vector4Field.value.w));
                                property.serializedObject.ApplyModifiedProperties();
                                property.serializedObject.Update();
                                if (isDragging) vector4Field.value = property.vector4Value;
                            });
                            
                        }
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
                isFocused = true;
                element.Unbind();
            });
            element.RegisterCallback<FocusOutEvent>(_ =>
            {
                isFocused = false;
                element.Bind(serializedObject);
            });
            
            // Detect numeric field label dragging
            var labels = element.Query<Label>(className: "unity-base-text-field__label").ToList();
            foreach (var label in labels)
            {
                label.RegisterCallback<PointerCaptureEvent>(_ => isDragging = true);
                label.RegisterCallback<PointerCaptureOutEvent>(_ => isDragging = false);
            }
        }
        
        
        private static byte ValidateMin(MinValueAttribute attribute, byte value) => Math.Max((byte)attribute.MinX, value);
        private static sbyte ValidateMin(MinValueAttribute attribute, sbyte value) => Math.Max((sbyte)attribute.MinX, value);
        private static short ValidateMin(MinValueAttribute attribute, short value) => Math.Max((short)attribute.MinX, value);
        private static ushort ValidateMin(MinValueAttribute attribute, ushort value) => Math.Max((ushort)attribute.MinX, value);
        
        private static int ValidateMin(MinValueAttribute attribute, int value) => Math.Max((int)attribute.MinX, value);
        private static uint ValidateMin(MinValueAttribute attribute, uint value) => Math.Max((uint)attribute.MinX, value);
        private static long ValidateMin(MinValueAttribute attribute, long value) => Math.Max((long)attribute.MinX, value);
        private static ulong ValidateMin(MinValueAttribute attribute, ulong value) => Math.Max((ulong)attribute.MinX, value);
        
        private static float ValidateMin(MinValueAttribute attribute, float value) => Math.Max(attribute.MinX, value);
        private static double ValidateMin(MinValueAttribute attribute, double value) => Math.Max(attribute.MinX, value);
        
        private static Vector2 ValidateMin(MinValueAttribute attribute, Vector2 value) =>
            new(Math.Max(attribute.MinX, value.x),
                Math.Max(attribute.MinY, value.y));
        
        private static Vector2Int ValidateMin(MinValueAttribute attribute, Vector2Int value) =>
            new(Math.Max((int)attribute.MinX, value.x),
                Math.Max((int)attribute.MinY, value.y));
        
        private static Vector3 ValidateMin(MinValueAttribute attribute, Vector3 value) =>
            new(Math.Max(attribute.MinX, value.x),
                Math.Max(attribute.MinY, value.y),
                Math.Max(attribute.MinZ, value.z));
        
        private static Vector3Int ValidateMin(MinValueAttribute attribute, Vector3Int value) =>
            new(Math.Max((int)attribute.MinX, value.x),
                Math.Max((int)attribute.MinY, value.y),
                Math.Max((int)attribute.MinZ, value.z));
        
        private static Vector4 ValidateMin(MinValueAttribute attribute, Vector4 value) =>
            new(Math.Max(attribute.MinX, value.x),
                Math.Max(attribute.MinY, value.y),
                Math.Max(attribute.MinZ, value.z),
                Math.Max(attribute.MinW, value.w));
    }
}
#endif
