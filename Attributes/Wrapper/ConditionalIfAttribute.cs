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
            Type = ConditionType.BoolExpression;
            BoolExpression = boolExpression;
        }
        
        protected ConditionalIfAttribute(string enumMemberName, params object[] enumMatchValues) : base(true)
        {
            Type = ConditionType.EnumMatchValues;
            EnumMemberName = enumMemberName;
            EnumMatchValues = enumMatchValues;
        }
        
        protected ConditionalIfAttribute(BoolGroup boolOperation, params string[] boolExpressions) : base(true)
        {
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
    using UnityEngine;
    using UnityEditor;
    using UnityEngine.UIElements;
    using UnityEditor.UIElements;
    
    using ConditionType = ConditionalIfAttribute.ConditionType;
    
    [CustomPropertyDrawer(typeof(ConditionalIfAttribute), true)]
    public class ConditionalIfDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var attribute = this.attribute as ConditionalIfAttribute;
            bool condition = EvaluateCondition(property, attribute);
            
            return IsVisible(condition, attribute)
                ? EditorGUI.GetPropertyHeight(property, label, true)
                : -EditorGUIUtility.standardVerticalSpacing;
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attribute = this.attribute as ConditionalIfAttribute;
            bool condition = EvaluateCondition(property, attribute);
            bool isVisible = IsVisible(condition, attribute);
            bool isEnabled = IsEnabled(condition, attribute);
            
            if (isVisible)
            {
                EditorGUI.BeginDisabledGroup(!isEnabled);
                EditorGUI.PropertyField(position, property, label, true);
                EditorGUI.EndDisabledGroup();
            }
        }
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var propertyField = new PropertyField(property);
            propertyField.name = "conditional-if-wrapper";
            
            // Define property field update for editor update events
            void UpdatePropertyField()
            {
                if (property == null) return;
                if (property.serializedObject == null) return;
                if (property.serializedObject.targetObject == null) return;
                
                var attribute = this.attribute as ConditionalIfAttribute;
                bool condition = EvaluateCondition(property, attribute);
                bool isVisible = IsVisible(condition, attribute);
                bool isEnabled = IsEnabled(condition, attribute);
                
                propertyField.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
                propertyField.SetEnabled(isEnabled);
            }
            
            // Subscribe to inspector start and input update events
            propertyField.schedule.Execute(() => UpdatePropertyField());
            propertyField.TrackSerializedObjectValue(property.serializedObject, _ => UpdatePropertyField());
            
            return propertyField;
        }
        
        
        // PRIVATE STATIC MEMBERS
        private static bool IsVisible(bool condition, ConditionalIfAttribute attribute)
            => attribute is ShowIfAttribute ? condition
                : attribute is HideIfAttribute ? !condition
                : true;
        
        private static bool IsEnabled(bool condition, ConditionalIfAttribute attribute)
            => attribute is EnableIfAttribute ? condition
                : attribute is DisableIfAttribute ? !condition
                : true;
        
        
        private static string GetRelativePath(SerializedProperty property)
        {
            string propertyPath = property.propertyPath;
            int lastDotIndex = propertyPath.LastIndexOf('.');
            
            return propertyPath.LastIndexOf('.') > 0
                ? propertyPath[..lastDotIndex]
                : string.Empty;
        }
        
        private static bool EvaluateCondition(SerializedProperty property, ConditionalIfAttribute attribute)
        {
            object targetObject = property.serializedObject.targetObject;
            string relativePath = GetRelativePath(property);
            
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
            object memberValue = MemberAccessCache.GetValue(targetObject, memberPath).Value;
            
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
            object memberValue = MemberAccessCache.GetValue(targetObject, memberPath).Value;
            
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
