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

        private string[] noStartOverride = new string[] { "Enable override to choose." };

        Rect _rect;

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
            initOnAwake = serializedObject.FindProperty("_initOnAwake");
            overrideStartingState = serializedObject.FindProperty("_overrideStartState");
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

            EditorGUILayout.Space(1);
            _rect = GUILayoutUtility.GetLastRect();
            _rect.x = EditorGUIUtility.labelWidth;
            _rect.width *= 0.6f;

            EditorGUILayout.LabelField("SETTINGS", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Settings Asset");
            _rect.height = GUILayoutUtility.GetLastRect().height;
            _rect.y = GUILayoutUtility.GetLastRect().y;
            settings.objectReferenceValue = EditorGUI.ObjectField(_rect, settings.objectReferenceValue, typeof(ATStateMachineSettings), false);
            EditorGUILayout.EndHorizontal();

            if (settings.objectReferenceValue != null)
            {
                EditorGUILayout.LabelField("Behavior Script");
                _rect.height = GUILayoutUtility.GetLastRect().height;
                _rect.y = GUILayoutUtility.GetLastRect().y;
                uniqueBehavior.objectReferenceValue = EditorGUI.ObjectField(_rect, uniqueBehavior.objectReferenceValue, typeof(ATStateMachineBehavior), true);
                EditorGUILayout.PropertyField(initOnAwake);
                if(initOnAwake.boolValue)
                {
                    EditorGUILayout.LabelField("NOTE: Initialization process may be significant in large systems.\nConsider initializing before entering play mode to reduce realtime operations", EditorStyles.wordWrappedMiniLabel);
                }
                EditorGUILayout.Space();
                EditorGUI.indentLevel--;
                EditorGUILayout.LabelField("STATE MACHINE", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(overrideStartingState);
                _rect.height = GUILayoutUtility.GetLastRect().height * 1.5f;
                _rect.y = GUILayoutUtility.GetLastRect().y;
                _rect.x += _rect.width * 0.1f;
                _rect.width *= 0.9f;
                if (overrideStartingState.boolValue)
                {
                    EditorGUI.PropertyField(_rect, startingState);
                }
                else
                {
                    GUI.enabled = false;
                    EditorGUI.Popup(_rect, 0, noStartOverride);
                    GUI.enabled= true;
                }
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.Space(30);
                _rect = GUILayoutUtility.GetLastRect();
                GUILayoutUtility.GetAspectRect(15f);
                _rect.y += _rect.height / 2f;
                _rect.width = 2 * (EditorGUIUtility.labelWidth + EditorGUIUtility.fieldWidth);
                
                if (GUI.Button(_rect, "Open Transition Editor")) StateTransitionsWindow.ShowWindow(machine, stateTransitions);
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}