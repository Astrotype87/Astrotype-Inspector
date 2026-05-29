using System;
using System.Diagnostics;
using UnityEngine;

namespace AstrotypeInspector
{
    [Conditional(Symbols.UNITY_EDITOR), Conditional(Symbols.INCLUDE_IN_BUILD)]
    [AttributeUsage(AttributeTargets.Field, Inherited = true)]
    public abstract class ConditionalInAttribute : WrapperAttribute
    {
        public bool IsEnumGroup = false;
        
        public readonly InEditor InEditor = InEditor.Any;
        public readonly InAsset InAsset = InAsset.Any;
        public readonly InPrefab InPrefab = InPrefab.Any;
        
        public readonly BoolGroup BoolGroup;
        public readonly object[] EnumValues;
        
        
        protected ConditionalInAttribute(InEditor inEditor) : base(true)
        {
            InEditor = inEditor;
        }
        
        protected ConditionalInAttribute(InAsset inAsset) : base(true)
        {
            InAsset = inAsset;
        }
        
        protected ConditionalInAttribute(InPrefab inPrefab) : base(true)
        {
            InPrefab = inPrefab;
        }
        
        
        protected ConditionalInAttribute(InEditor inEditor, InAsset inAsset) : base(true)
        {
            InEditor = inEditor;
            InAsset = inAsset;
        }
        
        protected ConditionalInAttribute(InAsset inAsset, InPrefab inPrefab) : base(true)
        {
            InAsset = inAsset;
            InPrefab = inPrefab;
        }
        
        protected ConditionalInAttribute(InEditor inEditor, InPrefab inPrefab) : base(true)
        {
            InEditor = inEditor;
            InPrefab = inPrefab;
        }
        
        
        protected ConditionalInAttribute(InEditor inEditor, InAsset inAsset, InPrefab inPrefab) : base(true)
        {
            InEditor = inEditor;
            InAsset = inAsset;
            InPrefab = inPrefab;
        }
        
        protected ConditionalInAttribute(BoolGroup boolGroup, params object[] enumValues) : base(true)
        {
            IsEnumGroup = true;
            BoolGroup = boolGroup;
            EnumValues = enumValues;
        }
        
    }
    
    public sealed class ShowInAttribute : ConditionalInAttribute
    {
        public ShowInAttribute(InEditor inEditor) : base(inEditor) { }
        public ShowInAttribute(InAsset inAsset) : base(inAsset) { }
        public ShowInAttribute(InPrefab inPrefab) : base(inPrefab) { }
        
        public ShowInAttribute(InEditor inEditor, InAsset inAsset) : base(inEditor, inAsset) { }
        public ShowInAttribute(InAsset inAsset, InPrefab inPrefab) : base(inAsset, inPrefab) { }
        public ShowInAttribute(InEditor inEditor, InPrefab inPrefab) : base(inEditor, inPrefab) { }
        
        public ShowInAttribute(InEditor inEditor, InAsset inAsset, InPrefab inPrefab) : base(inEditor, inAsset, inPrefab) { }
        public ShowInAttribute(BoolGroup boolGroup, params object[] enumValues) : base(boolGroup, enumValues) { }
    }
    
    public sealed class HideInAttribute : ConditionalInAttribute
    {
        public HideInAttribute(InEditor inEditor) : base(inEditor) { }
        public HideInAttribute(InAsset inAsset) : base(inAsset) { }
        public HideInAttribute(InPrefab inPrefab) : base(inPrefab) { }
        
        public HideInAttribute(InEditor inEditor, InAsset inAsset) : base(inEditor, inAsset) { }
        public HideInAttribute(InAsset inAsset, InPrefab inPrefab) : base(inAsset, inPrefab) { }
        public HideInAttribute(InEditor inEditor, InPrefab inPrefab) : base(inEditor, inPrefab) { }
        
        public HideInAttribute(InEditor inEditor, InAsset inAsset, InPrefab inPrefab) : base(inEditor, inAsset, inPrefab) { }
        public HideInAttribute(BoolGroup boolGroup, params object[] enumValues) : base(boolGroup, enumValues) { }
    }
    
    public sealed class EnableInAttribute : ConditionalInAttribute
    {
        public EnableInAttribute(InEditor inEditor) : base(inEditor) { }
        public EnableInAttribute(InAsset inAsset) : base(inAsset) { }
        public EnableInAttribute(InPrefab inPrefab) : base(inPrefab) { }
        
        public EnableInAttribute(InEditor inEditor, InAsset inAsset) : base(inEditor, inAsset) { }
        public EnableInAttribute(InAsset inAsset, InPrefab inPrefab) : base(inAsset, inPrefab) { }
        public EnableInAttribute(InEditor inEditor, InPrefab inPrefab) : base(inEditor, inPrefab) { }
        
        public EnableInAttribute(InEditor inEditor, InAsset inAsset, InPrefab inPrefab) : base(inEditor, inAsset, inPrefab) { }
        public EnableInAttribute(BoolGroup boolGroup, params object[] enumValues) : base(boolGroup, enumValues) { }
    }
    
    public sealed class DisableInAttribute : ConditionalInAttribute
    {
        public DisableInAttribute(InEditor inEditor) : base(inEditor) { }
        public DisableInAttribute(InAsset inAsset) : base(inAsset) { }
        public DisableInAttribute(InPrefab inPrefab) : base(inPrefab) { }
        
        public DisableInAttribute(InEditor inEditor, InAsset inAsset) : base(inEditor, inAsset) { }
        public DisableInAttribute(InAsset inAsset, InPrefab inPrefab) : base(inAsset, inPrefab) { }
        public DisableInAttribute(InEditor inEditor, InPrefab inPrefab) : base(inEditor, inPrefab) { }
        
        public DisableInAttribute(InEditor inEditor, InAsset inAsset, InPrefab inPrefab) : base(inEditor, inAsset, inPrefab) { }
        public DisableInAttribute(BoolGroup boolGroup, params object[] enumValues) : base(boolGroup, enumValues) { }
    }
    
}

#if UNITY_EDITOR
namespace AstrotypeInspector.Editor
{
    using System;
    using UnityEngine;
    using UnityEditor;
    using UnityEngine.UIElements;
    using UnityEditor.UIElements;
    using UnityEditor.SceneManagement;
    
    [CustomPropertyDrawer(typeof(ConditionalInAttribute), true)]
    public class ConditionalInDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return IsVisible(property, attribute as ConditionalInAttribute)
                ? EditorGUI.GetPropertyHeight(property, label, true)
                : -EditorGUIUtility.standardVerticalSpacing;
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attribute = this.attribute as ConditionalInAttribute;
            bool isVisible = IsVisible(property, attribute);
            bool isEnabled = IsEnabled(property, attribute);
            
            if (isVisible)
            {
                EditorGUI.BeginDisabledGroup(!isEnabled);
                EditorGUI.PropertyField(position, property, label, true);
                EditorGUI.EndDisabledGroup();
            }
        }
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var propertyField = new PropertyField(property);
            propertyField.name = "conditional-in-wrapper";
            
            // Define property field update for editor update events
            void UpdatePropertyField(PauseState _ = default)
            {
                if (property == null) return;
                if (property.serializedObject == null) return;
                if (property.serializedObject.targetObject == null) return;
                
                var attribute = this.attribute as ConditionalInAttribute;
                bool isVisible = IsVisible(property, attribute);
                bool isEnabled = IsEnabled(property, attribute);
                
                propertyField.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
                propertyField.SetEnabled(isEnabled);
            }
            
            // Subscribe and unsubscribe to pause state change event
            propertyField.RegisterCallback<AttachToPanelEvent>(_ =>
                EditorApplication.pauseStateChanged += UpdatePropertyField);
            
            propertyField.RegisterCallback<DetachFromPanelEvent>(_ =>
                EditorApplication.pauseStateChanged -= UpdatePropertyField);
            
            // schedule.Execute is called every time you switch between Edit and Play mode
            // works even if reload domain or scene is disabled
            propertyField.schedule.Execute(() => UpdatePropertyField());
            
            return propertyField;
        }
        
        
        // PRIVATE STATIC MEMBERS
        private static bool IsVisible(SerializedProperty property, ConditionalInAttribute attribute)
        {
            bool condition = EvaluateCondition(property, attribute);
            return attribute is ShowInAttribute ? condition
                : attribute is HideInAttribute ? !condition
                : true;
        }
        
        private static bool IsEnabled(SerializedProperty property, ConditionalInAttribute attribute)
        {
            bool condition = EvaluateCondition(property, attribute);
            return attribute is EnableInAttribute ? condition
                : attribute is DisableInAttribute ? !condition
                : true;
        }
        
        
        private static bool EvaluateCondition(SerializedProperty property, ConditionalInAttribute attribute)
        {
            if (attribute.IsEnumGroup)
            {
                return EvaluateEnumGroup(property, attribute.BoolGroup, attribute.EnumValues);
            }
            else
            {
                bool isInEditor = EvaluateEditorState(attribute.InEditor);
                bool isInAsset = EvaluateAssetType(property, attribute.InAsset);
                bool isInPrefab = EvaluatePrefabType(property, attribute.InPrefab);
                return isInEditor && isInAsset && isInPrefab;
            }
            
        }
        
        private static bool EvaluateEnumGroup(SerializedProperty property, BoolGroup boolOperation, object[] enumGroup)
        {
            bool IsTrue(object enumValue)
            {
                if (enumValue is InEditor inEditor) return EvaluateEditorState(inEditor);
                if (enumValue is InAsset inAsset) return EvaluateAssetType(property, inAsset);
                if (enumValue is InPrefab inPrefab) return EvaluatePrefabType(property, inPrefab);
                return false;
            }
            
            if (boolOperation is BoolGroup.All) // no false values
            {
                foreach (object enumValue in enumGroup)
                    if (!IsTrue(enumValue)) return false;
                return true;
            }
            else if (boolOperation is BoolGroup.Any) // at least one true value
            {
                foreach (object enumValue in enumGroup)
                    if (IsTrue(enumValue)) return true;
                return false;
            }
            else if (boolOperation is BoolGroup.NotAll) // at least one false value
            {
                foreach (object enumValue in enumGroup)
                    if (!IsTrue(enumValue)) return true;
                return false;
            }
            else if (boolOperation is BoolGroup.None) // no true values
            {
                foreach (object enumValue in enumGroup)
                    if (IsTrue(enumValue)) return false;
                return true;
            }
            else if (boolOperation is BoolGroup.Odd or BoolGroup.Even) // odd or even true values
            {
                bool isOdd = false;
                foreach (object enumValue in enumGroup)
                    isOdd ^= IsTrue(enumValue);
                return boolOperation == BoolGroup.Odd ? isOdd : !isOdd;
            }
            else if (boolOperation is BoolGroup.Imply) // at least one false value before last, or last value is true
            {
                if (enumGroup.Length == 0) return true;
                
                for (int i = 0; i < enumGroup.Length - 1; i++)
                    if (!IsTrue(enumGroup[i])) return true;
                return IsTrue(enumGroup[^1]);
            }
            else if (boolOperation is BoolGroup.NotImply) // no false values except last value is false
            {
                if (enumGroup.Length == 0) return false;
                
                for (int i = 0; i < enumGroup.Length - 1; i++)
                    if (!IsTrue(enumGroup[i])) return false;
                return !IsTrue(enumGroup[^1]);
            }
            else if (boolOperation is BoolGroup.Same) // no different values
            {
                if (enumGroup.Length == 0) return true;
                
                bool firstValue = IsTrue(enumGroup[0]);
                for (int i = 1; i < enumGroup.Length; i++)
                    if (firstValue != IsTrue(enumGroup[i])) return false;
                return true;
            }
            else if (boolOperation is BoolGroup.Different) // at least one different value
            {
                if (enumGroup.Length == 0) return false;
                
                bool firstValue = IsTrue(enumGroup[0]);
                for (int i = 1; i < enumGroup.Length; i++)
                    if (firstValue != IsTrue(enumGroup[i])) return true;
                return false;
            }
            else
            {
                throw new NotImplementedException($"The evaluation logic for {boolOperation} is not implemented.");
            }
        }
        
        
        private static bool EvaluateEditorState(InEditor inEditor)
        {
            if (inEditor == InEditor.Any) return true;
            
            bool isPlaying = EditorApplication.isPlaying;
            bool isPaused = EditorApplication.isPaused;
            
            return inEditor switch
            {
                InEditor.EditMode => !isPlaying,
                InEditor.PlayMode => isPlaying,
                InEditor.Running => !isPaused,
                InEditor.Paused => isPaused,
                InEditor.EditRunning => !isPlaying && !isPaused,
                InEditor.EditPaused => !isPlaying && isPaused,
                InEditor.PlayRunning => isPlaying && !isPaused,
                InEditor.PlayPaused => isPlaying && isPaused,
                _ => throw new NotImplementedException($"The evaluation logic for {inEditor} is not implemented.")
            };
        }
        
        private static bool EvaluateAssetType(SerializedProperty property, InAsset inAsset)
        {
            if (inAsset == InAsset.Any) return true;
            
            object targetObject = property.serializedObject.targetObject;
            
            if (targetObject is ScriptableObject)
                return inAsset == InAsset.ScriptableObject;
            
            GameObject gameObject = targetObject switch
            {
                Component c => c.gameObject,
                GameObject go => go,
                _ => null
            };
            
            if (gameObject)
                return gameObject.scene.IsValid()
                    ? inAsset == InAsset.Scene
                    : inAsset == InAsset.Prefab;
            
            return false;
        }
        
        private static bool EvaluatePrefabType(SerializedProperty property, InPrefab inPrefab)
        {
            if (inPrefab == InPrefab.Any) return true;
            
            object targetObject = property.serializedObject.targetObject;
            GameObject gameObject = targetObject switch
            {
                Component c => c.gameObject,
                GameObject go => go,
                _ => null
            };
            if (!gameObject) return inPrefab == InPrefab.NotPrefab;
            
            var stage = PrefabStageUtility.GetCurrentPrefabStage();
            return inPrefab switch
            {
                InPrefab.NotPrefab => !PrefabUtility.IsPartOfAnyPrefab(gameObject),
                InPrefab.Prefab => PrefabUtility.IsPartOfAnyPrefab(gameObject),
                InPrefab.Asset => PrefabUtility.IsPartOfPrefabAsset(gameObject),
                InPrefab.Instance => PrefabUtility.IsPartOfPrefabInstance(gameObject),
                InPrefab.Regular => PrefabUtility.IsPartOfRegularPrefab(gameObject),
                InPrefab.Variant => PrefabUtility.IsPartOfVariantPrefab(gameObject),
                InPrefab.Root => PrefabUtility.IsOutermostPrefabInstanceRoot(gameObject),
                InPrefab.Child => !PrefabUtility.IsOutermostPrefabInstanceRoot(gameObject),
                InPrefab.Model => PrefabUtility.IsPartOfModelPrefab(gameObject),
                InPrefab.EditMode => stage != null && stage.IsPartOfPrefabContents(gameObject),
                _ => throw new NotImplementedException($"The evaluation logic for {inPrefab} is not implemented.")
            };
        }
        
    }
}
#endif

