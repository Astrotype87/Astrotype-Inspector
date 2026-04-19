using UnityEngine;

namespace AstrotypeInspector.Demo
{
    public class TitleConsistencyTest_Title : MonoBehaviour
    {
        [Title("EEE Header Title Subtitle")]
        [SerializeField] private int fieldA;
        [SerializeField] private int fieldB;
        [SerializeField] private int fieldC;
        
        [Title("EEE Header Title Subtitle")]
        [Title("EEE Header Title Subtitle")]
        [Title("EEE Header Title Subtitle")]
        [SerializeField] private int fieldAA;
        [SerializeField] private int fieldBB;
        [SerializeField] private int fieldCC;
        
        [Title("EEE Header Title Subtitle")]
        [SerializeField] private int fieldAAA;
        [SerializeField] private int fieldBBB;
        [SerializeField] private int fieldCCC;
    }
}
