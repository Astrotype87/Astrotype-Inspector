using UnityEngine;

namespace AstrotypeInspector.Demo
{
    public class InfoBoxDemo : MonoBehaviour
    {
        [Header("Info Box")]
        [InfoBox("This is an info box with normal text content.", InfoType.None)]
        [SerializeField] private string noneBox;
        
        [InfoBox("This is an info box that displays extra information.", InfoType.Info)]
        [SerializeField] private string infoBox;
        
        [InfoBox("This is an info box that displays warning information.", InfoType.Warning)]
        [SerializeField] private string warningBox;
        
        [InfoBox("This is an info box that displays error information.", InfoType.Error)]
        [SerializeField] private string errorBox;
        
        
        [Header("Conditional Info Box")]
        [InfoBox("This info box is displayed if the condition is true.", showIf: "showInfoBox")]
        [SerializeField] private bool showInfoBox;
    }
}
