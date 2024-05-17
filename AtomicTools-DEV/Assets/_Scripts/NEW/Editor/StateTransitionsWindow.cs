using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

namespace AtomicTools
{
    public class StateTransitionsWindow : EditorWindow
    {
        private static SerializedProperty transitionList;

        private static ATStateMachine sm;
        private static StateTransitionsWindow window = null;
        private Vector2 scrollPos = new Vector2(0,0);
        public static void ShowWindow(ATStateMachine machine, SerializedProperty transitions)
        {
            window = (StateTransitionsWindow)GetWindow(typeof(StateTransitionsWindow));
            window.minSize = new Vector2(300, 500);
            window.maxSize = new Vector2(600, 800);
            //sm = machine;
            //transitions = sm.GetTransitionsList();
            sm = machine;
            transitionList = transitions;
        }

        private void OnEnable()
        {
            
        }

        private void OnGUI()
        {
            try
            {
                EditorGUILayout.PropertyField(transitionList, true);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Save"))
                {
                    //sm.OverwriteTransitionsList(translist);
                    Debug.Log("SAVE");
                }
                if (GUILayout.Button("Exit"))
                {
                    Close();
                }
                GUILayout.EndHorizontal();
            }
            catch { Close(); }
        }
    }

}