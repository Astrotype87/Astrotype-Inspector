using UnityEngine;

namespace AstrotypeInspector
{
    /// <summary> Global priority of inspector attributes. Lower values are applied last. </summary>
    public static class AttributePriority
    {
        // TODO: Reconsider all attribute priority/order
        // Decorative attributes should be the same priority as wrapper attributes and group attributes to support order
        // 
        
        public const int PostPropertyAttribute = -100;
        public const int DecorativeAttribute = -200;
        public const int WrapperAttribute = -200;
        public const int GroupAttribute = -200;
        
        
        // public const int ConditionalIf = -100;
        // public const int ConditionalIn = -200;
        // public const int ReadOnly = -300;
        
        
        // DIFFERENT TYPES OF ATTRIBUTES : based on order and priority
        // ----------------------------------------------------------------------
        // UNITY PROPERTY ATTRIBUTES (Priority of 0)
        // - Range, Multiline, etc...
        // 
        // DECORATIVE ATTRIBUTES (Priority of -100)
        // - Title, Separator, InfoBox, Required/NotDefault/Validate (InfoBox variants)
        // 
        // GROUP ATTRIBUTES (Priority of -100)
        // - FoldoutGroup/BoxGroup
        // 
        // SCOPE ATTRIBUTES (Priority of -100)
        // - FoldoutScope/BoxScope, EndScope
        // 
        // WRAPPER ATTRIBUTES (Priority of -100)
        // - (Show/Hide/Enable/Disable)(If/In), ReadOnly, Indent/FixedIndent, 
        // 
        // CONTROL ATTRIBUTES (Priority of 0)
        // - Slider, 
        // 
        
        
        
    }
}
