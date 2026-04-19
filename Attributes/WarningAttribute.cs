using System;
using UnityEngine;

namespace AstrotypeInspector
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public sealed class WarningAttribute : PropertyAttribute
    {
        public string Tooltip { get; }
        
        public WarningAttribute() { }
        
        public WarningAttribute(string tooltip)
        {
            Tooltip = tooltip;
        }
    }
}


#if UNITY_EDITOR
namespace AstrotypeInspector.Editor
{
    using UnityEditor;
    
    [CustomPropertyDrawer(typeof(WarningAttribute))]
    public class WarningDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // var attributes = fieldInfo.GetCustomAttributes<WarningAttribute>(true);
            // foreach (var attribute in attributes)
            // {
            //     Debug.Log($"{attribute.Message}");
            // }
            
            // Debug.Log($"- - - - - - - - - - - - - - - - - - - - - - - - -");
            
            // EditorGUI.PropertyField(position, property, label, true);
            
            
            // var attribute = (WarningAttribute)this.attribute;
            
            // GUIContent warningIcon = EditorGUIUtility.IconContent("warning");
            // warningIcon.tooltip = attribute.Tooltip;
            
            // float warningWidth = EditorGUIUtility.singleLineHeight;
            // float warningPadding = 2f;
            
            // Rect fieldRect = position;
            // fieldRect.width -= warningWidth + warningPadding;
            // EditorGUI.PropertyField(fieldRect, property, label, true);
            
            // Rect warningRect = position;
            // warningRect.x = position.width;
            // warningRect.width = warningWidth;
            // EditorGUI.LabelField(warningRect, warningIcon);
            
            var attribute = (WarningAttribute)this.attribute;
            
            position = WarningGUI.WarningField(position, attribute.Tooltip);
            EditorGUI.PropertyField(position, property, label, true);
        }
    }
}
#endif
