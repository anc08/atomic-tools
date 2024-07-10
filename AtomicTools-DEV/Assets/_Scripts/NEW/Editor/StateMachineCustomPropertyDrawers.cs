using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AtomicTools
{
    [CustomPropertyDrawer(typeof(ATStateTransition))]
    public class ATStateTransitionDrawer : PropertyDrawer
    {
        private ATStateMachine objref;
        private List<string> methodNames = new List<string>();
        private string[] emptyNames = new string[] { "Link behavior script to choose success method." };

        SerializedProperty transitionType;
        SerializedProperty transitionConditions;
        SerializedProperty conditionEvaluation;
        SerializedProperty fromState;
        SerializedProperty toState;
        SerializedProperty successMethodName;

        SerializedProperty triggerEnterTags;
        SerializedProperty collisionTags;
        SerializedProperty timerLength;
        SerializedProperty callId;

        SerializedProperty selectedMethod;

        GUIStyle boldFoldout = EditorStyles.foldout;

        private void OnEnable()
        {
            boldFoldout.fontStyle = FontStyle.Bold;
        }

        private void GetProperties(SerializedProperty property)
        {
            // Get properties
            transitionType = property.FindPropertyRelative("transitionType");
            transitionConditions = property.FindPropertyRelative("transitionConditions");
            conditionEvaluation = property.FindPropertyRelative("conditionEvaluation");
            fromState = property.FindPropertyRelative("fromState");
            toState = property.FindPropertyRelative("toState");
            successMethodName = property.FindPropertyRelative("successMethodName");
            selectedMethod = property.FindPropertyRelative("selectedMethod");

            // Conditional properties
            triggerEnterTags = property.FindPropertyRelative("triggerEnterTags");
            collisionTags = property.FindPropertyRelative("collisionTags");
            timerLength = property.FindPropertyRelative("timerLength");
            callId = property.FindPropertyRelative("callId");

            // Get names of behavior methods
            try
            {
                objref = (ATStateMachine)property.serializedObject.targetObject;
                methodNames = objref.GetComponent<ATStateMachine>().GetBehaviorMethodNames();
                if (methodNames.Count > 0)
                    methodNames.Insert(0, "None");
            }
            catch
            {
                objref = null;
                methodNames.Clear();
            }

            // for formatting
            boldFoldout.fontStyle = FontStyle.Bold;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GetProperties(property);

            EditorGUI.BeginProperty(position, label, property);

            EditorGUILayout.PropertyField(transitionType);

            if (transitionType.enumValueIndex == 0) // TriggerEnterTag index
                EditorGUILayout.PropertyField(triggerEnterTags);
            if (transitionType.enumValueIndex == 1) // CollisionTag index
                EditorGUILayout.PropertyField(collisionTags);
            if (transitionType.enumValueIndex == 2) // Timer index
                EditorGUILayout.PropertyField(timerLength);
            if (transitionType.enumValueIndex == 3) // CallID index
                EditorGUILayout.PropertyField(callId);

            EditorGUILayout.PropertyField(conditionEvaluation);
            EditorGUILayout.PropertyField(transitionConditions);
            EditorGUILayout.PropertyField(fromState);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("To State");
            EditorGUILayout.PropertyField(toState);
            EditorGUILayout.EndHorizontal();

            // SUCCESS
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Success Method");
            if (methodNames.Count > 1) // If a valid behavior script exists
            {
                selectedMethod.intValue = EditorGUILayout.Popup(selectedMethod.intValue, methodNames.ToArray());
                successMethodName.stringValue = methodNames[selectedMethod.intValue];
            }
            else // If no valid behavior script exists
            {
                selectedMethod.intValue = 0;
                GUI.enabled = false;
                selectedMethod.intValue = EditorGUILayout.Popup(selectedMethod.intValue, emptyNames);
                GUI.enabled = true;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
            
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            GetProperties(property);
            float sum = 0;
            switch (transitionType.enumValueIndex)
            {
                case 0:
                    sum += EditorGUI.GetPropertyHeight(triggerEnterTags);
                    break;
                case 1:
                    sum += EditorGUI.GetPropertyHeight(collisionTags);
                    break;
                case 2:
                    sum += EditorGUI.GetPropertyHeight(timerLength);
                    break;
                case 3:
                    sum += EditorGUI.GetPropertyHeight(callId);
                    break;
                default:
                    break;
            }
            sum += EditorGUI.GetPropertyHeight(conditionEvaluation);
            sum += EditorGUI.GetPropertyHeight(transitionConditions);
            sum += EditorGUI.GetPropertyHeight(fromState);
            
            return sum / (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * 2);
        }
    }



    [CustomPropertyDrawer(typeof(ATTransitionCondition))]
    public class ATTransitionConditionDrawer : PropertyDrawer
    {
        private static float lineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        private ATStateMachine objref;
        private List<string> methodNames = new List<string>();

        private string[] emptyNames = new string[] { "Link behavior script to choose comparison method." };
        private int comparisonMethodIndex = 0;

        SerializedProperty conditionType;
        SerializedProperty tagsInTrigger;
        SerializedProperty tagsInCollision;
        SerializedProperty customComparison;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Calculate rects
            var typeRect = new Rect(position.x, position.y, position.width, lineHeight);
            var conditionalRect = new Rect(position.x, position.y + 30, position.width, position.height - lineHeight);

            // Get properties
            conditionType = property.FindPropertyRelative("conditionType");

            // Conditional properties
            tagsInTrigger = property.FindPropertyRelative("tagsInTrigger");
            tagsInCollision = property.FindPropertyRelative("tagsInCollision");
            customComparison = property.FindPropertyRelative("customComparisonMethod");

            // Get comparison method names
            try
            {
                objref = (ATStateMachine)property.serializedObject.targetObject;
                methodNames = objref.GetComponent<ATStateMachine>().GetComparisonMethodNames();
                if (methodNames.Count > 0)
                    methodNames.Insert(0, "None");
            }
            catch
            {
                objref = null;
                methodNames.Clear();
            }


            // Draw inspector
            EditorGUI.PropertyField(typeRect, conditionType);

            if (conditionType.enumValueIndex == 1) // AttributeInTrigger index
            {
                EditorGUI.PropertyField(conditionalRect, tagsInTrigger);
            }
            else if (conditionType.enumValueIndex == 2) // AttributeInCollision index
            {
                EditorGUI.PropertyField(conditionalRect, tagsInCollision);
            }
            else if (conditionType.enumValueIndex == 3) // CustomComparison index
            {
                if (methodNames.Count > 1) // If a valid behavior script exists
                {
                    comparisonMethodIndex = EditorGUI.Popup(conditionalRect, comparisonMethodIndex, methodNames.ToArray());
                    customComparison.stringValue = methodNames[comparisonMethodIndex];
                }
                else // If no valid behavior script exists
                {
                    comparisonMethodIndex = 0;
                    GUI.enabled = false;
                    comparisonMethodIndex = EditorGUI.Popup(conditionalRect, comparisonMethodIndex, emptyNames);
                    GUI.enabled = true;
                }
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            conditionType = property.FindPropertyRelative("conditionType");
            tagsInTrigger = property.FindPropertyRelative("tagsInTrigger");
            tagsInCollision = property.FindPropertyRelative("tagsInCollision");

            float extraLines = 0;

            if (conditionType.enumValueIndex == 1) // AttributeInTrigger index
            {
                extraLines += (1.5f + (tagsInTrigger.isExpanded ? Mathf.Max(tagsInTrigger.arraySize, 1) + 1.5f : 0));
            }
            else if (conditionType.enumValueIndex == 2) // AttributeInCollision index
            {
                extraLines += (1.5f + (tagsInCollision.isExpanded ? Mathf.Max(tagsInCollision.arraySize, 1) + 1.5f : 0));
            }
            else if (conditionType.enumValueIndex == 3) // CustomComparison index
            {
                extraLines += 1.5f;
            }

            return lineHeight + lineHeight * extraLines;
        }
    }


    // ATStateDrawer by Adam Cohen
    [CustomPropertyDrawer(typeof(ATState))]
    public class ATStateDrawer : PropertyDrawer
    {
        SerializedProperty state;
        ATStateMachineSettings settings = null;
        ATStateMachineSettings settings_cache = null;
        string[] options = null;

        void GetValues(SerializedProperty property)
        {
            try
            {
                settings = ((ATStateMachine)property.serializedObject.targetObject).GetSettings();
                if (options == null || settings_cache != settings)
                {
                    options = settings.GetStates().ToArray();
                    settings_cache = settings;
                }
            }
            catch { }
            state = property.FindPropertyRelative("state");
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GetValues(property);
            if (settings != null && state != null)
            {
                EditorGUI.BeginProperty(position, label, property);
                state.intValue = EditorGUI.Popup(position, state.intValue, options);
                EditorGUI.EndProperty();
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUI.LabelField(position, label.text);
                Rect halfpos = new Rect(position.x + position.width / 2, position.y, position.width / 2, position.height);
                state.intValue = EditorGUI.IntField(halfpos, state.intValue);
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}