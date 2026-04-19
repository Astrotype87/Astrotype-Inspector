using UnityEngine;

namespace AstrotypeInspector.Demo
{
    public class SeparatorDemo : MonoBehaviour
    {
        [SerializeField] private string fieldA;
        [SerializeField] private string fieldB;
        [Separator(0f)]
        [SerializeField] private string fieldC;
        [SerializeField] private string fieldD;
        [SerializeField] private string fieldE;
        [Separator(1f)]
        [SerializeField] private string fieldF;
        [Separator(2f)]
        [SerializeField] private string fieldG;
        [SerializeField] private string fieldH;
        [Separator(3f)]
        [SerializeField] private string fieldI;
        [SerializeField] private string fieldJ;
        [Separator(4f)]
        [SerializeField] private string fieldK;
        [SerializeField] private string fieldL;
        
        
        // [SerializeField] private string fieldA;
        // [SerializeField] private string fieldB;
        // [Separator]
        // [SerializeField] private string fieldC;
        // [SerializeField] private string fieldD;
        // [SerializeField] private string fieldE;
        // [Separator]
        // [SerializeField] private string fieldF;
        // [Separator]
        // [SerializeField] private string fieldG;
        // [SerializeField] private string fieldH;
        // [Separator]
        // [SerializeField] private string fieldI;
        // [SerializeField] private string fieldJ;
        // [Separator]
        // [SerializeField] private string fieldK;
        // [SerializeField] private string fieldL;
    }
}
