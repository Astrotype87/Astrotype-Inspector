using System;
using System.Diagnostics;
using UnityEngine;

namespace AstrotypeInspector
{
    [Conditional(Symbols.UNITY_EDITOR), Conditional(Symbols.INCLUDE_IN_BUILD)]
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public sealed class InfoBoxAttribute : DecorativeAttribute
    {
        public readonly string Message;
        public readonly InfoType InfoType;
        public readonly string ShowIf;
        public readonly float Height;
        
        public InfoBoxAttribute(string message, InfoType infoType = InfoType.Info, string showIf = default, float height = -1f)
        {
            Message = message;
            InfoType = infoType;
            ShowIf = showIf;
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
    
    [CustomPropertyDrawer(typeof(InfoBoxAttribute))]
    public class InfoBoxDrawer : PropertyDrawer
    {
        private const float IMGUI_HelpBoxHeight = 18f * 2f + 2f;
        private const float IMGUI_HelpBoxHeightNoIcon = 18f;
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var attribute = this.attribute as InfoBoxAttribute;
            MessageType messageType = GetMessageType(attribute.InfoType);
            bool showHelpBox = string.IsNullOrWhiteSpace(attribute.ShowIf) || EvaluateCondition(property, attribute.ShowIf);
            
            float helpBoxHeight = showHelpBox
                ? attribute.Height >= 0
                    ? attribute.Height
                    : messageType == MessageType.None
                        ? IMGUI_HelpBoxHeightNoIcon
                        : IMGUI_HelpBoxHeight
                : 0;
            float spacing = showHelpBox ? EditorGUIUtility.standardVerticalSpacing : 0f;
            
            return helpBoxHeight + spacing + EditorGUI.GetPropertyHeight(property, label, true);
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attribute = this.attribute as InfoBoxAttribute;
            MessageType messageType = GetMessageType(attribute.InfoType);
            bool showHelpBox = string.IsNullOrEmpty(attribute.ShowIf) || EvaluateCondition(property, attribute.ShowIf);
            
            float offsetIfBottom = attribute.bottom
                ? EditorGUI.GetPropertyHeight(property) + EditorGUIUtility.standardVerticalSpacing
                : 0;
            float helpBoxHeight = showHelpBox
                ? attribute.Height >= 0
                    ? attribute.Height
                    : messageType == MessageType.None
                        ? IMGUI_HelpBoxHeightNoIcon
                        : IMGUI_HelpBoxHeight
                : 0;
            float spacing = showHelpBox ? EditorGUIUtility.standardVerticalSpacing : 0f;
            
            // Draw help box
            if (showHelpBox)
            {
                Rect helpBoxRect = EditorGUI.IndentedRect(position);
                helpBoxRect.y += offsetIfBottom;
                helpBoxRect.height = helpBoxHeight;
                EditorGUI.HelpBox(helpBoxRect, attribute.Message, messageType);
            }
            
            // Draw property field
            Rect propertyRect = position;
            if (showHelpBox && !attribute.bottom)
                propertyRect.y += helpBoxHeight + spacing;
            
            EditorGUI.PropertyField(propertyRect, property, label, true);
        }
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var attribute = this.attribute as InfoBoxAttribute;
            HelpBoxMessageType messageType = GetHelpBoxMessageType(attribute.InfoType);
            
            // Create help box
            var helpBox = new HelpBox(attribute.Message, messageType);
            helpBox.style.minHeight = attribute.Height;
            helpBox.style.marginLeft = 0f;
            helpBox.style.marginRight = 0f;
            
            if (attribute.Height >= 0)
            {
                helpBox.RegisterCallback<AttachToPanelEvent>(_ =>
                {
                    var helpBoxIcon = helpBox.Q<VisualElement>(className: "unity-help-box__icon");
                    if (helpBoxIcon == null) return;
                    
                    // Reduce icon height by paddingTop, paddingBottom, borderTop and borderBottom of help box
                    float iconHeight = attribute.Height - 2f - 2f - 1f - 1f;
                    
                    // Change help box icon size
                    helpBoxIcon.style.minWidth = iconHeight + 2f; // width is 2f higher than height in unity help box icon
                    helpBoxIcon.style.maxWidth = iconHeight + 2f;
                    
                    helpBoxIcon.style.minHeight = iconHeight;
                    helpBoxIcon.style.maxHeight = iconHeight;
                });
            }
            
            if (messageType == HelpBoxMessageType.None)
                helpBox.style.paddingLeft = 3f;
            
            // Create property field
            var propertyField = new PropertyField(property);
            propertyField.schedule.Execute(() =>
            {
                // Unwrap property field wrapper
                propertyField.UnwrapElement(out var parent);
                
                // Add decorative elements inside top/bottom decorator drawers container
                if (attribute.bottom)
                    parent.AddToBottomDecoratorContainer(helpBox);
                else
                    parent.AddToDecoratorContainer(helpBox);
            });
            
            // Define property field update for editor update events
            void UpdatePropertyField()
            {
                bool showHelpBox = string.IsNullOrEmpty(attribute.ShowIf) || EvaluateCondition(property, attribute.ShowIf);
                helpBox.style.display = showHelpBox ? DisplayStyle.Flex : DisplayStyle.None;
            }
            
            // Subscribe to inspector start and input update events
            propertyField.schedule.Execute(() => UpdatePropertyField());
            propertyField.TrackSerializedObjectValue(property.serializedObject, _ => UpdatePropertyField());
            
            return propertyField;
        }
        
        
        private static bool EvaluateCondition(SerializedProperty property, string expression)
        {
            // Get the target object of this property
            object targetObject = property.serializedObject.targetObject;
            
            // Get the parent path prefix
            string propertyPath = property.propertyPath;
            int lastDotIndex = propertyPath.LastIndexOf('.');
            string parentPath = propertyPath.LastIndexOf('.') > 0
                ? propertyPath[..lastDotIndex]
                : string.Empty;
            
            // Evaluate expression
            return EvaluateBoolExpression(targetObject, parentPath, expression);
        }
        
        private static bool EvaluateBoolExpression(object targetObject, string parentPath, string boolExpression)
        {
            // HACK: Temporary placeholder before ConditionalExpressionParser is implemented
            string memberPath = string.IsNullOrWhiteSpace(parentPath)
                ? boolExpression : $"{parentPath}.{boolExpression}";
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
        
        
        private static MessageType GetMessageType(InfoType infoType)
        {
            return infoType switch
            {
                InfoType.None => MessageType.None,
                InfoType.Info => MessageType.Info,
                InfoType.Warning => MessageType.Warning,
                InfoType.Error => MessageType.Error,
                _ => MessageType.None
            };
        }
        
        private static HelpBoxMessageType GetHelpBoxMessageType(InfoType infoType)
        {
            return infoType switch
            {
                InfoType.None => HelpBoxMessageType.None,
                InfoType.Info => HelpBoxMessageType.Info,
                InfoType.Warning => HelpBoxMessageType.Warning,
                InfoType.Error => HelpBoxMessageType.Error,
                _ => HelpBoxMessageType.None
            };
        }
        
        
    }
}
#endif
