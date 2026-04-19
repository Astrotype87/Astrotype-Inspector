using UnityEngine;

namespace AstrotypeInspector.Demo
{
    public class SubtitleDemo : MonoBehaviour
    {
        [SerializeField] private string fieldA;
        [SerializeField] private string fieldB;
        [Subtitle("My Subtitle")]
        [SerializeField] private string fieldC;
        [SerializeField] private string fieldD;
        [Subtitle("My Subtitle")]
        [SerializeField] private string fieldE;
        [SerializeField] private string fieldF;
    }
}
