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
        // UNITY PROPERTY ATTRIBUTES (Priority of 0)    - Range, Multiline, etc...
        // POST PROPERTY ATTRIBUTES (Priority of -100)  - Slider
        // DECORATIVE ATTRIBUTES (Priority of -200)     - Title, Separator, InfoBox, Required/NotDefault/Validate (InfoBox variants)
        // WRAPPER ATTRIBUTES (Priority of -200)        - (Show/Hide/Enable/Disable)(If/In), ReadOnly, Indent/FixedIndent
        // GROUP ATTRIBUTES (Priority of -200)          - FoldoutGroup/BoxGroup
        // SCOPE ATTRIBUTES (Priority of -200) same as GROUP ATTRIBUTE - FoldoutScope/BoxScope, EndScope
    }
    
    /// <summary>
    /// Attributes that applies after normal property attributes
    /// </summary>
    public abstract class PostPropertyAttribute : PropertyAttribute
    {
        public new int order
        {
            get => base.order - AttributePriority.PostPropertyAttribute;
            set => base.order = AttributePriority.PostPropertyAttribute + value;
        }
        
        protected PostPropertyAttribute() : base(applyToCollection: false)
            => order = 0;
        
        protected PostPropertyAttribute(bool applyToCollection) : base(applyToCollection)
            => order = 0;
    }
    
    /// <summary>
    /// Attributes that add elements above or below the property drawer,
    /// contained inside top or bottom decorator containers.
    /// </summary>
    public abstract class DecorativeAttribute : PropertyAttribute
    {
        public new int order
        {
            get => base.order - AttributePriority.DecorativeAttribute;
            set => base.order = AttributePriority.DecorativeAttribute + value;
        }
        
        /// <summary>
        /// Consecutive decorative attributes with <c>bottom = true</c> option
        /// are drawn in inspector by reverse/bottom-up order.
        /// </summary>
        public bool bottom { get; set; } = false;
        
        protected DecorativeAttribute() : base(applyToCollection: false)
            => order = 0;
        
        protected DecorativeAttribute(bool applyToCollection) : base(applyToCollection)
            => order = 0;
    }
    
    /// <summary>
    /// Attributes that wraps the property field and creates own decorator containers
    /// if succeeded by a decorative attribute.
    /// </summary>
    public abstract class WrapperAttribute : PropertyAttribute
    {
        public new int order
        {
            get => base.order - AttributePriority.WrapperAttribute;
            set => base.order = AttributePriority.WrapperAttribute + value;
        }
        
        protected WrapperAttribute() : base(applyToCollection: false)
            => order = 0;
        
        protected WrapperAttribute(bool applyToCollection) : base(applyToCollection)
            => order = 0;
    }
    
    /// <summary>
    /// Attributes that inserts the property field, including its decorator containers,
    /// inside a group container.
    /// </summary>
    public abstract class GroupAttribute : PropertyAttribute
    {
        protected GroupAttribute() : base(applyToCollection: false)
            => order = 0;
        
        protected GroupAttribute(bool applyToCollection) : base(applyToCollection)
            => order = 0;
    }
    
    
}
