#if UNITY_EDITOR
namespace AstrotypeInspector
{
    using UnityEngine;
    using UnityEditor;
    
    public static class WarningGUI
    {
        /// <summary>
        /// Adds a warning icon to the right of field.
        /// </summary>
        /// <param name="position">Position of the property field.</param>
        /// <param name="tooltip">Tooltip to display when mouse is over the warning icon.</param>
        /// <returns>
        /// Position to use for EditorGUI.PropertyField() where width is reduced
        /// to make space for the warning icon.
        /// </returns>
        public static Rect WarningField(Rect position, string tooltip)
        {
            // Main settings
            const float PADDING = 2f;
            float warningWidth = EditorGUIUtility.singleLineHeight;
            
            // Create rect for warning icon
            Rect warningRect = position;
            warningRect.x += position.width - warningWidth;
            warningRect.width = warningWidth;
            
            // Get warning icon and set tooltip
            GUIContent warningIcon = EditorGUIUtility.IconContent("console.warnicon");
            // GUIContent warningIcon = EditorGUIUtility.IconContent("warning");
            warningIcon.tooltip = tooltip;
            
            // Draw warning icon with tooltip
            EditorGUI.LabelField(warningRect, warningIcon);
            
            // Reduce position width
            position.width -= warningWidth + PADDING;
            
            // Return position to be used for PropertyField
            return position;
        }
    }
}
#endif