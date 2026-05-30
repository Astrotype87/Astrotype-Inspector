using UnityEngine;

namespace AstrotypeInspector
{
    /// <summary> Rules for evaluating a group of boolean conditions. </summary>
    public enum BoolGroup
    {
        /// <summary> All conditions are true. Equivalent to AND gate.</summary>
        All,
        /// <summary> At least one condition is true. Equivalent to OR gate.</summary>
        Any,
        /// <summary> Not all conditions are true. Equivalent to NAND gate.</summary>
        NotAll,
        /// <summary> No condition is true. Equivalent to NOR gate.</summary>
        None,
        /// <summary> The number of true conditions is odd. Equivalent to XOR gate.</summary>
        Odd,
        /// <summary> The number of true conditions is even. Equivalent to XNOR gate.</summary>
        Even,
        /// <summary> True unless all preceding conditions are true and the last is false. Equivalent to IMPLY gate.</summary>
        Imply,
        /// <summary> True only when all preceding conditions are true and the last is false. Equivalent to NIMPLY gate.</summary>
        NotImply,
        /// <summary> All conditions are the same. Equivalent to EQ gate.</summary>
        Same,
        /// <summary> Not all conditions are the same. Equivalent to NEQ gate.</summary>
        Different,
    }
    
    /// <summary> Determines if the editor is in Edit or Play mode, and if it is running or unpaused. </summary>
    public enum InEditor
    {
        /// <summary> Any editor and pause state. </summary>
        Any,
        /// <summary> The editor is not in Play mode. </summary>
        EditMode,
        /// <summary> The editor is in Play mode. </summary>
        PlayMode,
        /// <summary> The editor is not paused in Edit or Play mode. </summary>
        Running,
        /// <summary> The editor is paused in Edit or Play mode. </summary>
        Paused,
        /// <summary> The editor is not paused in Edit mode. </summary>
        EditRunning,
        /// <summary> The editor is paused in Edit mode. </summary>
        EditPaused,
        /// <summary> The editor is not paused in Play mode. </summary>
        PlayRunning,
        /// <summary> The editor is paused in Play mode. </summary>
        PlayPaused,
    }
    
    /// <summary>
    /// The type of asset which the serialized field is located and saved at.<br/>
    /// This setting is most effective with serializable class and struct type fields that can be used anywhere.
    /// </summary>
    public enum InAsset
    {
        /// <summary> The serialized field is located from any asset. </summary>
        Any,
        /// <summary> The serialized field is located inside the scene asset. </summary>
        Scene,
        /// <summary> The serialized field is located inside the prefab asset. </summary>
        Prefab,
        /// <summary> The serialized field is inside a ScriptableObject. </summary>
        ScriptableObject,
    }
    
    /// <summary> The current prefab context of a component's GameObject. </summary>
    public enum InPrefab
    {
        /// <summary> The prefab context is not checked. </summary>
        Any,
        /// <summary> The serialized field is not inside a prefab. </summary>
        NotPrefab,
        /// <summary> The serialized field is inside a Prefab Asset or Prefab instance. </summary>
        Prefab,
        /// <summary> The serialized field is inside a Prefab Asset. </summary>
        Asset,
        /// <summary> The serialized field is inside a Prefab Instance in the scene. </summary>
        Instance,
        /// <summary> The serialized field is inside a regular/base Prefab Asset or Prefab instance. </summary>
        Regular,
        /// <summary> The serialized field is inside a variant Prefab Variant Asset or Prefab Variant instance. </summary>
        Variant,
        /// <summary> The serialized field is inside an outermost Prefab instance root. </summary>
        Root,
        /// <summary> The serialized field is inside a child an outermost Prefab instance root. </summary>
        Child,
        /// <summary> The serialized field is inside a Model Prefab Asset or Model Prefab instance. </summary>
        Model,
        /// <summary> The serialized field is inside a Prefab Asset or Prefab instance currently in edit mode. </summary>
        EditMode,
    }
    
    
    /// <summary> Font style settings for Title attribute. </summary>
    public enum TitleStyle
    {
        Default,
        DefaultItalic,
        Normal,
        Bold,
        Italic,
        BoldAndItalic
    }
    
    /// <summary> Text alignment settings for Title, Text, and Label attribute. </summary>
    public enum Align
    {
        Left, Right, Center
    }
    
    /// <summary> Position settings for Button attribute. </summary>
    public enum Position
    {
        Bottom, Top, Right
    }
    
    
    /// <summary> Set icon for InfoType attribute. </summary>
    public enum InfoType
    {
        None,
        Info,
        Warning,
        Error
    }
    
    
}
