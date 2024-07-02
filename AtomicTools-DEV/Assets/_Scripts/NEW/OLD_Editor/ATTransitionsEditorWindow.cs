using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using AtomicTools;

public class TransitionsWindowData : Object
{
    public List<ATStateTransition> _stateTransitions = new List<ATStateTransition>();
    public List<string> _transitionNames = new List<string>();
}

namespace AtomicTools
{
    /* 
     * AtomicTools::ATTransitionsEditorWindow
     * Author: Adam Cohen
     * https://adamncohen08.wixsite.com/adam-cohen-game-dev
     * Editor window to provide an interface for setting up state transitions
     */
    public class ATTransitionsEditorWindow : EditorWindow
    {
        private static string[] listnames = { "Trigger Enter Transition", "Collision Enter Transition", "Timer Transition", "Hook Transition" };
        private static ATTransitionsEditorWindow window = null;
        public VisualTreeAsset visualTree = null;
        //private static ATStateMachine _machine;
        private static SerializedObject serializedObject;
        private static ATStateMachine originalObj;
        private static SerializedProperty transitionsProperty;
        private static TransitionsWindowData data;

        int selected = -1;

        public static void ShowWindow(ATStateMachine machine)
        {
            window = GetWindow<ATTransitionsEditorWindow>("Transition Editor");
            window.minSize = new Vector2(600, 300);
            window.Show();

            data = new TransitionsWindowData();
            data._stateTransitions = machine.GetTransitionsList();
            originalObj = machine;
            serializedObject = new SerializedObject(data as Object);
            transitionsProperty = serializedObject.FindProperty("_stateTransitions");
        }

        private void CreateGUI()
        {
            DefineNamesList();
            // add in all UI builder stuff
            visualTree.CloneTree(rootVisualElement);
            VisualElement listelement = rootVisualElement.Query("TransitionsList");
            listelement.Bind(serializedObject);
        }

        static void DefineNamesList()
        {
            SerializedProperty property;
            int type;
            data._transitionNames.Clear();
/*            foreach (ATStateTransition t in _data._stateTransitions)
            {
                _data._transitionNames.Add(listnames[(int)t.transitionType]);
            }*/

            var i = transitionsProperty.GetEnumerator();
            while (i.MoveNext())
            {
                property = i.Current as SerializedProperty;
                type = property.FindPropertyRelative("transitionType").intValue;
                data._transitionNames.Add(listnames[type]);
            }
        }

        void AddTransition()
        {
            selected = transitionsProperty.arraySize;
            transitionsProperty.InsertArrayElementAtIndex(selected);
            DefineNamesList();
        }

        void RemoveTransition()
        {
            if (selected >= 0)
            {
                transitionsProperty.DeleteArrayElementAtIndex(selected);
                if (selected >= transitionsProperty.arraySize)
                    selected--;
            }
            DefineNamesList();
        }

        void MoveSelectedUp()
        {
            transitionsProperty.MoveArrayElement(selected, selected - 1);
            selected--;
        }

        void MoveSelectedDown()
        {
            transitionsProperty.MoveArrayElement(selected, selected + 1);
            selected++;
        }

        void Save()
        {
            originalObj.OverwriteTransitionsList(transitionsProperty);
        }
    }
}