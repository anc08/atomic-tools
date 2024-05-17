using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
/*
[CustomPropertyDrawer(typeof(StateTransition))]
public class StateTransitionDrawer : PropertyDrawer
{
    string labelstr = " State Transition";
    string typestr = "[CHOOSE]";

    private DeathCause objref;
    private List<string> methodNames;
    private string[] emptyNames = new string[] {"Link behavior script to choose success method."};

    private static float lineHeight = EditorGUIUtility.singleLineHeight;

    SerializedProperty transitionType;
    SerializedProperty transitionConditions;
    SerializedProperty transitionConditionEvaluation;
    SerializedProperty fromState;
    SerializedProperty toState;
    SerializedProperty successMethodName;
    SerializedProperty killImmediate;
    SerializedProperty deathReport;
    SerializedProperty reportTask;
    SerializedProperty taskReport;

    SerializedProperty addAttribute;
    SerializedProperty removeAttribute;
    SerializedProperty attributeToAdd;
    SerializedProperty attributeToRemove;

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
        killImmediate = property.FindPropertyRelative("triggerDeathImmediately");
        deathReport = property.FindPropertyRelative("deathReport");
        reportTask = property.FindPropertyRelative("triggerTaskCompletion");
        taskReport = property.FindPropertyRelative("taskCompletionReport");
        menuOpen = property.FindPropertyRelative("menuOpen");
        selectedMethod = property.FindPropertyRelative("selectedMethod");
        addAttribute = property.FindPropertyRelative("addAttribute");
        removeAttribute = property.FindPropertyRelative("removeAttribute");
        attributeToAdd = property.FindPropertyRelative("attributeToAdd");
        attributeToRemove = property.FindPropertyRelative("attributeToRemove");

        // Conditional properties
        triggerEnterTags = property.FindPropertyRelative("triggerEnterTags");
        collisionTags = property.FindPropertyRelative("collisionTags");
        timerLength = property.FindPropertyRelative("timerLength");

        // Get names of behavior methods
        objref = (DeathCause)property.serializedObject.targetObject;
        methodNames = new List<string>(objref.GetComponent<DeathCause>().GetBehaviorMethodNames());
        methodNames.Insert(0,"None");

        // for formatting
        boldFoldout.fontStyle = FontStyle.Bold;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GetProperties(property);

        EditorGUI.BeginProperty(position, label, property);
        menuOpen.boolValue = EditorGUI.Foldout(new Rect(3, 3, position.width - 6, 15), menuOpen.boolValue, typestr + labelstr, boldFoldout);
        
        
        typestr = transitionType.enumDisplayNames[transitionType.enumValueIndex];

        if(menuOpen.boolValue)
        {
            // Draw inspector
            EditorGUILayout.PropertyField(transitionType);

            if(transitionType.enumValueIndex == 2) // TriggerEnterTag index
                EditorGUILayout.PropertyField(triggerEnterTags);
            if(transitionType.enumValueIndex == 3) // CollisionTag index
                EditorGUILayout.PropertyField(collisionTags);
            if(transitionType.enumValueIndex == 4) // Timer index
                EditorGUILayout.PropertyField(timerLength);

            EditorGUILayout.PropertyField(transitionConditionEvaluation);
            EditorGUILayout.PropertyField(transitionConditions);
            EditorGUILayout.PropertyField(fromState);
            EditorGUILayout.PropertyField(toState);
            
            // SUCCESS
            EditorGUILayout.PropertyField(killImmediate);
            if(killImmediate.boolValue) // Display death report field
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(deathReport);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.PropertyField(reportTask);
            if(reportTask.boolValue) // Display task report field
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(taskReport);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.PropertyField(removeAttribute);
            if(removeAttribute.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(attributeToRemove);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.PropertyField(addAttribute);
            if (addAttribute.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(attributeToAdd);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Success Method");
            if(methodNames.Count > 1) // If a valid behavior script exists
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

[CustomPropertyDrawer(typeof(TransitionCondition))]
public class TransitionConditionDrawer : PropertyDrawer
{
    private static float lineHeight = EditorGUIUtility.singleLineHeight;

    private DeathCause objref;
    private List<string> methodNames;
    private string[] emptyNames = new string[] { "Link behavior script to choose comparison method." };
    private int comparisonMethodIndex = 0;

    SerializedProperty conditionType;
    SerializedProperty playerHoldingAttributes;
    SerializedProperty attributesInTrigger;
    SerializedProperty attributesInCollision;
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
        playerHoldingAttributes = property.FindPropertyRelative("playerHoldingAttributes");
        attributesInTrigger = property.FindPropertyRelative("attributesInTrigger");
        attributesInCollision = property.FindPropertyRelative("attributesInCollision");
        customComparison = property.FindPropertyRelative("customComparisonMethod");

        // Get behavior method names
        objref = (DeathCause)property.serializedObject.targetObject;
        methodNames = new List<string>(objref.GetComponent<DeathCause>().GetBehaviorMethodNames());
        methodNames.Insert(0, "None");

        // Draw inspector
        EditorGUI.PropertyField(typeRect, conditionType);

        if(conditionType.enumValueIndex == 1) // PlayerHoldingAttribute index
        {
            EditorGUI.PropertyField(conditionalRect, playerHoldingAttributes);
        }
        else if(conditionType.enumValueIndex == 2) // AttributeInTrigger index
        {
            EditorGUI.PropertyField(conditionalRect, attributesInTrigger);
        }
        else if(conditionType.enumValueIndex == 3) // AttributeInCollision index
        {
            EditorGUI.PropertyField(conditionalRect, attributesInCollision);
        }
        else if(conditionType.enumValueIndex == 6) // CustomComparison index
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
        playerHoldingAttributes = property.FindPropertyRelative("playerHoldingAttributes");
        attributesInTrigger = property.FindPropertyRelative("attributesInTrigger");
        attributesInCollision = property.FindPropertyRelative("attributesInCollision");

        float extraLines = 0;

        if(conditionType.enumValueIndex == 1) // PlayerHoldingAttribute index
        {
            extraLines += (1.5f + (playerHoldingAttributes.isExpanded ? Mathf.Max(playerHoldingAttributes.arraySize, 1) + 1.5f : 0));
        }
        else if(conditionType.enumValueIndex == 2) // AttributeInTrigger index
        {
            extraLines += (1.5f + (attributesInTrigger.isExpanded ? Mathf.Max(attributesInTrigger.arraySize, 1) + 1.5f : 0));
        }
        else if (conditionType.enumValueIndex == 3) // AttributeInCollision index
        {
            extraLines += (1.5f + (attributesInCollision.isExpanded ? Mathf.Max(attributesInCollision.arraySize, 1) + 1.5f : 0));
        }
        else if(conditionType.enumValueIndex == 6) // CustomComparison index
        {
            extraLines += 1.5f;
        }

        return lineHeight + lineHeight * extraLines * 1.2f;
    }
}

[CustomEditor(typeof(TransitionInitializer))]
public class TransitionInitializerCustomEditor : Editor
{
    TransitionInitializer initializer;

    void OnEnable()
    {
        initializer = (TransitionInitializer)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Initialize Transition Data In DeathCause Instances", EditorStyles.boldLabel);
        if(GUILayout.Button("Initialize This Instance"))
        {
            if(!initializer.InitializeTransitionsHere())
                Debug.Log("Init Failed");
        }
        if(GUILayout.Button("Initialize All Instances In Scene"))
        {
            initializer.InitializeAllTransitionsInScene();
        }
    }
}*/