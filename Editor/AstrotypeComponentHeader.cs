// using UnityEditor;
// using UnityEditorInternal;
// using UnityEngine;

// namespace AstrotypeInspector.Editor
// {
//     using Editor = UnityEditor.Editor;
    
//     [InitializeOnLoad]
//     public static class AstrotypeComponentHeader
//     {
//         static AstrotypeComponentHeader()
//         {
//             Editor.finishedDefaultHeaderGUI += FinishedDefaultHeaderGUI;
//             // ActiveEditorTracker.sharedTracker.activeEditors[0].UseDefaultMargins
            
//             // InternalEditorUtility.SetIsInspectorExpanded()
//         }
        
//         static void FinishedDefaultHeaderGUI(Editor editor)
//         {
//             Debug.Log($"Target name: {editor.target.name}, Target type: {editor.target.GetType().Name}");
//         }
//     }
// }
