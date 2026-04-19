using UnityEngine;

namespace AstrotypeInspector.Demo
{
    public class UnitySpaceDemo : MonoBehaviour
    {
        [SerializeField] private float field;
        [Space]
        [SerializeField] private float spaceDefault;
        [Space(1)]
        [SerializeField] private float space1;
        [Space(2)]
        [SerializeField] private float space2;
        [Space(3)]
        [SerializeField] private float space3;
        [Space(4)]
        [SerializeField] private float space4;
        [Space(6)]
        [SerializeField] private float space6;
        [Space(8)]
        [SerializeField] private float space8;
        [Space(12)]
        [SerializeField] private float space12;
        [Space(16)]
        [SerializeField] private float space16;
    }
}
