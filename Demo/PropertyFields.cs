using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AstrotypeInspector.Demo
{
    public class PropertyFields : MonoBehaviour
    {
        public bool @bool;
        public char @char;
        public string @string;
        public Enumeration @enum;
        public Flags flags;
        
        [Space]
        public sbyte @sbyte;
        public byte @byte;
        public short @short;
        public ushort @ushort;
        public int @int;
        public uint @uint;
        public long @long;
        public ulong @ulong;
        public float @float;
        public double @double;
        
        [Space]
        public Object objectReference;
        public Vector2 vector2;
        public Vector3 vector3;
        public Vector4 vector4;
        public Rect rect;
        public Bounds bounds;
        public Quaternion quaternion;
        public Color color;
        public Gradient gradient;
        public AnimationCurve animationCurve;
        public LayerMask layerMask;
        public Hash128 hash128;
        
        [Space]
        public string[] array;
        public List<string> list;
        public Class @class;
        public Struct @struct;
        
        
        
        // --------------------------------------
        
        // public Vector2Int vector2Int;
        // public Vector3Int vector3Int;
        // public RectInt rectInt;
        // public BoundsInt boundsInt;
        
        // public int integer;
        // public bool boolean;
        // public float @float;
        // public string @string;
        // public Color color;
        // public Object objectReference;
        // public LayerMask layerMask;
        // public Enumeration @enum;
        // public Vector2 vector2;
        // public Vector3 vector3;
        // public Vector4 vector4;
        // public Rect rect;
        // public char character;
        // public AnimationCurve animationCurve;
        // public Bounds bounds;
        // public Gradient gradient;
        // public Quaternion quaternion;
        // public Vector2Int vector2Int;
        // public Vector3Int vector3Int;
        // public RectInt rectInt;
        // public BoundsInt boundsInt;
        // public Hash128 hash128;
        
        
        public void Test()
        {
            
        }
        
        public enum Enumeration
        {
            ValueA,
            ValueB,
            ValueC,
            ValueD,
        }
        
        [Flags]
        public enum Flags
        {
            FlagA,
            FlagB,
            FlagC,
            FlagD,
        }
        
        [Serializable]
        public class Class
        {
            public bool boolean;
            public char character;
            public int integer;
            public float @float;
            public double @double;
            public string @string;
            public Enumeration @enum;
        }
        
        [Serializable]
        public struct Struct
        {
            public bool boolean;
            public char character;
            public int integer;
            public float @float;
            public double @double;
            public string @string;
            public Enumeration @enum;
        }
        
    }
}
