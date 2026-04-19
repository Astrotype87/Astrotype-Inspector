using UnityEngine;

namespace AstrotypeInspector.Demo
{
    public class TitleConsistencyTest_Header : MonoBehaviour
    {
        [Header("EEE Header Title Subtitle")]
        [SerializeField] private int fieldA;
        [SerializeField] private int fieldB;
        [SerializeField] private int fieldC;
        
        [Header("EEE Header Title Subtitle")]
        [Header("EEE Header Title Subtitle")]
        [Header("EEE Header Title Subtitle")]
        [SerializeField] private int fieldAA;
        [SerializeField] private int fieldBB;
        [SerializeField] private int fieldCC;
        
        [Header("EEE Header Title Subtitle")]
        [SerializeField] private int fieldAAA;
        [SerializeField] private int fieldBBB;
        [SerializeField] private int fieldCCC;
    }
}
