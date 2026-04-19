using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;

namespace AstrotypeInspector.Editor
{
    using Editor = UnityEditor.Editor;
    
    /// <summary>
    /// The base custom editor for AstrotypeInspector.<br/>
    /// <list type="bullet">
    ///     <item> Apply decorative attributes to class and struct declaration. </item>
    ///     <item> Apply button attribute to method declaration. </item>
    ///     <item> Display non-serializable fields, properties, static </item>
    ///     <item> Support grouping attributes with custom layout system. </item>
    /// </list>
    /// </summary>
    public abstract class AstrotypeEditor : Editor
    {
        // EDITOR METHODS
        public override void OnInspectorGUI()
        {
            DrawAstrotypeInspector();
        }
        
        public override VisualElement CreateInspectorGUI()
        {
            var settingsData = AstrotypeInspectorSettingsData.instance;
            
            // Create inspector using IMGUI or UI Toolkit
            if (settingsData.UseIMGUIDefaultInspector)
            {
                return settingsData.UseIMGUIContainer
                    ? CreateInspectorIMGUIContainer() // Wrap OnInspectorGUI() inside IMGUI Container and manually apply padding
                    : null; // Let Unity call OnInspectorGUI() and add inspector padding
            }
            else
            {
                return CreateAstrotypeInspector();
            }
        }
        
        
        // ASTROTYPE INSPECTOR IMGUI
        /// <summary> Draws the default AstrotypeInspector in IMGUI. </summary>
        protected bool DrawAstrotypeInspector()
        {
            // Debug.Log($"[IMGUI] AstrotypeEditor.DrawAstrotypeInspector()");
            var settingsData = AstrotypeInspectorSettingsData.instance;
            
            bool hasGUIChanges;
            
            // Create scope for localization support
            using (new LocalizationGroup(target))
            {
                // Set wide mode
                bool wideMode = EditorGUIUtility.wideMode; // wideModeMinWidth = 330
                EditorGUIUtility.wideMode = EditorGUIUtility.currentViewWidth > settingsData.WideModeMinWidth;
                
                // Draw serialized properties and audio meter (for scripts with audio processing)
                hasGUIChanges = DrawSerializedProperties();
                DrawAudioFilterGUIInternal();
                
                // Reset wide mode
                EditorGUIUtility.wideMode = wideMode;
            }
            
            return hasGUIChanges;
        }
        
        /// <summary> Iterate through each properties and draw them in the inspector in IMGUI. </summary>
        private bool DrawSerializedProperties()
        {
            EditorGUI.BeginChangeCheck();
            serializedObject.UpdateIfRequiredOrScript();
            
            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                bool isScriptField = iterator.propertyPath == "m_Script";
                bool hideScriptField = AstrotypeInspectorSettingsData.instance.HideScriptField;
                
                if (!isScriptField)
                {
                    EditorGUILayout.PropertyField(iterator, true);
                }
                else if (!hideScriptField)
                {
                    using (new EditorGUI.DisabledScope(isScriptField))
                    {
                        EditorGUILayout.PropertyField(iterator, true);
                    }
                }
                
                enterChildren = false;
            }
            
            serializedObject.ApplyModifiedProperties();
            return EditorGUI.EndChangeCheck();
        }
        
        /// <summary> Access internal classes to draw audio meter for scripts with audio processing in IMGUI. </summary>
        private void DrawAudioFilterGUIInternal()
        {
            // bool hasAudioCallback = AudioUtil.HasAudioCallback(monoBehaviour);
            // bool customFilterChannelCount = AudioUtil.GetCustomFilterChannelCount(monoBehaviour) <= 0;
            
            // MonoBehaviour monoBehaviour = target as MonoBehaviour;
            // if (monoBehaviour == null || !hasAudioCallback || customFilterChannelCount <= 0)
            // {
            //     return hasGUIChanges;
            // }
            
            // if (base.m_AudioFilterGUI)
            // {
            //     m_AudioFilterGUI = new AudioFIlterGUI();
            // }
            // m_AudioFilterGUI.DrawAudioFilterGUI(monoBehaviour);
        }
        
        
        // ASTROTYPE INSPECTOR UI TOOLKIT
        /// <summary> Creates the default AstrotypeInspector in UI Toolkit. </summary>
        protected VisualElement CreateAstrotypeInspector()
        {
            // Debug.Log($"[UI Toolkit] AstrotypeEditor.CreateAstrotypeInspector()");
            var settingsData = AstrotypeInspectorSettingsData.instance;
            
            // List of properties to exclude
            List<string> propertiesToExclude = new();
            if (settingsData.HideScriptField)
                propertiesToExclude.Add("m_Script");
            
            // Fill default inspector
            var editor = new VisualElement();
            InspectorElement.FillDefaultInspector(editor, serializedObject, this, propertiesToExclude.ToArray());
            
            return editor;
        }
        
        /// <summary> Draws IMGUI inspector inside IMGUIContainer in UI Toolkit. </summary>
        private IMGUIContainer CreateInspectorIMGUIContainer()
        {
            var editor = new IMGUIContainer(() =>
            {
                var settings = AstrotypeInspectorSettingsData.instance;
                
                // Enables auto-adjusting label width and better foldouts.
                bool hierarchyMode = EditorGUIUtility.hierarchyMode;
                EditorGUIUtility.hierarchyMode = true;
                
                // Wrap input fields for Vector2, Vector3, etc. when current inspector window width reaches below threshold.
                bool wideMode = EditorGUIUtility.wideMode; // threshold = 330
                EditorGUIUtility.wideMode = EditorGUIUtility.currentViewWidth >= settings.WideModeMinWidth;
                
                
                // Start padding
                EditorGUILayout.BeginVertical();
                GUILayout.Space(settings.PaddingTop); // top padding = 4
                GUILayout.BeginHorizontal();
                GUILayout.Space(settings.PaddingLeft); // left padding = 18
                EditorGUILayout.BeginVertical();
                
                // Draw inspector
                OnInspectorGUI();
                
                // End padding
                EditorGUILayout.EndVertical();
                GUILayout.Space(settings.PaddingRight); // right padding = 4
                GUILayout.EndHorizontal();
                GUILayout.Space(settings.PaddingBottom); // bottom padding = 2
                EditorGUILayout.EndVertical();
                
                
                // Restore modes
                EditorGUIUtility.hierarchyMode = hierarchyMode;
                EditorGUIUtility.wideMode = wideMode;
            });
            
            editor.RegisterCallback<AttachToPanelEvent>(_ =>
            {
                var inspectorElement = editor.parent;
                
                inspectorElement.RemoveFromClassList("unity-inspector-element--uie-custom");
                inspectorElement.RemoveFromClassList("unity-inspector-element--uie");
                
                inspectorElement.AddToClassList("unity-inspector-element--imgui-custom");
                inspectorElement.AddToClassList("unity-inspector-element--imgui");
            });
            
            return editor;
        }
        
        
    }
    
    
    #if !DISABLE_ASTROTYPE_INSPECTOR
    /// <summary> Overrides the default MonoBehaviour fallback editor to use AstrotypeInspector. </summary>
    [CustomEditor(typeof(MonoBehaviour), editorForChildClasses: true, isFallback = true)]
    public class MonoBehaviourEditor : AstrotypeEditor { }
    
    /// <summary> Overrides the default ScriptableObject fallback editor to use AstrotypeInspector. </summary>
    [CustomEditor(typeof(ScriptableObject), editorForChildClasses: true, isFallback = true)]
    public class ScriptableObjectEditor : AstrotypeEditor { }
    
    #endif
    
    
}
