using UnityEngine;

namespace AstrotypeInspector
{
    public interface IGroupAttribute { }
    
    /// <summary>
    /// Marks this attribute as decorative if it's based on PropertyDrawer.
    /// For UI Toolkit-based inspectors, the decorative element from this attribute is
    /// added inside decorator drawers container on top or bottom of property field.
    /// </summary>
    public interface IDecorativeAttribute
    {
        /// <summary>
        /// Adds the decorative element below the property field,
        /// which will be contained inside bottom decorator drawers container.
        /// </summary>
        public bool Bottom { get; set; }
    }
}
