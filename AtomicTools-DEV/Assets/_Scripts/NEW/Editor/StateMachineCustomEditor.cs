using UnityEngine;
using UnityEditor;
using NUnit.Framework;

namespace AtomicTools
{
    [CustomEditor(typeof(ATStateMachine))]
    public class StateMachineCustomEditor : Editor
    {
        ATStateMachine machine;
        ATStateMachineSettings settings_cache;

        SerializedProperty settings;
        SerializedProperty uniqueBehavior;
        SerializedProperty initOnAwake;
        SerializedProperty overrideStartingState;
        SerializedProperty startingState;
        SerializedProperty stateTransitions;

        private string[] emptySettings = new string[] { "Choose a settings asset." };
        private string[] noStartOverride = new string[] { "Enabled override to choose." };

        private void OnEnable()
        {
            machine = (ATStateMachine)target;
            settings_cache = machine.GetSettings();
            GetProperties();
        }

        private void GetProperties()
        {
            settings = serializedObject.FindProperty("_settings");
            uniqueBehavior = serializedObject.FindProperty("_uniqueBehavior");
            initOnAwake = serializedObject.FindProperty("_initTransitionsOnAwake");
            overrideStartingState = serializedObject.FindProperty("_overrideStartingState");
            startingState = serializedObject.FindProperty("_startingState");
            stateTransitions = serializedObject.FindProperty("_stateTransitions");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            // Check if settings have been changed
            if (settings_cache != machine.GetSettings())
            {
                GetProperties();
                settings_cache = machine.GetSettings();
            }

            EditorGUILayout.LabelField("SETTINGS", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Settings Asset");
            EditorGUILayout.ObjectField(settings.objectReferenceValue, typeof(ATStateMachineSettings), false);
            EditorGUILayout.EndHorizontal();

            if (settings.objectReferenceValue != null)
            {
                EditorGUILayout.PropertyField(uniqueBehavior);
                EditorGUILayout.PropertyField(initOnAwake);
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("STATE MACHINE", EditorStyles.boldLabel);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(overrideStartingState);
                if(overrideStartingState.boolValue)
                {
                    //EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(startingState);
                    //EditorGUI.indentLevel--;
                }
                else
                {
                    GUI.enabled = false;
                    EditorGUILayout.Popup(0, noStartOverride);
                    GUI.enabled= true;
                }
                EditorGUILayout.EndHorizontal();
                if (GUILayout.Button("Open Transitions Menu")) StateTransitionsWindow.ShowWindow(machine, stateTransitions);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}