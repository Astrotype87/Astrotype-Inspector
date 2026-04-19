using UnityEngine;

namespace AstrotypeInspector.Demo
{
    [CreateAssetMenu(fileName = "ConditionalInDemoSO", menuName = "Scriptable Objects/ConditionalInDemoSO")]
    public class ConditionalInDemoSO : ScriptableObject
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
        
        // PREFAB CONTEXT
        [Header("Prefab Context")]
        [EnableIn(InPrefab.NotPrefab)] public string notPrefab;
        [EnableIn(InPrefab.Prefab)] public string prefab;
        [EnableIn(InPrefab.Asset)] public string prefabAsset;
        [EnableIn(InPrefab.Instance)] public string prefabInstance;
        [EnableIn(InPrefab.Regular)] public string regularPrefab;
        [EnableIn(InPrefab.Variant)] public string variantPrefab;
        [EnableIn(InPrefab.Root)] public string rootPrefab;
        [EnableIn(InPrefab.Model)] public string modelPrefab;
        [EnableIn(InPrefab.EditMode)] public string prefabEditMode;
        
        
    }
}
