using UnityEngine;

namespace AstrotypeInspector.Demo
{
    public class ConditionalInDemo1 : MonoBehaviour
    {
        // EDITOR STATE
        [Header("Editor State")]
        [EnableIn(InEditor.EditMode)] public string editMode;
        [EnableIn(InEditor.PlayMode)] public string playMode;
        [EnableIn(InEditor.Running)] public string running;
        [EnableIn(InEditor.Paused)] public string paused;
        [EnableIn(InEditor.EditRunning)] public string editRunning;
        [EnableIn(InEditor.EditPaused)] public string editPaused;
        [EnableIn(InEditor.PlayRunning)] public string playRunning;
        [EnableIn(InEditor.PlayPaused)] public string playPaused;
        
        // ASSET TYPE
        [Header("Asset Type")]
        [EnableIn(InAsset.Scene)] public string sceneAssetType;
        [EnableIn(InAsset.Prefab)] public string prefabAssetType;
        [EnableIn(InAsset.ScriptableObject)] public string scriptableObjectAssetType;
        
        // SEE SCRIPTABLE OBJECT
        [Header("See Scriptable Object")]
        [SerializeField] private ConditionalInDemoSO scriptableObject;
        
    }
}
