using UnityEngine;

namespace AstrotypeInspector.Demo
{
    public class ConditionalInDemo2 : MonoBehaviour
    {
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
