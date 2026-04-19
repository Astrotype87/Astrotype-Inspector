#if !DISABLE_ASTROTYPE_INSPECTOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace AstrotypeInspector.Editor
{
    /// <summary>
    /// Overrides the the default fallback property drawer for all serializable fields.<br/>
    /// Extends support for advanced AstrotypeInspector features for class and struct declarations:
    /// <list type="bullet">
    ///     <item> Apply decorative attributes to class and struct declaration. </item>
    ///     <item> Apply button attribute to method declaration. </item>
    ///     <item> Display non-serializable fields, properties, static </item>
    ///     <item> Support grouping attributes with custom layout system. </item>
    /// </list>
    /// </summary>
    [CustomPropertyDrawer(typeof(object), useForChildren: true)]
    public class AstrotypePropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, includeChildren: true);
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Debug.Log($"[IMGUI] AstrotypePropertyDrawer.OnGUI() => {property.propertyPath}");
            EditorGUI.PropertyField(position, property, label, includeChildren: true);
        }
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            // Debug.Log($"[UI Toolkit] AstrotypePropertyDrawer.CreatePropertyGUI() => {property.propertyPath}");
            PropertyField propertyField = new(property);
            propertyField.schedule.Execute(propertyField.UnwrapElement);
            return propertyField;
        }
    }
}
#endif
