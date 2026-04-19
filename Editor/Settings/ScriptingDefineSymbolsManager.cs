using System.Linq;
using UnityEditor;
using UnityEditor.Build;

namespace AstrotypeInspector.Editor
{
    public class ScriptingDefineSymbolsManager
    {
        /// <summary> Checks if a scripting define symbol is added to the list. </summary>
        public static bool IsEnabled(string symbol)
        {
            var buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);
            var defines = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);
            
            return defines.Split(';').Contains(symbol);
        }
        
        /// <summary> Adds or removes the specified scripting define symbol in the list. </summary>
        public static void SetEnabled(string symbol, bool enabled)
        {
            var buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);
            var defines = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget)
                .Split(';').ToList();
            
            if (enabled)
            {
                if (!defines.Contains(symbol))
                    defines.Add(symbol);
            }
            else
            {
                defines.Remove(symbol);
            }
            
            PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, string.Join(",", defines));
        }
    }
}
