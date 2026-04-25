#if UNITY_EDITOR
namespace AstrotypeInspector.Editor
{
    using System;
    using UnityEngine.UIElements;
    
    /// <summary>
    /// Collection of IMGUI and UIToolkit helper functions.
    /// </summary>
    public static class AstrotypeEditorGUI
    {
        /// <summary>
        /// Moves all child elements of the element to its parent and removes the element from the hierarchy.
        /// </summary>
        /// <param name="element"> The visual element to unwrap. </param>
        /// <remarks>
        /// NOTE: Use this to unwrap PropertyField wrappers inside <c>propertyField.schedule.Execute()</c>.
        /// </remarks>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static void UnwrapElement(this VisualElement element)
        {
            UnwrapElement(element, out _);
        }
        
        /// <summary>
        /// Moves all child elements of the element to its parent,
        /// removes the element from the hierarchy, and outputs its parent.
        /// </summary>
        /// <param name="element"> The visual element to unwrap. </param>
        /// <param name="parent"> Outputs the parent of the visual element. </param>
        /// <remarks>
        /// NOTE: Use this to unwrap PropertyField wrappers inside <c>propertyField.schedule.Execute()</c>.
        /// </remarks>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static void UnwrapElement(this VisualElement element, out VisualElement parent)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            
            // Retrieve the parent of the wrapper
            parent = element.parent ?? throw new InvalidOperationException(
                "The element has no parent, either because the element is not attached to the panel yet, or is a root element.");
            
            // Move child elements outside the wrapper
            int insertIndex = parent.IndexOf(element);
            while (element.childCount > 0)
            {
                var child = element[0];
                parent.Insert(insertIndex, child);
                insertIndex++;
            }
            
            // Remove wrapper from parent hierarchy
            element.RemoveFromHierarchy();
        }
        
        /// <summary>
        /// Adds decorative elements to the top decorator container inside the parent.<br/>
        /// Automatically creates the decorator container if it does not exist,
        /// which is recognizable by Unity decorator drawers.<br/>
        /// </summary>
        /// <param name="parent">The parent element that may contain a decorator container.</param>
        /// <param name="decorativeElements">The decorative elements to add inside the decorator container.</param>
        /// <remarks>
        /// NOTE: Inside <c>propertyField.schedule.Execute()</c>, use <c>propertyField.UnwrapElement(out var parent)</c>
        /// to access the bottom decorator drawers container inside <c>out var parent</c>.
        /// </remarks>
        /// <exception cref="ArgumentNullException"></exception>
        public static void AddToDecoratorContainer(this VisualElement parent, params VisualElement[] decorativeElements)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));
            if (decorativeElements == null)
                throw new ArgumentNullException(nameof(decorativeElements));
            
            // Get existing decorator container (created by Unity, or by this method)
            var decoratorContainer = parent.Q(className: "unity-decorator-drawers-container");
            
            // If no decorator container is present, create new one and insert to top
            if (decoratorContainer == null)
            {
                decoratorContainer = new VisualElement();
                decoratorContainer.AddToClassList("unity-decorator-drawers-container");
                parent.Insert(0, decoratorContainer);
            }
            
            // Add decorative elements
            foreach (var element in decorativeElements)
            {
                if (element != null)
                    decoratorContainer.Add(element);
            }
        }
        
        /// <summary>
        /// Adds decorative elements to the bottom decorator container inside the parent.<br/>
        /// Automatically creates the bottom decorator container if it does not exist.<br/>
        /// </summary>
        /// <param name="parent">The parent element that may contain a bottom decorator container.</param>
        /// <param name="decorativeElements">The decorative elements to add inside the bottom decorator container.</param>
        /// <remarks>
        /// NOTE: Inside <c>propertyField.schedule.Execute()</c>, use <c>propertyField.UnwrapElement(out var parent)</c>
        /// to access the bottom decorator drawers container inside <c>out var parent</c>.
        /// </remarks>
        /// <exception cref="ArgumentNullException"></exception>
        public static void AddToBottomDecoratorContainer(this VisualElement parent, params VisualElement[] decorativeElements)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));
            if (decorativeElements == null)
                throw new ArgumentNullException(nameof(decorativeElements));
            
            // Get existing bottom decorator container (created by this method)
            var decoratorContainer = parent.Q(name: "bottom-decorator-drawers-container");
            
            // If no decorator container is present, create new one and add to bottom
            if (decoratorContainer == null)
            {
                decoratorContainer = new VisualElement();
                decoratorContainer.name = "bottom-decorator-drawers-container";
                decoratorContainer.AddToClassList("unity-decorator-drawers-container");
                parent.Add(decoratorContainer);
            }
            
            // Add decorative elements
            foreach (var element in decorativeElements)
            {
                if (element != null)
                    decoratorContainer.Add(element);
            }
        }
        
        /// <summary>
        /// Creates a container specifically for wrapping multiple decorative elements before being added to a decorator drawers container.<br/>
        /// The child-targeted styling of <c>unity-decorator-drawers-container</c> is preserved for the child elements of this container.
        /// </summary>
        /// <param name="name">The element name of the wrapper.</param>
        /// <returns>
        /// A visual element with <c>unity-decorator-drawers-container</c> added to class list,
        /// with <c>flexDirection = FlexDirection.Column</c> and <c>marginLeft = marginRight = none</c>.
        /// </returns>
        public static VisualElement CreateDecoratorWrapper(string name = default)
        {
            var wrapper = new VisualElement();
            if (!string.IsNullOrWhiteSpace(name))
                wrapper.name = name;
            
            // Used only to preserve child-targeted styles
            wrapper.AddToClassList("unity-decorator-drawers-container");
            wrapper.style.flexDirection = FlexDirection.Column;
            
            // Remove margins of unity-decorator-drawers-container, because we are already inside decorator container
            wrapper.style.marginLeft = StyleKeyword.None;
            wrapper.style.marginRight = StyleKeyword.None;
            
            return wrapper;
        }
        
    }
}
#endif
