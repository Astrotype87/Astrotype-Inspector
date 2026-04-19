using System;
using System.Collections.Generic;
using UnityEngine;

namespace AstrotypeInspector.Demo
{
    public class ConditionalIfDemo2 : MonoBehaviour
    {
        // BOOL GROUP
        [Header("Bool Group")]
        [SerializeField] private bool boolA;
        [SerializeField] private bool boolB;
        [SerializeField] private bool boolC;
        [Space]
        [EnableIf(BoolGroup.All, nameof(boolA), nameof(boolB), nameof(boolC))]
        [SerializeField] private int allField;
        [EnableIf(BoolGroup.Any, nameof(boolA), nameof(boolB), nameof(boolC))]
        [SerializeField] private int anyField;
        [Space]
        [EnableIf(BoolGroup.NotAll, nameof(boolA), nameof(boolB), nameof(boolC))]
        [SerializeField] private int notAllField;
        [EnableIf(BoolGroup.None, nameof(boolA), nameof(boolB), nameof(boolC))]
        [SerializeField] private int noneField;
        [Space]
        [EnableIf(BoolGroup.Odd, nameof(boolA), nameof(boolB), nameof(boolC))]
        [SerializeField] private int oddField;
        [EnableIf(BoolGroup.Even, nameof(boolA), nameof(boolB), nameof(boolC))]
        [SerializeField] private int evenField;
        [Space]
        [EnableIf(BoolGroup.Imply, nameof(boolA), nameof(boolB), nameof(boolC))]
        [SerializeField] private int implyField;
        [EnableIf(BoolGroup.NotImply, nameof(boolA), nameof(boolB), nameof(boolC))]
        [SerializeField] private int notImplyField;
        [Space]
        [EnableIf(BoolGroup.Same, nameof(boolA), nameof(boolB), nameof(boolC))]
        [SerializeField] private int sameField;
        [EnableIf(BoolGroup.Different, nameof(boolA), nameof(boolB), nameof(boolC))]
        [SerializeField] private int differentField;
        
        
        // ENUM CONDITION
        public enum ColorMode { RGB, HSV, HSL, HEX }
        
        [Header("Enum Condition")]
        [SerializeField] private ColorMode colorMode;
        
        [SerializeField, Range(0, 255), ShowIf(nameof(colorMode), ColorMode.RGB)] private int red = 30;
        [SerializeField, Range(0, 255), ShowIf(nameof(colorMode), ColorMode.RGB)] private int green = 41;
        [SerializeField, Range(0, 255), ShowIf(nameof(colorMode), ColorMode.RGB)] private int blue = 82;
        
        [SerializeField, Range(0, 360), ShowIf(nameof(colorMode), ColorMode.HSV, ColorMode.HSL)] private float hue = 227;
        [SerializeField, Range(0, 100), ShowIf(nameof(colorMode), ColorMode.HSV, ColorMode.HSL)] private float saturation = 63;
        [SerializeField, Range(0, 100), ShowIf(nameof(colorMode), ColorMode.HSV)] private float value = 32;
        [SerializeField, Range(0, 100), ShowIf(nameof(colorMode), ColorMode.HSL)] private float lightness = 16;
        
        [SerializeField, ShowIf(nameof(colorMode), ColorMode.HEX)] private string hexCode = "#1E2952";
        
        
    }
}
