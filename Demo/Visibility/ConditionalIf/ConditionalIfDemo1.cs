using System;
using System.Collections.Generic;
using UnityEngine;

namespace AstrotypeInspector.Demo
{
    public class ConditionalIfDemo1 : MonoBehaviour
    {
        // BOOL FIELD
        [Header("Bool Field")]
        [SerializeField] private bool active;
        [SerializeField, ShowIf(nameof(active))] private int showIfActive;
        [SerializeField, HideIf(nameof(active))] private int hideIfActive;
        [SerializeField, EnableIf(nameof(active))] private int enableIfActive;
        [SerializeField, DisableIf(nameof(active))] private int disableIfActive;
        
        // BOOL PROPERTY
        [Header("Bool Property")]
        [SerializeField] private int integerValue;
        [SerializeField, ShowIf(nameof(IsPositive))] private bool positiveInteger;
        [SerializeField, HideIf(nameof(IsPositive))] private bool negativeInteger;
        
        public bool IsPositive => integerValue >= 0;
        
        // BOOL METHOD
        [Header("Bool Method")]
        [SerializeField] private int health;
        [SerializeField, EnableIf(nameof(IsAlive))] private string aliveText;
        [SerializeField, DisableIf(nameof(IsAlive))] private string deadText;
        
        bool IsAlive() => health > 0;
        
        // OBJECT REFERENCE AS BOOL CONDITION
        [Header("Object Reference as Bool Condition")]
        [SerializeField] private UnityEngine.Object anyUnityObject;
        [SerializeField, EnableIf(nameof(anyUnityObject))] private string enableIfAssigned;
        [SerializeField, DisableIf(nameof(anyUnityObject))] private string disableIfAssigned;
        [SerializeField, ShowIf(nameof(anyUnityObject))] private string showIfAssigned;
        [SerializeField, HideIf(nameof(anyUnityObject))] private string hideIfAssigned;
        
        
    }
}
