using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace AstrotypeInspector
{
    public static class MemberAccessCache
    {
        /// <summary>
        /// (Type, string) represents a unique targetObject + memberPath key.<br/>
        /// • Type is the target object like MonoBehaviour or ScriptableObject.<br/>
        /// • string is the property path relative to the target object.<br/>
        /// MemberInfo[] is the chain of member info to describe member path. 
        /// </summary>
        private static readonly Dictionary<(Type, string), MemberInfo[]> memberInfoPathCache = new();
        
        /// <summary> Get value of a member by its target object and nested member path. </summary>
        public static object GetMemberValue(object targetObject, string memberPath)
        {
            MemberInfo[] memberInfoPath = GetMemberInfoPath(targetObject.GetType(), memberPath);
            object currentObject = targetObject;
            
            foreach (var memberInfo in memberInfoPath)
            {
                if (currentObject == null) return null;
                
                if (memberInfo is FieldInfo field) currentObject = field.GetValue(currentObject);
                else if (memberInfo is PropertyInfo property) currentObject = property.GetValue(currentObject);
                else if (memberInfo is MethodInfo method) currentObject = method.Invoke(currentObject, null);
            }
            
            return currentObject;
        }
        
        public static void SetMemberValue(object targetObject, string memberPath)
        {
            
        }
        
        
        
        /// <summary> Get list of member info to describe member path, staring from root type towards the target member. </summary>
        private static MemberInfo[] GetMemberInfoPath(Type rootType, string memberPath)
        {
            // Check if type is null or path is null or whitespace
            if (rootType == null || string.IsNullOrWhiteSpace(memberPath))
                return Array.Empty<MemberInfo>();
            
            // Create member key using member type and property path
            (Type, string) key = (rootType, memberPath);
            
            // Retrieve cached member path by (type, path) pair, and return value
            if (memberInfoPathCache.TryGetValue(key, out MemberInfo[] membersArray))
                return membersArray;
            
            
            // Initialize variables
            string[] memberNames = memberPath.Split(".");
            membersArray = new MemberInfo[memberNames.Length];
            Type currentType = rootType;
            
            // Look for each member name split by '.'
            for (int i = 0; i < membersArray.Length; i++)
            {
                // The target member must be an instance type, and it can be public or nonpublic
                const BindingFlags FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
                
                // Get reference to field/property/method
                MemberInfo member = currentType.GetField(memberNames[i], FLAGS) as MemberInfo
                    ?? currentType.GetProperty(memberNames[i], FLAGS) as MemberInfo
                    ?? currentType.GetMethod(memberNames[i], FLAGS);
                
                // Return null array if field/property/method does not exist, skipping entire member search
                if (member  == null)
                {
                    // TODO: [WARNING CHECKPOINT] Put warning in WarningRegistry.AddWarning(object)
                    Debug.LogWarning($"Member \"{memberNames[i]}\" not found from type {currentType.Name} ({currentType.FullName})");
                    return null;
                }
                
                // Add member info top list
                membersArray[i] = member;
                
                // Set this member as the next type to search from
                currentType = null;
                if (member is FieldInfo field) currentType = field.FieldType;
                else if (member is PropertyInfo property) currentType = property.PropertyType;
                else if (member is MethodInfo method) currentType = method.ReturnType;
            }
            
            // Save member info array to cache and return
            return memberInfoPathCache[key] = membersArray;
        }
        
    }
    
}
