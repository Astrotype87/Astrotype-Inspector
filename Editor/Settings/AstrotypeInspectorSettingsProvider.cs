using UnityEditor;
using UnityEngine;

namespace AstrotypeInspector.Editor
{
    /// <summary>
    /// Display AstrotypeInspector settings in the Project Settings window.
    /// </summary>
    public class AstrotypeInspectorSettingsProvider
    {
        private static readonly RectOffset padding = new(7, 0, 0, 0);
        private const float labelWidth = 250f;
        
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            SettingsProvider provider = new("Project/Astrotype/Astrotype Inspector", SettingsScope.Project);
            // provider.label = "Astrotype Inspector";
            provider.guiHandler = searchContext =>
            {
                // Begin padding
                EditorGUILayout.BeginVertical();
                GUILayout.Space(padding.top); // top padding = 4
                GUILayout.BeginHorizontal();
                GUILayout.Space(padding.left); // left padding = 18
                EditorGUILayout.BeginVertical();
                
                
                // Get AstrotypeInspector settings data
                var settingsData = AstrotypeInspectorSettingsData.instance;
                
                // Change label with and indent
                float originalLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = labelWidth;
                
                // Draw help box
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox(
                    "Disabling Astrotype Inspector allows custom editors from other similar libraries to work normally.\n" +
                    "Majority of property drawer-based attributes still works normally.\n" +
                    "More advanced attributes such as Button for methods and grouping attributes will not work when Astrotype Inspector is disabled.",
                    MessageType.Info);
                
                // Draw toggle for DisableAstrotypeEditor
                GUILayout.BeginHorizontal();
                var toggleFieldWidth = GUILayout.Width(EditorGUIUtility.labelWidth + 15f);
                
                GUIContent DisableAstrotypeEditor_Label = new("Disable Astrotype Inspector", "Disable override for default inspectors to use AstrotypeEditor.");
                settingsData.DisableAstrotypeEditor = EditorGUILayout.Toggle(DisableAstrotypeEditor_Label, settingsData.DisableAstrotypeEditor, toggleFieldWidth);
                
                EditorGUI.BeginDisabledGroup(true);
                GUILayout.Label("(Triggers Script Compilation)");
                EditorGUI.EndDisabledGroup();
                
                GUILayout.EndHorizontal();
                
                
                // Draw toggle for InspectorUIMode
                // GUIContent InspectorUIMode_Label = new("Inspector UI Mode", "Select from IMGUI and UI Toolkit inspector UI mode.");
                // settingsData.InspectorUIMode = (UIMode)EditorGUILayout.EnumPopup(InspectorUIMode_Label, settingsData.InspectorUIMode);
                
                
                // Draw toggle for UseIMGUIDefaultInspector
                GUILayout.BeginHorizontal();
                
                GUIContent UseIMGUIDefaultInspector = new("Use IMGUI Default Inspector", "Wrap IMGUI inspector inside controlled IMGUI Container to allow more IMGUI settings.");
                settingsData.UseIMGUIDefaultInspector = EditorGUILayout.Toggle(UseIMGUIDefaultInspector, settingsData.UseIMGUIDefaultInspector, toggleFieldWidth);
                
                EditorGUI.BeginDisabledGroup(true);
                GUILayout.Label("(Project Settings > Editor > Inspector > Use IMGUI Default Inspector)");
                EditorGUI.EndDisabledGroup();
                
                GUILayout.EndHorizontal();
                
                
                // Draw toggle for UseIMGUIContainer
                GUIContent UseIMGUIContainer_Label = new("Use IMGUI Container", "Wrap IMGUI inspector inside controlled IMGUI Container to allow more IMGUI settings.");
                settingsData.UseIMGUIContainer = EditorGUILayout.Toggle(UseIMGUIContainer_Label, settingsData.UseIMGUIContainer);
                
                // Draw toggle for HideScriptField
                GUIContent HideScriptField_Label = new("Hide Script Field", "Hide the script field for MonoBehaviour scripts.");
                settingsData.HideScriptField = EditorGUILayout.Toggle(HideScriptField_Label, settingsData.HideScriptField);
                
                
                // Draw settings for inspectorIMGUIPadding
                GUIContent paddingLeft_Label = new("Padding Left", "Set IMGUI inspector left padding.");
                settingsData.PaddingLeft = EditorGUILayout.FloatField(paddingLeft_Label, settingsData.PaddingLeft);
                
                GUIContent paddingRight_Label = new("Padding Right", "Set IMGUI inspector right padding.");
                settingsData.PaddingRight = EditorGUILayout.FloatField(paddingRight_Label, settingsData.PaddingRight);
                
                GUIContent paddingTop_Label = new("Padding Top", "Set IMGUI inspector top padding.");
                settingsData.PaddingTop = EditorGUILayout.FloatField(paddingTop_Label, settingsData.PaddingTop);
                
                GUIContent paddingBottom_Label = new("Padding Bottom", "Set IMGUI inspector bottom padding.");
                settingsData.PaddingBottom = EditorGUILayout.FloatField(paddingBottom_Label, settingsData.PaddingBottom);
                
                
                // Draw toggle for WideModeMinWidth
                GUIContent WideModeMinWidth_Label = new("Wide Mode Threshold Width (IMGUI only)",
                    "The inspector window minimum width before label and input fields for Vector2, Vector3, etc. are separated into two lines.");
                settingsData.WideModeMinWidth = EditorGUILayout.FloatField(WideModeMinWidth_Label, settingsData.WideModeMinWidth);
                
                // Draw toggle for FieldWidth
                // GUIContent FieldWidth_Label = new("Field Width (IMGUI only)", "Set the field width of GUI controls.");
                // settingsData.FieldWidth = EditorGUILayout.FloatField(FieldWidth_Label, settingsData.FieldWidth);
                
                
                // End padding
                EditorGUILayout.EndVertical();
                GUILayout.Space(padding.right); // right padding = 4
                GUILayout.EndHorizontal();
                GUILayout.Space(padding.bottom); // bottom padding = 2
                EditorGUILayout.EndVertical();
            };
            
            return provider;
        }
        
        
    }
}
