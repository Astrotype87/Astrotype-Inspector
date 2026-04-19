using UnityEngine;

namespace AstrotypeInspector.Demo
{
    public class ConditionalInDemo3 : MonoBehaviour
    {
        // MULTIPLE EDITOR CONDITIONS
        [Header("Multiple Editor Conditions")]
        
        [EnableIn(BoolGroup.Any, InEditor.EditRunning, InEditor.PlayPaused)]
        public string runningEditOrPlayPaused;
        
        
    }
}
