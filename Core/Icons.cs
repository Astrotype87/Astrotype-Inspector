using System.Collections.Generic;
using UnityEngine;

namespace AstrotypeInspector
{
    public enum Icon
    {
        /// <summary> Automatically selects an icon base on field type. </summary>
        _AUTO = -1,
        /// <summary> No icon is selected. </summary>
        _NONE = 0,
        GameObject,
        Script,
        Transform,
        RectTransform
    }
    
    public static class IconDictionary
    {
        public static readonly Dictionary<Icon, string> iconDictionary = new()
        {
            { Icon._NONE, "" },
            { Icon.GameObject, "gameobject icon" },
            { Icon.Script, "cs script icon" },
            { Icon.Transform, "transform icon" },
            { Icon.RectTransform, "recttransform icon"}
        };
        
        public static string GetIconPath(Icon icon)
        {
            iconDictionary.TryGetValue(icon, out string path);
            return path;
        }
        
        public static Texture GetIconTexture(Icon icon)
        {
            if (icon == Icon._NONE || icon == Icon._AUTO) return null;
            
            #if UNITY_EDITOR
            else return UnityEditor.EditorGUIUtility.IconContent(GetIconPath(icon)).image;
            #endif
        }
        
    }
}