using UnityEngine;

namespace AstrotypeInspector
{
    /// <summary> Global priority of inspector attributes. Lower values are applied last. </summary>
    public static class AttributePriority
    {
        public const int PostPropertyAttribute = -100;
        public const int DecorativeAttribute = -200;
        public const int WrapperAttribute = -200;
        public const int GroupAttribute = -200;
        
        // DIFFERENT TYPES OF ATTRIBUTES : based on order and priority
        // ----------------------------------------------------------------------
        // UNITY PROPERTY ATTRIBUTES (Priority of 0)
        // - Range, Multiline, etc...
        // 
        // POST PROPERTY ATTRIBUTES (Priority of -100)
        // - Slider, 
        // 
        // DECORATIVE ATTRIBUTES (Priority of -200)
        // - Title, Separator, InfoBox, Required/NotDefault/Validate (InfoBox variants)
        // 
        // WRAPPER ATTRIBUTES (Priority of -200)
        // - (Show/Hide/Enable/Disable)(If/In), ReadOnly, Indent/FixedIndent, 
        // 
        // GROUP ATTRIBUTES (Priority of -200)
        // - FoldoutGroup/BoxGroup
        // 
        // SCOPE ATTRIBUTES (Priority of -200) same as GROUP ATTRIBUTES
        // - FoldoutScope/BoxScope, EndScope
    }
}
