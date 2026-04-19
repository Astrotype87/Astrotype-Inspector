using UnityEngine;

namespace AstrotypeInspector.Demo
{
    public class TitleAttributeDemo1 : MonoBehaviour
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
        
        
        // [Title("EEEEEEEEEEEEEEEEEEEEEEEEEEEEEE")]
        // [Subtitle("EEEEEEEEEEEEEEEEEEEEEEEEEEEEEE")]
        // [Title("EEE Header Title Subtitle")]
        
        [Title("Title Subtitle"), Subtitle("Subtitle Title"), Separator]
        [SerializeField] private int fieldA;
        [SerializeField] private int fieldB;
        
        [Title("Title Subtitle"), Separator]
        [SerializeField] private int fieldC;
        [SerializeField] private int fieldD;
        
        [Subtitle("Header Title Subtitle"), Separator]
        [SerializeField] private int fieldE;
        [SerializeField] private int fieldF;
        
        [Separator]
        [SerializeField] private int fieldG;
        [SerializeField] private int fieldH;
        
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
        
        // [Title("EEEEEEEEEEEEEEEEEEEEEE", "EEEEEEEEEEEEEEEEEEEEEE", Icon.Script)]
        // [SerializeField] private int fieldCE;
        // [SerializeField] private int fieldDE;
        
        // [Title("Title", "Subtitle", Icon.Script)]
        // [SerializeField] private int fieldC;
        // [SerializeField] private int fieldD;
        
        // [Title("Non-Bold Title", "Subtitle", Icon.Script, style: TitleStyle.Normal)]
        // [SerializeField] private int fieldE;
        // [SerializeField] private int fieldF;
        
        // [Title("Default Italic Title", "Default Italic Subtitle", Icon.Script, style: TitleStyle.DefaultItalic)]
        // [SerializeField] private int fieldG;
        // [SerializeField] private int fieldH;
        
        // [Title("Bold Title", "Bold Subtitle", Icon.Script, style: TitleStyle.Bold)]
        // [SerializeField] private int fieldI;
        // [SerializeField] private int fieldJ;
        
        // [Title("Bold Italic Title", "Bold Italic Subtitle", Icon.Script, style: TitleStyle.BoldAndItalic)]
        // [SerializeField] private int fieldK;
        // [SerializeField] private int fieldL;
        
        // [Title("Numbers", "These fields uses numeric characters", Icon.Script)]
        // [SerializeField] private int intField;
        // [SerializeField] private float floatField;
        
        // [Title("Characters", "These fields uses unicode characters", Icon.Script, TitleStyle.Bold)]
        // [SerializeField] private char charField;
        // [SerializeField] private string stringField;
        
        // [Title("Bools", Icon.Script)]
        // [SerializeField] private bool boolField;
        
    }
}
