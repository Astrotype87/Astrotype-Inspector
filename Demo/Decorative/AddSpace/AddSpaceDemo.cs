using UnityEngine;

namespace AstrotypeInspector.Demo
{
    public class AddSpaceDemo : MonoBehaviour
    {
        [SerializeField] private float field;
        [AddSpace]
        [SerializeField] private float spaceDefault;
        [AddSpace(1)]
        [SerializeField] private float space1;
        [AddSpace(2)]
        [SerializeField] private float space2;
        [AddSpace(3)]
        [SerializeField] private float space3;
        [AddSpace(4)]
        [SerializeField] private float space4;
        [AddSpace(6)]
        [SerializeField] private float space6;
        [AddSpace(8)]
        [SerializeField] private float space8;
        [AddSpace(12)]
        [SerializeField] private float space12;
        [AddSpace(16)]
        [SerializeField] private float space16;
    }
}
