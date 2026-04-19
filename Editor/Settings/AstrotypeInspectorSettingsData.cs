using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace AstrotypeInspector.Editor
{
    public enum UIMode { IMGUI, UIToolkit }
    
    /// <summary>
    /// Manage AstrotypeInspector settings saved inside (ProjectFolder)/ProjectSettings/Astrotype folder.
    /// </summary>
    [FilePath("ProjectSettings/Astrotype/AstrotypeInspectorSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    public class AstrotypeInspectorSettingsData : ScriptableSingleton<AstrotypeInspectorSettingsData>
    {
        // FIELDS
        [SerializeField] private UIMode inspectorUIMode = UIMode.UIToolkit;
        [SerializeField] private bool useIMGUIContainer = false;
        
        [SerializeField] private bool hideScriptField = false;
        [SerializeField] private float paddingLeft;
        [SerializeField] private float paddingRight;
        [SerializeField] private float paddingTop;
        [SerializeField] private float paddingBottom;
        
        [SerializeField] private float wideModeMinWidth = 330f;
        
        
        // CONSTANTS
        public const string DISABLE_ASTROTYPE_INSPECTOR = nameof(DISABLE_ASTROTYPE_INSPECTOR);
        
        
        // PROPERTIES
        /// <summary> Adds or removes <c>DISABLE_ASTROTYPE_INSPECTOR</c> to scripting define symbols. </summary>
        public bool DisableAstrotypeEditor
        {
            get => ScriptingDefineSymbolsManager.IsEnabled(DISABLE_ASTROTYPE_INSPECTOR);
            set
            {
                if (ScriptingDefineSymbolsManager.IsEnabled(DISABLE_ASTROTYPE_INSPECTOR) == value) return;
                ScriptingDefineSymbolsManager.SetEnabled(DISABLE_ASTROTYPE_INSPECTOR, value);
                SaveAndRecompile();
            }
        }
        
        public bool UseIMGUIDefaultInspector
        {
            get => GetInspectorUseIMGUIDefaultInspector();
            set
            {
                if (GetInspectorUseIMGUIDefaultInspector() == value) return;
                
                SetInspectorUseIMGUIDefaultInspector(value);
                ActiveEditorTracker.sharedTracker.ForceRebuild();
            }
        }
        
        
        /// <summary> Switch between IMGUI and UI Toolkit inspector UI mode. </summary>
        public UIMode InspectorUIMode
        {
            get => inspectorUIMode;
            set
            {
                if (inspectorUIMode == value) return;
                inspectorUIMode = value;
                SaveAndForceRebuildEditor();
            }
        }
        
        /// <summary> Wrap IMGUI inspector inside controlled IMGUI Container to allow more IMGUI settings. </summary>
        public bool UseIMGUIContainer
        {
            get => useIMGUIContainer;
            set
            {
                if (useIMGUIContainer == value) return;
                useIMGUIContainer = value;
                SaveAndForceRebuildEditor();
            }
        }
        
        
        /// <summary> Hides the <c>m_script</c> field across all MonoBehaviour inspectors. </summary>
        public bool HideScriptField
        {
            get => hideScriptField;
            set
            {
                if (hideScriptField == value) return;
                hideScriptField = value;
                SaveAndForceRebuildEditor();
            }
        }
        
        /// <summary> The left padding applied to inspectors. </summary>
        public float PaddingLeft
        {
            get => paddingLeft;
            set
            {
                if (paddingLeft == value) return;
                paddingLeft = value;
                SaveAndRepaintEditor();
            }
        }
        
        /// <summary> The right padding applied to inspectors. </summary>
        public float PaddingRight
        {
            get => paddingRight;
            set
            {
                if (paddingRight == value) return;
                paddingRight = value;
                SaveAndRepaintEditor();
            }
        }
        
        /// <summary> The top padding applied to inspectors. </summary>
        public float PaddingTop
        {
            get => paddingTop;
            set
            {
                if (paddingTop == value) return;
                paddingTop = value;
                SaveAndRepaintEditor();
            }
        }
        
        /// <summary> The bottom padding applied to inspectors. </summary>
        public float PaddingBottom
        {
            get => paddingBottom;
            set
            {
                if (paddingBottom == value) return;
                paddingBottom = value;
                SaveAndRepaintEditor();
            }
        }
        
        
        /// <summary>
        /// The inspector window minimum width before label and input fields for Vector2, Vector3, etc.
        /// are separated into two lines. (IMGUI only)
        /// </summary>
        public float WideModeMinWidth
        {
            get => wideModeMinWidth;
            set
            {
                if (wideModeMinWidth == value) return;
                wideModeMinWidth = value;
                SaveAndRepaintEditor();
            }
        }
        
        
        // PRIVATE METHODS
        /// <summary> Save settings and refresh editor to apply quick visual changes. </summary>
        private void SaveAndRepaintEditor()
        {
            Save(true);
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
        }
        
        /// <summary> Save settings and force rebuild editor to allow quick refresh between IMGUI and UI Toolkit. </summary>
        private void SaveAndForceRebuildEditor()
        {
            Save(true);
            ActiveEditorTracker.sharedTracker.ForceRebuild();
        }
        
        /// <summary> Save settings and request script recompilation to apply settings that deal with scripting backend. </summary>
        private void SaveAndRecompile()
        {
            Save(true);
            CompilationPipeline.RequestScriptCompilation();
        }
        
        
        private static bool GetInspectorUseIMGUIDefaultInspector()
        {
            var property = typeof(EditorSettings).GetProperty(
                "inspectorUseIMGUIDefaultInspector",
                BindingFlags.Static | BindingFlags.NonPublic);
            
            if (property != null)
                return (bool)property.GetValue(null);
            
            Debug.LogWarning("EditorSettings.inspectorUseIMGUIDefaultInspector not found via System.Reflection. Returning false.");
            return false;
        }
        
        private static void SetInspectorUseIMGUIDefaultInspector(bool value)
        {
            var property = typeof(EditorSettings).GetProperty(
                "inspectorUseIMGUIDefaultInspector",
                BindingFlags.Static | BindingFlags.NonPublic);
            
            if (property != null)
            {
                property.SetValue(null, value);
                return;
            }
            
            Debug.LogWarning("EditorSettings.inspectorUseIMGUIDefaultInspector not found via System.Reflection.");
        }
        
    }
}
