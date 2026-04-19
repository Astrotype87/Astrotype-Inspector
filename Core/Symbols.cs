using UnityEngine;

namespace AstrotypeInspector
{
    /// <summary>
    /// List of string-exact scripting define symbols to use in Conditional attribute for attribute classes.
    /// Made to defend against string typos.
    /// </summary>
    public static class Symbols
    {
        /// <summary> Use this to exclude attribute usages in compiled build. </summary>
        public const string UNITY_EDITOR = nameof(UNITY_EDITOR);
        
        /// <summary> Use this to optionally include attribute usages in compiled build where UNITY_EDITOR is applied. </summary>
        public const string INCLUDE_IN_BUILD = nameof(INCLUDE_IN_BUILD);
    }
}
