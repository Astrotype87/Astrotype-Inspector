using System;
using System.Collections.Generic;
using UnityEngine;

namespace AstrotypeInspector.Demo
{
    public class ConditionalIfDemo3 : MonoBehaviour
    {
        // ARRAY AND LIST
        [Header("Array and List")]
        [SerializeField] private bool showArray;
        [SerializeField] private bool enableList;
        [SerializeField, ShowIf(nameof(showArray))] private string[] stringArray = { "Alpha", "Beta", "Gamma", "Delta" };
        [SerializeField, EnableIf(nameof(enableList))] private List<string> stringList = new(){ "Alpha", "Beta", "Gamma", "Delta" };
        
        
        // NESTED MEMBERS
        [Header("Nested Members")]
        [SerializeField] private Options options;
        [SerializeField, ShowIf("options.showDetails")] private string vehicleName;
        [SerializeField, ShowIf("options.showDetails")] private int serialNumber;
        [SerializeField, Multiline, EnableIf("options.editDescription")] private string description;
        
        [Serializable]
        public class Options
        {
            public bool showDetails;
            public bool editDescription;
        }
        
        
        // CLASS AND STRUCT
        [Header("Class and Struct")]
        [SerializeField] private bool enableClass;
        [SerializeField, EnableIf(nameof(enableClass))] private MyClass myClass;
        
        [Serializable]
        public class MyClass
        {
            [SerializeField] private bool showValue;
            [SerializeField] private bool enableValue;
            [SerializeField] private bool activateValue;
            
            [ShowIf(nameof(showValue)), EnableIf(nameof(enableValue))]
            [SerializeField] private float myValue;
            
            [EnableIf(BoolGroup.All, nameof(showValue), nameof(enableValue), nameof(activateValue))]
            [SerializeField] private string showEnableActivate;
            
            public enum MyMode { Alpha, Beta, Gamma }
            
            [SerializeField] private MyMode myMode;
            [SerializeField, ShowIf(nameof(myMode), MyMode.Alpha)] private string alphaString;
            [SerializeField, ShowIf(nameof(myMode), MyMode.Beta)] private int betaInt;
            [SerializeField, ShowIf(nameof(myMode), MyMode.Beta, MyMode.Gamma)] private float betaGammaVector2;
            [SerializeField, ShowIf(nameof(myMode), MyMode.Alpha, MyMode.Beta, MyMode.Gamma)] private float alphaBetaGamma3;
            
            [SerializeField] private bool enableNested;
            [SerializeField, EnableIf(nameof(enableNested))] private MyNestedClass myNestedClass;
            
            [Serializable]
            public class MyNestedClass
            {
                [SerializeField] private bool showNestValue;
                [SerializeField] private bool enableNestValue;
                
                [ShowIf(nameof(showNestValue)), EnableIf(nameof(enableNestValue))]
                [SerializeField] public float myNestedValue;
            }
        }
        
        
        // COMBINED SHOW AND ENABLE
        [Header("Combined Show and Enable")]
        [SerializeField] private bool showName;
        [SerializeField] private bool editName;
        
        [ShowIf(nameof(showName)), EnableIf(nameof(editName))]
        [SerializeField] private string playerName;
        
        
        // SHOW/HIDE IF ATTRIBUTE CHAIN
        [Header("Show/Hide If Attribute Chain")]
        public bool showA;
        public bool hideB;
        public bool showC;
        public bool showD;
        
        [ShowIf("showA"), HideIf("hideB"), ShowIf("showC"), ShowIf("showD")]
        public string visibleField;
        
        // ENABLE/DISABLE IF ATTRIBUTE CHAIN
        [Header("Enable/Disable If Attribute Chain")]
        public bool enableA;
        public bool enableB;
        public bool disableC;
        public bool enableD;
        
        [EnableIf("enableA"), EnableIf("enableB"), DisableIf("disableC"), EnableIf("enableD")]
        public string enabledField;
        
    }
}
