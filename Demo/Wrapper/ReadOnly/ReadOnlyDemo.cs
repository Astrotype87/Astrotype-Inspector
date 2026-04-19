using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace AstrotypeInspector.Demo
{
    public class ReadOnlyDemo : MonoBehaviour
    {
        [SerializeField, ReadOnly] private int readOnlyInt;
        [SerializeField, ReadOnly] private float readOnlyFloat;
        [SerializeField, ReadOnly] private string readOnlyString;
        [Space]
        [SerializeField, ReadOnly] private int[] readOnlyIntArray;
        [SerializeField, ReadOnly] private List<string> readOnlyStringList;
        [Space]
        [SerializeField, ReadOnly] private MyStruct myStruct;
        [SerializeField, ReadOnly] private MyClass myClass;
        
        [Serializable]
        public struct MyStruct
        {
            public int myStructInt;
            public float myStructFloat;
            public string myStructString;
        }
        
        [Serializable]
        public struct MyClass
        {
            public int myClassInt;
            public float myClassFloat;
            public string myClassString;
        }
    }
}
