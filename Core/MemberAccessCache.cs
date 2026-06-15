using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace AstrotypeInspector
{
    public static class MemberAccessCache
    {
        private static readonly Dictionary<(Type, string), MemberInfo[]> memberPathCache = new();
        private static readonly Dictionary<(Type, string), Error> errorCache = new();
        
        /// <summary>
        /// Get the value of a field, property, or method inside the object.<br/>
        /// NOTE: Method must be parameterless and non-void return type.
        /// </summary>
        /// <param name="targetObject">The instance to access.</param>
        /// <param name="memberNameOrPath">The name or dotted path of field, property, or method (e.g. <c>health</c>, <c>stats.propulsion.topSpeed</c>).</param>
        /// <returns>The value of field, property, or method.</returns>
        public static Result<object> GetValue(object targetObject, string memberNameOrPath)
        {
            if (targetObject == null)
                return Errors.NullTargetObject;
            
            var result = ResolveMemberPath(targetObject.GetType(), memberNameOrPath);
            if (result.IsFailure)
                return Result.Failure(result.Error);
            
            object currentObject = targetObject;
            MemberInfo[] memberInfoPath = result.Value;
            foreach (var member in memberInfoPath)
            {
                if (member is FieldInfo f) currentObject = f.GetValue(currentObject);
                else if (member is PropertyInfo p) currentObject = p.GetValue(currentObject);
                else if (member is MethodInfo m) currentObject = m.Invoke(currentObject, null);
                else return Errors.InvalidMemberType;
            }
            
            return currentObject;
        }
        
        /// <summary>
        /// Get list of member info to describe the path of a nested member.
        /// </summary>
        private static Result<MemberInfo[]> ResolveMemberPath(Type rootType, string memberNameOrPath)
        {
            // Throw exceptions for invalid parameters
            if (rootType == null)
                throw new ArgumentNullException(nameof(rootType));
            if (string.IsNullOrWhiteSpace(memberNameOrPath))
                throw new ArgumentException("The memberNameOrPath is null or white space.", nameof(memberNameOrPath));
            
            // Return cached member path or error
            (Type, string) key = (rootType, memberNameOrPath);
            if (memberPathCache.TryGetValue(key, out MemberInfo[] memberInfoArray))
                return memberInfoArray;
            if (errorCache.TryGetValue(key, out Error error))
                return error;
            
            string[] memberNames = memberNameOrPath.Split(".");
            memberInfoArray = new MemberInfo[memberNames.Length];
            Type currentType = rootType;
            
            // Look for each member name split by '.'
            for (int i = 0; i < memberInfoArray.Length; i++)
            {
                string memberName = memberNames[i]; // TODO LATER: Trim out '()' parenthesis check for methods with no parameters
                bool isLastMember = i == memberInfoArray.Length - 1;
                const BindingFlags FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
                
                // Get reference to field, property, or method
                MemberInfo member = currentType.GetField(memberNames[i], FLAGS);
                member ??= currentType.GetProperty(memberNames[i], FLAGS);
                member ??= currentType.GetMethod(memberNames[i], FLAGS);
                
                // Check for errors: void method and member not found
                if (member is MethodInfo methodInfo && methodInfo.ReturnType == typeof(void))
                    return errorCache[key] = Errors.VoidMethod(memberName);
                if (member == null)
                    return errorCache[key] = Errors.MemberNotFound(memberName, currentType);
                
                // Save new member info, end the loop if last member.
                memberInfoArray[i] = member;
                if (isLastMember) break;
                
                // Set next current type
                if (member is FieldInfo field) currentType = field.FieldType;
                else if (member is PropertyInfo property) currentType = property.PropertyType;
                else if (member is MethodInfo method) currentType = method.ReturnType;
                else return errorCache[key] = Errors.InvalidMemberType;
            }
            
            // Save member info array to cache and return
            return memberPathCache[key] = memberInfoArray;
        }
        
        private static class Errors
        {
            public static Error NullTargetObject
                => new("NullTargetObject", $"The targetObject is null.");
                
            public static Error MemberNotFound(string memberName, Type type)
                => new("MemberNotFound", $"Member '{memberName}' not found from type '{type.Name}'");
            
            public static Error VoidMethod(string methodName)
                => new("VoidMethod", $"Method '{methodName}' has a void return type which cannot be traversed.");
            
            public static Error AmbiguousMethod(string methodName, Type type)
                => new("AmbiguousMethod", $"Multiple overloads of '{methodName}' on '{type.Name}' match the supplied arguments.");
            
            public static Error NotMethod
                => new("NotMethod", "The member path is not a method.");
                
            public static Error InvalidMemberType
                => new("InvalidMemberType", $"The MemberInfo is not a field, property, or method.");
        }
        
    }
    
}
