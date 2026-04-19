using System;
using System.Diagnostics;
using UnityEngine;

namespace AstrotypeInspector
{
    [Conditional(Symbols.UNITY_EDITOR), Conditional(Symbols.INCLUDE_IN_BUILD)]
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public abstract class ConditionalIfAttribute : WrapperAttribute
    {
        internal enum ConditionType { BoolExpression, EnumMatchValues, BoolExpressionGroup }
        internal readonly ConditionType Type;
        
        public readonly string BoolExpression;
        
        public readonly string EnumMemberName;
        public readonly object[] EnumMatchValues;
        
        public readonly BoolGroup BoolOperation;
        public readonly string[] BoolExpressions;
        
        
        protected ConditionalIfAttribute(string boolExpression) : base(true)
        {
            order = 0;
            Type = ConditionType.BoolExpression;
            BoolExpression = boolExpression;
        }
        
        protected ConditionalIfAttribute(string enumMemberName, params object[] enumMatchValues) : base(true)
        {
            order = 0;
            Type = ConditionType.EnumMatchValues;
            EnumMemberName = enumMemberName;
            EnumMatchValues = enumMatchValues;
        }
        
        protected ConditionalIfAttribute(BoolGroup boolOperation, params string[] boolExpressions) : base(true)
        {
            order = 0;
            Type = ConditionType.BoolExpressionGroup;
            BoolOperation = boolOperation;
            BoolExpressions = boolExpressions;
        }
        
    }
    
    public sealed class ShowIfAttribute : ConditionalIfAttribute
    {
        public ShowIfAttribute(string boolExpression) : base(boolExpression) { }
        public ShowIfAttribute(string enumMemberName, params object[] enumMatchValues) : base(enumMemberName, enumMatchValues) { }
        public ShowIfAttribute(BoolGroup boolOperation, params string[] boolExpressions) : base(boolOperation, boolExpressions) { }
    }
    
    public sealed class HideIfAttribute : ConditionalIfAttribute
    {
        public HideIfAttribute(string boolExpression) : base(boolExpression) { }
        public HideIfAttribute(string enumMemberName, params object[] enumMatchValues) : base(enumMemberName, enumMatchValues) { }
        public HideIfAttribute(BoolGroup boolOperation, params string[] boolExpressions) : base(boolOperation, boolExpressions) { }
    }
    
    public sealed class EnableIfAttribute : ConditionalIfAttribute
    {
        public EnableIfAttribute(string boolExpression) : base(boolExpression) { }
        public EnableIfAttribute(string enumMemberName, params object[] enumMatchValues) : base(enumMemberName, enumMatchValues) { }
        public EnableIfAttribute(BoolGroup boolOperation, params string[] boolExpressions) : base(boolOperation, boolExpressions) { }
    }
    
    public sealed class DisableIfAttribute : ConditionalIfAttribute
    {
        public DisableIfAttribute(string boolExpression) : base(boolExpression) { }
        public DisableIfAttribute(string enumMemberName, params object[] enumMatchValues) : base(enumMemberName, enumMatchValues) { }
        public DisableIfAttribute(BoolGroup boolOperation, params string[] boolExpressions) : base(boolOperation, boolExpressions) { }
    }
    
}

#if UNITY_EDITOR
namespace AstrotypeInspector.Editor
{
    using System;
    using System.Reflection;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using UnityEngine.UIElements;
    using UnityEditor.UIElements;
    
    using ConditionType = ConditionalIfAttribute.ConditionType;
    using System.Linq;

    [CustomPropertyDrawer(typeof(ConditionalIfAttribute), true)]
    public class ConditionalIfDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return IsVisible(property)
                ? EditorGUI.GetPropertyHeight(property, label, true)
                : -EditorGUIUtility.standardVerticalSpacing;
            
            // object targetObject = property.serializedObject.targetObject;
            // string relativePath = GetRelativePath(property);
            // var attributes = GetConditionalIfAttributes(fieldInfo);
            
            // float propertyHeight = EditorGUI.GetPropertyHeight(property, label, true);
            // return GetPropertyHeight(propertyHeight, targetObject, relativePath, attributes);
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // (bool isVisible, bool isEnabled) = IsVisibleAndEnabled(property);
            // if (!isVisible) return;
            
            // EditorGUI.BeginDisabledGroup(!isEnabled);
            // EditorGUI.PropertyField(position, property, label, true);
            // EditorGUI.EndDisabledGroup();
            
            
            object targetObject = property.serializedObject.targetObject;
            string relativePath = GetRelativePath(property);
            var attributes = GetConditionalIfAttributes(fieldInfo);
            
            // using (new Scope(targetObject, relativePath, attributes))
            // {
            //     EditorGUI.PropertyField(position, property, label, true);
            // }
            
            // Start conditional scope
            void DrawProperty() => EditorGUI.PropertyField(position, property, label, true);
            OnGUI(DrawProperty, targetObject, relativePath, attributes);
        }
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var propertyField = new PropertyField(property);
            propertyField.name = "conditional-if-wrapper";
            
            object targetObject = property.serializedObject.targetObject;
            string relativePath = GetRelativePath(property);
            var attributes = GetConditionalIfAttributes(fieldInfo);
            
            // Define property field update for editor update events
            void UpdatePropertyField()
            {
                if (property == null) return;
                if (property.serializedObject == null) return;
                if (property.serializedObject.targetObject == null) return;
                
                (bool isVisible, bool isEnabled) = EvaluateVisibleAndEnabled(targetObject, relativePath, attributes);
                propertyField.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
                propertyField.SetEnabled(isEnabled);
            }
            
            // Subscribe to inspector start and input update events
            propertyField.schedule.Execute(() => UpdatePropertyField());
            propertyField.TrackSerializedObjectValue(property.serializedObject, _ => UpdatePropertyField());
            
            return propertyField;
        }
        
        
        // PUBLIC STATIC MEMBERS
        public static float GetPropertyHeight(float propertyHeight, object targetObject, string relativePath,
            params ConditionalIfAttribute[] attributes)
        {
            return EvaluateVisible(targetObject, relativePath, attributes)
                ? propertyHeight
                : -EditorGUIUtility.standardVerticalSpacing;
        }
        
        public static void OnGUI(Action drawProperty, object targetObject, string relativePath,
            params ConditionalIfAttribute[] attributes)
        {
            (bool isVisible, bool isEnabled) = EvaluateVisibleAndEnabled(targetObject, relativePath, attributes);
            if (!isVisible) return;
            
            EditorGUI.BeginDisabledGroup(!isEnabled);
            drawProperty.Invoke();
            EditorGUI.EndDisabledGroup();
        }
        
        // public struct Scope : IDisposable
        // {
        //     public Scope(object targetObject, string relativePath, params ConditionalIfAttribute[] attributes)
        //     {
        //         (bool isVisible, bool isEnabled) = EvaluateVisibleAndEnabled(targetObject, relativePath, attributes);
        //         EditorGUI.BeginDisabledGroup(!isEnabled);
                
        //         return;
        //     }
            
        //     public readonly void Dispose()
        //     {
        //         EditorGUI.EndDisabledGroup();
        //     }
        // }
        
        
        
        private bool IsVisible(SerializedProperty property)
        {
            // Get the target object of serialized property
            object targetObject = property.serializedObject.targetObject;
            
            // Get the parent path of serialized property relative to the target object
            string propertyPath = property.propertyPath;
            int lastDotIndex = propertyPath.LastIndexOf('.');
            string relativePath = propertyPath.LastIndexOf('.') > 0
                ? propertyPath[..lastDotIndex]
                : string.Empty;
            
            // Get conditional if attributes for the current property
            if (!_fieldInfoAttributesCache.TryGetValue(fieldInfo, out ConditionalIfAttribute[] attributes))
            {
                attributes = fieldInfo.GetCustomAttributes<ConditionalIfAttribute>(true).ToArray();
                _fieldInfoAttributesCache[fieldInfo] = attributes;
            }
            
            // Evaluate visible state
            return EvaluateVisible(targetObject, relativePath, attributes);
        }
        
        private (bool, bool) IsVisibleAndEnabled(SerializedProperty property)
        {
            // Get the target object of serialized property
            object targetObject = property.serializedObject.targetObject;
            
            // Get the parent path of serialized property relative to the target object
            string propertyPath = property.propertyPath;
            int lastDotIndex = propertyPath.LastIndexOf('.');
            string relativePath = propertyPath.LastIndexOf('.') > 0
                ? propertyPath[..lastDotIndex]
                : string.Empty;
            
            // Get conditional if attributes for the current property
            if (!_fieldInfoAttributesCache.TryGetValue(fieldInfo, out ConditionalIfAttribute[] attributes))
            {
                attributes = fieldInfo.GetCustomAttributes<ConditionalIfAttribute>(true).ToArray();
                _fieldInfoAttributesCache[fieldInfo] = attributes;
            }
            
            // Evaluate visible and enable state
            return EvaluateVisibleAndEnabled(targetObject, relativePath, attributes);
        }
        
        
        // PRIVATE STATIC MEMBERS
        private static readonly Dictionary<FieldInfo, ConditionalIfAttribute[]> _fieldInfoAttributesCache = new();
        
        private static string GetRelativePath(SerializedProperty property)
        {
            string propertyPath = property.propertyPath;
            int lastDotIndex = propertyPath.LastIndexOf('.');
            
            return propertyPath.LastIndexOf('.') > 0
                ? propertyPath[..lastDotIndex]
                : string.Empty;
        }
        
        private static ConditionalIfAttribute[] GetConditionalIfAttributes(FieldInfo fieldInfo)
        {
            if (!_fieldInfoAttributesCache.TryGetValue(fieldInfo, out ConditionalIfAttribute[] attributes))
            {
                attributes = fieldInfo.GetCustomAttributes<ConditionalIfAttribute>(true).ToArray();
                _fieldInfoAttributesCache[fieldInfo] = attributes;
            }
            return attributes;
        }
        
        
        private static (bool, bool) EvaluateVisibleAndEnabled(
            object targetObject, string relativePath, params ConditionalIfAttribute[] attributes)
        {
            bool isVisible = true, isEnabled = true;
            bool isVisibleSet = false, isEnabledSet = false;
            
            foreach (var attribute in attributes)
            {
                bool condition = EvaluateCondition(targetObject, relativePath, attribute);
                
                if (attribute is ShowIfAttribute)
                {
                    if (!isVisibleSet) { isVisibleSet = true; isVisible = false; }
                    if (condition) isVisible = true;
                }
                else if (attribute is HideIfAttribute)
                {
                    if (!isVisibleSet) { isVisibleSet = true; isVisible = true; }
                    if (condition) isVisible = false;
                }
                else if (attribute is EnableIfAttribute)
                {
                    if (!isEnabledSet) { isEnabledSet = true; isEnabled = false; }
                    if (condition) isEnabled = true;
                }
                else if (attribute is DisableIfAttribute)
                {
                    if (!isEnabledSet) { isEnabledSet = true; isEnabled = true; }
                    if (condition) isEnabled = false;
                }
            }
            
            return (isVisible, isEnabled);
        }
        
        private static bool EvaluateVisible(
            object targetObject, string relativePath, params ConditionalIfAttribute[] attributes)
        {
            bool isVisible = true;
            bool isVisibleSet = false;
            
            foreach (var attribute in attributes)
            {
                bool condition = EvaluateCondition(targetObject, relativePath, attribute);
                
                if (attribute is ShowIfAttribute)
                {
                    if (!isVisibleSet) { isVisibleSet = true; isVisible = false; }
                    if (condition) isVisible = true;
                }
                else if (attribute is HideIfAttribute)
                {
                    if (!isVisibleSet) { isVisibleSet = true; isVisible = true; }
                    if (condition) isVisible = false;
                }
            }
            
            return isVisible;
        }
        
        
        private static bool EvaluateCondition(object targetObject, string relativePath, ConditionalIfAttribute attribute)
        {
            // Evaluate expression based on condition type
            if (attribute.Type == ConditionType.BoolExpression)
                return EvaluateBoolExpression(targetObject, relativePath, attribute.BoolExpression);
            else if (attribute.Type == ConditionType.EnumMatchValues)
                return EvaluateEnumMatchValues(targetObject, relativePath, attribute.EnumMemberName, attribute.EnumMatchValues);
            else if (attribute.Type == ConditionType.BoolExpressionGroup)
                return EvaluateBoolExpressionGroup(targetObject, relativePath, attribute.BoolOperation, attribute.BoolExpressions);
            else
                throw new NotImplementedException($"The evaluation logic for {attribute.Type} is not implemented.");
        }
        
        private static bool EvaluateBoolExpression(object targetObject, string relativePath, string boolExpression)
        {
            // HACK: Temporary placeholder before ConditionalExpressionParser is implemented
            string memberPath = string.IsNullOrWhiteSpace(relativePath)
                ? boolExpression : $"{relativePath}.{boolExpression}";
            object memberValue = MemberAccessCache.GetMemberValue(targetObject, memberPath);
            
            // Return bool value or return false if unity object reference or serialized reference is null
            if (memberValue is bool boolValue)
                return boolValue;
            else if (memberValue is UnityEngine.Object unityObject)
                return unityObject != null;
            else if (memberValue == null)
                return false;
            
            // Return false if not bool value or null check
            // TODO: [WARNING CHECKPOINT] Add invalid expression warning.
            Debug.Log($"Expression {boolExpression} does not return bool.");
            return false;
        }
        
        private static bool EvaluateEnumMatchValues(object targetObject, string relativePath,
            string enumMemberName, object[] enumMatchValues)
        {
            // Get member path and value
            string memberPath = string.IsNullOrWhiteSpace(relativePath)
                ? enumMemberName : $"{relativePath}.{enumMemberName}";
            object memberValue = MemberAccessCache.GetMemberValue(targetObject, memberPath);
            
            // Return true if one enum value matches
            foreach (object enumValue in enumMatchValues)
                if (Equals(memberValue, enumValue)) return true;
            
            // Return false if no enum value matches
            return false;
        }
        
        private static bool EvaluateBoolExpressionGroup(object targetObject, string relativePath,
            BoolGroup boolOperation, string[] boolExpressions)
        {
            bool IsTrue(string expression) => EvaluateBoolExpression(targetObject, relativePath, expression);
            
            if (boolOperation is BoolGroup.All) // no false values
            {
                foreach (string expression in boolExpressions)
                    if (!IsTrue(expression)) return false;
                return true;
            }
            else if (boolOperation is BoolGroup.Any) // at least one true value
            {
                foreach (string expression in boolExpressions)
                    if (IsTrue(expression)) return true;
                return false;
            }
            else if (boolOperation is BoolGroup.NotAll) // at least one false value
            {
                foreach (string expression in boolExpressions)
                    if (!IsTrue(expression)) return true;
                return false;
            }
            else if (boolOperation is BoolGroup.None) // no true values
            {
                foreach (string expression in boolExpressions)
                    if (IsTrue(expression)) return false;
                return true;
            }
            else if (boolOperation is BoolGroup.Odd or BoolGroup.Even) // odd or even true values
            {
                bool isOdd = false;
                foreach (string expression in boolExpressions)
                    isOdd ^= IsTrue(expression);
                return boolOperation == BoolGroup.Odd ? isOdd : !isOdd;
            }
            else if (boolOperation is BoolGroup.Imply) // at least one false value before last, or last value is true
            {
                if (boolExpressions.Length == 0) return true;
                
                for (int i = 0; i < boolExpressions.Length - 1; i++)
                    if (!IsTrue(boolExpressions[i])) return true;
                return IsTrue(boolExpressions[^1]);
            }
            else if (boolOperation is BoolGroup.NotImply) // no false values except last value is false
            {
                if (boolExpressions.Length == 0) return false;
                
                for (int i = 0; i < boolExpressions.Length - 1; i++)
                    if (!IsTrue(boolExpressions[i])) return false;
                return !IsTrue(boolExpressions[^1]);
            }
            else if (boolOperation is BoolGroup.Same) // no different values
            {
                if (boolExpressions.Length == 0) return true;
                
                bool firstValue = IsTrue(boolExpressions[0]);
                for (int i = 1; i < boolExpressions.Length; i++)
                    if (firstValue != IsTrue(boolExpressions[i])) return false;
                return true;
            }
            else if (boolOperation is BoolGroup.Different) // at least one different value
            {
                if (boolExpressions.Length == 0) return false;
                
                bool firstValue = IsTrue(boolExpressions[0]);
                for (int i = 1; i < boolExpressions.Length; i++)
                    if (firstValue != IsTrue(boolExpressions[i])) return true;
                return false;
            }
            else
            {
                throw new NotImplementedException($"The evaluation logic for {boolOperation} is not implemented.");
            }
        }
        
    }
    
}
#endif
