using UnityEngine;

namespace AstrotypeInspector.Demo
{
    public class TitleAttributeDemo2 : MonoBehaviour
    {
        // [Header("Header")]
        // [Header("Header 2")]
        // [Header("Header 3")]
        // [SerializeField] private string myTitle;
        // [SerializeField] private string mySubtitle;
        
        // [Header("Header")]
        // [Header("Header")]
        // [SerializeField] private string myTitle;
        // [SerializeField] private string mySubtitle;
        
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
        
        // [Title("Title", "Subtitle", Icon._NONE)]
        // [SerializeField] private int fieldC;
        // [SerializeField] private int fieldD;
        
        // [Title("Non-Bold Title", "Subtitle", Icon._NONE, style: TitleStyle.Normal)]
        // [SerializeField] private int fieldE;
        // [SerializeField] private int fieldF;
        
        // [Title("Default Italic Title", "Default Italic Subtitle", Icon._NONE, style: TitleStyle.DefaultItalic)]
        // [SerializeField] private int fieldG;
        // [SerializeField] private int fieldH;
        
        // [Title("Bold Title", "Bold Subtitle", Icon._NONE, style: TitleStyle.Bold)]
        // [SerializeField] private int fieldI;
        // [SerializeField] private int fieldJ;
        
        // [Title("Bold Italic Title", "Bold Italic Subtitle", Icon._NONE, style: TitleStyle.BoldAndItalic)]
        // [SerializeField] private int fieldK;
        // [SerializeField] private int fieldL;
        
        // [Title("Numbers", "These fields uses numeric characters", Icon._NONE)]
        // [SerializeField] private int intField;
        // [SerializeField] private float floatField;
        
        // [Title("Characters", "These fields uses unicode characters", Icon._NONE, TitleStyle.Bold)]
        // [SerializeField] private char charField;
        // [SerializeField] private string stringField;
        
        // [Title("Bools", Icon.GameObject)]
        // [SerializeField] private bool boolField;
        
    }
}
