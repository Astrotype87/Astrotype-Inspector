using UnityEngine;

namespace AstrotypeInspector.Demo
{
    public class WarningTest : MonoBehaviour
    {
        // [Warning("Warning 1"), Warning("Warning 2"), Warning("Warning 3"), Warning("Warning 4")]
        // [SerializeField] private string text;
        [Warning("Warning 1")]
        [SerializeField] private string text;
        
        [SerializeField] private string myValue;
    }
}
