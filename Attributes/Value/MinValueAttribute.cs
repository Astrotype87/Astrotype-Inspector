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
        
        public MinValueAttribute(float minX, float minY = 0, float minZ = 0, float minW = 0)
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

    [CustomPropertyDrawer(typeof(MinValueAttribute))]
    public class MinValueDrawer : PropertyDrawer
    {
        private const string InvalidTypeMessage = "Use MinValue with numeric or vector types.";
        
        private bool isFocused;
        private bool isDragging;
        
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!IsNumericOrVector(property.propertyType))
            {
                EditorGUI.LabelField(position, label.text, InvalidTypeMessage);
                return;
            }
            
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, property, label, true);
            if (EditorGUI.EndChangeCheck())
            {
                var attribute = this.attribute as MinValueAttribute;
                
                if (property.propertyType == SerializedPropertyType.Float)
                    property.floatValue = ValidateMin(attribute, property.floatValue);
                
                else if (property.propertyType == SerializedPropertyType.Integer)
                    property.intValue = ValidateMin(attribute, property.intValue);
                
                else if (property.propertyType == SerializedPropertyType.Vector2)
                    property.vector2Value = ValidateMin(attribute, property.vector2Value);
                
                else if (property.propertyType == SerializedPropertyType.Vector2Int)
                    property.vector2IntValue = ValidateMin(attribute, property.vector2IntValue);
                
                else if (property.propertyType == SerializedPropertyType.Vector3)
                    property.vector3Value = ValidateMin(attribute, property.vector3Value);
                
                else if (property.propertyType == SerializedPropertyType.Vector3Int)
                    property.vector3IntValue = ValidateMin(attribute, property.vector3IntValue);
                
                else if (property.propertyType == SerializedPropertyType.Vector4)
                    property.vector4Value = ValidateMin(attribute, property.vector4Value);
            }
        }
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            if (!IsNumericOrVector(property.propertyType))
            {
                var label = new Label(InvalidTypeMessage);
                label.AddToClassList("unity-base-field__label");
                label.style.marginLeft = 3;
                label.style.marginBottom = 1;
                label.style.marginTop = 1;
                label.style.marginRight = 0;
                return label;
            }
            
            // Create property field
            var propertyField = new PropertyField(property);
            propertyField.schedule.Execute(() =>
            {
                // Unwrap property field wrapper
                propertyField.UnwrapElement(out var parent);
                var attribute = this.attribute as MinValueAttribute;
                
                if (property.propertyType == SerializedPropertyType.Float)
                {
                    if (property.type == "float")
                    {
                        var field = parent.Q<FloatField>();
                        RegisterNumericField(field, property.serializedObject,
                            () => property.floatValue = ValidateMin(attribute, field.value),
                            () => field.value = property.floatValue);
                    }
                    else if (property.type == "double")
                    {
                        var field = parent.Q<DoubleField>();
                        RegisterNumericField(field, property.serializedObject,
                            () => property.doubleValue = ValidateMin(attribute, field.value),
                            () => field.value = property.doubleValue);
                    }
                }
                else if (property.propertyType == SerializedPropertyType.Integer)
                {
                    if (property.type is "int" or "short" or "ushort" or "sbyte" or "byte")
                    {
                        var field = parent.Q<IntegerField>();
                        RegisterNumericField(field, property.serializedObject,
                            () => property.intValue = ValidateMin(attribute, field.value),
                            () => field.value = property.intValue);
                    }
                    else if (property.type == "long")
                    {
                        var field = parent.Q<LongField>();
                        RegisterNumericField(field, property.serializedObject,
                            () => property.longValue = ValidateMin(attribute, field.value),
                            () => field.value = property.longValue);
                    }
                    else if (property.type == "uint")
                    {
                        var field = parent.Q<UnsignedIntegerField>();
                        RegisterNumericField(field, property.serializedObject,
                            () => property.uintValue = ValidateMin(attribute, field.value),
                            () => field.value = property.uintValue);
                    }
                    else if (property.type == "ulong")
                    {
                        var field = parent.Q<UnsignedLongField>();
                        RegisterNumericField(field, property.serializedObject,
                            () => property.ulongValue = ValidateMin(attribute, field.value),
                            () => field.value = property.ulongValue);
                    }
                }
                else if (property.propertyType == SerializedPropertyType.Vector2)
                {
                    var field = parent.Q<Vector2Field>();
                    var floatFields = parent.Query<FloatField>().ToList();
                    RegisterVectorField<float>(field, property.serializedObject, floatFields,
                        () => property.vector2Value = ValidateMin(attribute, new Vector2(
                            floatFields[0].value, floatFields[1].value)),
                        (i) => floatFields[i].SetValueWithoutNotify(property.vector2Value[i]));
                }
                else if (property.propertyType == SerializedPropertyType.Vector2Int)
                {
                    var field = parent.Q<Vector2IntField>();
                    var intFields = parent.Query<IntegerField>().ToList();
                    RegisterVectorField<int>(field, property.serializedObject, intFields,
                        () => property.vector2IntValue = ValidateMin(attribute, new Vector2Int(
                            intFields[0].value, intFields[1].value)),
                        (i) => intFields[i].SetValueWithoutNotify(property.vector2IntValue[i]));
                }
                else if (property.propertyType == SerializedPropertyType.Vector3)
                {
                    var field = parent.Q<Vector3Field>();
                    var floatFields = parent.Query<FloatField>().ToList();
                    RegisterVectorField<float>(field, property.serializedObject, floatFields,
                        () => property.vector3Value = ValidateMin(attribute, new Vector3(
                            floatFields[0].value, floatFields[1].value, floatFields[2].value)),
                        (i) => floatFields[i].SetValueWithoutNotify(property.vector3Value[i]));
                }
                else if (property.propertyType == SerializedPropertyType.Vector3Int)
                {
                    var field = parent.Q<Vector3IntField>();
                    var intFields = parent.Query<IntegerField>().ToList();
                    RegisterVectorField<int>(field, property.serializedObject, intFields,
                        () => property.vector3IntValue = ValidateMin(attribute, new Vector3Int(
                            intFields[0].value, intFields[1].value, intFields[2].value)),
                        (i) => intFields[i].SetValueWithoutNotify(property.vector3IntValue[i]));
                }
                else if (property.propertyType == SerializedPropertyType.Vector4)
                {
                    // Vector4 is usually displayed as foldout, but check for Vector4Field as well
                    BindableElement field = parent.Q<Foldout>();
                    field ??= parent.Q<Vector4Field>();
                    var floatFields = parent.Query<FloatField>().ToList();
                    
                    if (field is Foldout)
                    {
                        foreach (var floatField in floatFields)
                        {
                            HandleFocusAndDragging(floatField, property.serializedObject);
                            floatField.RegisterCallback<ChangeEvent<float>>(_ =>
                            {
                                if (!isFocused) return;
                                property.vector4Value = ValidateMin(attribute, new Vector4(
                                    floatFields[0].value, floatFields[1].value, floatFields[2].value, floatFields[3].value));
                                property.serializedObject.ApplyModifiedProperties();
                                property.serializedObject.Update();
                                
                                if (isDragging) {
                                    for (int i = 0; i < floatFields.Count; i++)
                                        floatFields[i].value = property.vector4Value[i];
                                }
                            });
                        }
                    }
                    else if (field is Vector4Field)
                    {
                        RegisterVectorField<float>(field, property.serializedObject, floatFields,
                            () => property.vector4Value = ValidateMin(attribute, new Vector4(
                                floatFields[0].value, floatFields[1].value, floatFields[2].value, floatFields[3].value)),
                            (i) => floatFields[i].SetValueWithoutNotify(property.vector4Value[i]));
                    }
                }
            });
            
            return propertyField;
        }
        
        
        private bool IsNumericOrVector(SerializedPropertyType propertyType)
        {
            return propertyType
                is SerializedPropertyType.Float or SerializedPropertyType.Integer
                or SerializedPropertyType.Vector2 or SerializedPropertyType.Vector2Int
                or SerializedPropertyType.Vector3 or SerializedPropertyType.Vector3Int
                or SerializedPropertyType.Vector4;
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
        
        
        private void RegisterNumericField(
            BindableElement numericField,
            SerializedObject serializedObject,
            Action updateSerializedValue,
            Action updateNumericFieldValue)
        {
            HandleFocusAndDragging(numericField, serializedObject);
            numericField.RegisterCallback<InputEvent>(_ =>
            {
                if (!isFocused) return;
                updateSerializedValue();
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
                if (isDragging) updateNumericFieldValue();
            });
        }
        
        private void RegisterVectorField<TValue>(
            BindableElement vectorField,
            SerializedObject serializedObject,
            IReadOnlyList<BindableElement> numericFields,
            Action updateSerializedValue,
            Action<int> updateNumericFieldValue)
        {
            HandleFocusAndDragging(vectorField, serializedObject);
            foreach (var numericField in numericFields)
            {
                numericField.RegisterCallback<ChangeEvent<TValue>>(_ =>
                {
                    if (!isFocused) return;
                    updateSerializedValue();
                    serializedObject.ApplyModifiedProperties();
                    serializedObject.Update();
                    for (int i = 0; i < numericFields.Count; i++)
                        if (numericField != numericFields[i] || isDragging)
                            updateNumericFieldValue(i);
                });
            }
        }
        
        
        private static int ValidateMin(MinValueAttribute attribute, int value) =>
            Math.Max((int)attribute.MinX, value);
        
        private static uint ValidateMin(MinValueAttribute attribute, uint value) =>
            Math.Max(attribute.MinX < 0 ? 0 : (uint)attribute.MinX, value);
        
        private static long ValidateMin(MinValueAttribute attribute, long value) =>
            Math.Max((long)attribute.MinX, value);
        
        private static ulong ValidateMin(MinValueAttribute attribute, ulong value) =>
            Math.Max(attribute.MinX < 0 ? 0 : (ulong)attribute.MinX, value);
        
        private static float ValidateMin(MinValueAttribute attribute, float value) =>
            Math.Max(attribute.MinX, value);
        
        private static double ValidateMin(MinValueAttribute attribute, double value) =>
            Math.Max(attribute.MinX, value);
        
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
