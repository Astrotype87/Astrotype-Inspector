using UnityEngine;

namespace AstrotypeInspector
{
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
        
        protected PostPropertyAttribute() : base(applyToCollection: false) { }
        protected PostPropertyAttribute(bool applyToCollection) : base(applyToCollection) { }
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
        
        protected DecorativeAttribute() : base(applyToCollection: false) { }
        protected DecorativeAttribute(bool applyToCollection) : base(applyToCollection) { }
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
        
        protected WrapperAttribute() : base(applyToCollection: false) { }
        protected WrapperAttribute(bool applyToCollection) : base(applyToCollection) { }
    }
    
    /// <summary>
    /// Attributes that inserts the property field, including its decorator containers,
    /// inside a group container.
    /// </summary>
    public abstract class GroupAttribute : PropertyAttribute
    {
        protected GroupAttribute() : base(applyToCollection: false) { }
        protected GroupAttribute(bool applyToCollection) : base(applyToCollection) { }
    }
    
    
}
