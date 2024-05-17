using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.ShaderKeywordFilter;

namespace AtomicTools
{
    [CustomPropertyDrawer(typeof(ATStateTransition))]
    public class ATStateTransitionDrawer : PropertyDrawer
    {
        private static float lineHeight = EditorGUIUtility.singleLineHeight;

        string labelstr = " State Transition";
        string typestr = "[CHOOSE]";

        private ATStateMachine objref;
        private List<string> methodNames;
        private string[] emptyNames = new string[] { "Link behavior script to choose success method." };

        SerializedProperty transitionType;
        SerializedProperty transitionConditions;
        SerializedProperty transitionConditionEvaluation;
        SerializedProperty fromState;
        SerializedProperty toState;
        SerializedProperty successMethodName;

        SerializedProperty triggerEnterTags;
        SerializedProperty collisionTags;
        SerializedProperty timerLength;

        SerializedProperty menuOpen;
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
            transitionConditionEvaluation = property.FindPropertyRelative("transitionConditionEvaluation");
            fromState = property.FindPropertyRelative("fromState");
            toState = property.FindPropertyRelative("toState");
            successMethodName = property.FindPropertyRelative("successMethodName");
            menuOpen = property.FindPropertyRelative("menuOpen");
            selectedMethod = property.FindPropertyRelative("selectedMethod");

            // Conditional properties
            triggerEnterTags = property.FindPropertyRelative("triggerEnterTags");
            collisionTags = property.FindPropertyRelative("collisionTags");
            timerLength = property.FindPropertyRelative("timerLength");

            // Get names of behavior methods
            objref = (ATStateMachine)property.serializedObject.targetObject;
            methodNames = new List<string>(objref.GetComponent<ATStateMachine>().GetBehaviorMethodNames());
            methodNames.Insert(0, "None");

            // for formatting
            boldFoldout.fontStyle = FontStyle.Bold;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GetProperties(property);

            EditorGUI.BeginProperty(position, label, property);
            menuOpen.boolValue = EditorGUI.Foldout(new Rect(3, 3, position.width - 6, 15), menuOpen.boolValue, typestr + labelstr, boldFoldout);


            typestr = transitionType.enumDisplayNames[transitionType.enumValueIndex];

            if (menuOpen.boolValue)
            {
                // Draw inspector
                EditorGUILayout.PropertyField(transitionType);

                if (transitionType.enumValueIndex == 2) // TriggerEnterTag index
                    EditorGUILayout.PropertyField(triggerEnterTags);
                if (transitionType.enumValueIndex == 3) // CollisionTag index
                    EditorGUILayout.PropertyField(collisionTags);
                if (transitionType.enumValueIndex == 4) // Timer index
                    EditorGUILayout.PropertyField(timerLength);

                EditorGUILayout.PropertyField(transitionConditionEvaluation);
                EditorGUILayout.PropertyField(transitionConditions);
                EditorGUILayout.PropertyField(fromState);
                EditorGUILayout.PropertyField(toState);

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
            }
            EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(ATTransitionCondition))]
    public class ATTransitionConditionDrawer : PropertyDrawer
    {
        private static float lineHeight = EditorGUIUtility.singleLineHeight;

        private ATStateMachine objref;
        private List<string> methodNames;

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
            tagsInTrigger = property.FindPropertyRelative("attributesInTrigger");
            tagsInCollision = property.FindPropertyRelative("attributesInCollision");
            customComparison = property.FindPropertyRelative("customComparisonMethod");

            // Get behavior method names
            objref = (ATStateMachine)property.serializedObject.targetObject;
            methodNames = new List<string>(objref.GetComponent<ATStateMachine>().GetBehaviorMethodNames());
            methodNames.Insert(0, "None");

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
            tagsInTrigger = property.FindPropertyRelative("_tagsInTrigger");
            tagsInCollision = property.FindPropertyRelative("_tagsInCollision");

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

            return lineHeight + lineHeight * extraLines * 1.2f;
        }
    }
}