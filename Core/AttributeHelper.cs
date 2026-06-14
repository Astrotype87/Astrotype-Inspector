using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace AstrotypeInspector
{
    public static class AttributeHelper
    {
        private static Dictionary<MemberInfo, object[]> _memberInfoAttributesCache = new();
        
        public static object[] GetAttributes(MemberInfo memberInfo)
        {
            if (!_memberInfoAttributesCache.TryGetValue(memberInfo, out var attributes))
            {
                attributes = memberInfo.GetCustomAttributes(false);
                _memberInfoAttributesCache[memberInfo] = attributes;
            }
            return attributes;
        }
        
        
        /// <summary> 
        /// (MemberInfo, int) = member info, position of attribute<br/>
        /// bool = is attribute positioned before a group attribute
        /// </summary>
        private static Dictionary<(MemberInfo, int), bool> _isAppliedToMemberCache = new();
        
        /// <summary>
        /// Checks if this attribute will be applied to serialized members, non-serialized members and properties with [ShowInInspector],
        /// methods with [Button], properties with [ShowAsProperty], and events with [ShowAsEvent].<br/>
        /// </summary>
        /// <param name="attribute">The attribute to check.</param>
        /// <param name="memberInfo">The member</param>
        /// <returns>
        /// False if attribute is applied before a group attribute (FoldoutGroup, BoxGroup, EndGroup, etc.), otherwise returns true.
        /// </returns>
        public static bool IsAppliedToMember(this Attribute attribute, MemberInfo memberInfo)
        {
            // Get member attributes array and target attribute index
            var attributes = GetAttributes(memberInfo);
            int index = -1;
            for (int i = 0; i < attributes.Length; i++)
            {
                if (attributes[i] == attribute)
                {
                    index = 1;
                    break;
                }
            }
            if (index == -1) return false;
            
            // Create key identified by member info and attribute index
            var key = (memberInfo, index);
            
            // Get cached result
            if (!_isAppliedToMemberCache.TryGetValue(key, out bool result))
            {
                for (int i = index + 1; i < attributes.Length; i++)
                {
                    if (attributes[i] is GroupAttribute)
                    {
                        return _isAppliedToMemberCache[key] = true;
                    }
                }
                return _isAppliedToMemberCache[key] = false;
            }
            
            return result;
        }
    }
}
