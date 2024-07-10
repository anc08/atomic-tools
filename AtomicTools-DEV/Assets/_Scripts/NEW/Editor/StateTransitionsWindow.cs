using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AtomicTools
{
    /* 
     * AtomicTools::StateTransitionsWindow
     * Author: Adam Cohen
     * https://adamncohen08.wixsite.com/adam-cohen-game-dev
     * Editor window to provide an interface for setting up state transitions
     */
    public class StateTransitionsWindow : EditorWindow
    {
        private static string[] listnames = { "Trigger Enter Transition", "Collision Enter Transition", "Timer Transition", "Call ID Transition" };

        private static SerializedProperty transitionsProperty;
        private static SerializedObject serializedObject = null;
        private static SerializedProperty objProperty;
        private static StateTransitionsWindow window = null;
        List<string> transitionNames = new List<string>();

        ATStateMachine sm = null;

        private Vector2 scrollPos = new Vector2(0,0);
        private Vector2 dataScrollPos = new Vector2(0,0);
        private Rect windowpos;
        private Rect listview;
        private Rect listscrollview;
        private Rect listscrollrect;
        private Rect listbuttonsview;
        private Rect dataview;
        private Rect datascrollview;
        private Rect datascrollrect;
        private Rect buttonsview;

        private static Texture2D buttonsTex;
        private static Texture2D selectedTex;

        private static Color buttonsClr = new Color(45/255f, 45/255f, 45/255f, 1f);
        private static Color selectedClr = new Color(195/255f, 220/255f, 220/255f, 1f);

        static GUIStyle listStyle = null;
        static GUIStyle selectedStyle = null;

        int selected = -1;

        public static void ShowWindow(ATStateMachine machine)
        {
            if (listStyle == null || selectedStyle == null)
            {
                InitTextures();
                InitStyles();
            }
            window = GetWindow<StateTransitionsWindow>("Transition Editor");
            window.minSize = new Vector2(600, 300);
            serializedObject = new SerializedObject(machine as Object);
            GetData();
        }

        void OnEnable()
        {
            GetData();
        }

        private void OnDisable()
        {
            transitionsProperty = null;
            serializedObject = null;
            objProperty = null;
            window = null;
            transitionNames = new List<string>();
        }

        private static void InitTextures()
        {
            buttonsTex = new Texture2D(1, 1);
            buttonsTex.SetPixel(0, 0, buttonsClr);
            buttonsTex.Apply();

            selectedTex = new Texture2D (1, 1);
            selectedTex.SetPixel (0, 0, selectedClr);
            selectedTex.Apply();
        }

        private static void InitStyles()
        {
            listStyle = new GUIStyle(EditorStyles.toolbarButton);
            listStyle.alignment = TextAnchor.MiddleLeft;
            selectedStyle = new GUIStyle(listStyle);
            selectedStyle.normal.background = selectedTex;
        }

        private void OnGUI()
        {
            DrawLayouts();
            try
            {
                //GetData();          // get transitions list
                DrawList();         // buttons to select which element of the list is being edited
                DrawListButtons();  // add/remove, move up/down buttons
                DrawData();         // display currently selected transition
                DrawButtons();      // save/exit buttons
            }
            catch { Close(); }
        }

        static void GetData()
        {
            if (serializedObject != null)
            {
                objProperty = serializedObject.FindProperty("_stateTransitions");
                transitionsProperty = objProperty.Copy();
            }
        }

        void DrawLayouts()
        {
            if(window == null) { Close(); return; }
            windowpos = window.position;

            // overall box for left side column
            listview.width = EditorGUIUtility.currentViewWidth * 0.35f; 
            listview.height = windowpos.height - 90;
            listview.x = 0;
            listview.y = 0;

            // available scrollview rect for list column
            float singleLine = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            listscrollrect.width = listview.width;
            listscrollrect.height = listview.height - singleLine;
            listscrollrect.x = listview.x;
            listscrollrect.y = singleLine;

            // actual needed area for list column
            listscrollview.width = listview.width;
            listscrollview.height = transitionNames.Count * singleLine;
            listscrollview.x = 0;
            listscrollview.y = singleLine;

            // box for Add/Delete/Move up/down buttons
            listbuttonsview.width = listview.width;
            listbuttonsview.height = 40;
            listbuttonsview.x = 0;
            listbuttonsview.y = windowpos.height - 90;

            dataview.width = EditorGUIUtility.currentViewWidth * 0.67f;
            dataview.height = windowpos.height - 50;
            dataview.x = listview.x + listview.width;
            dataview.y = listview.y;

            // available scrollview rect for data column
            datascrollrect.width = dataview.width * 0.95f;
            datascrollrect.height = dataview.height - singleLine;
            datascrollrect.x = 10;
            datascrollrect.y = 0;

            // needed area for data column
            datascrollview.width = dataview.width * 0.9f;
            datascrollview.height = selected >= 0 ? EditorGUI.GetPropertyHeight(transitionsProperty.GetArrayElementAtIndex(selected), true) * singleLine * 1.5f : 0;
            datascrollview.x = 0;
            datascrollview.y = 0;

            // bottom bar for Save/Exit buttons
            buttonsview.width = EditorGUIUtility.currentViewWidth;
            buttonsview.height = 50 - EditorGUIUtility.standardVerticalSpacing;
            buttonsview.x = 0;
            buttonsview.y = windowpos.height - 50;

            GUI.DrawTexture(buttonsview, buttonsTex);
            GUI.DrawTexture(dataview, buttonsTex);
        }

        void DrawList()
        {
            DefineNamesList();

            GUILayout.BeginArea(listview);

            EditorGUILayout.LabelField("TRANSITIONS", EditorStyles.boldLabel);
            scrollPos = GUI.BeginScrollView(listscrollrect, scrollPos, listscrollview, false, false, GUIStyle.none, GUI.skin.GetStyle("verticalScrollbar"));
            EditorGUI.indentLevel++;
            for (int i = 0; i < transitionNames.Count; i++)
            {
                if(GUILayout.Button(transitionNames[i], (selected == i ? selectedStyle : listStyle))) //EditorStyles.toolbarButton
                {
                    selected = i;
                }
            }
            EditorGUI.indentLevel--;
            GUI.EndScrollView();

            GUILayout.EndArea();
        }

        void DrawListButtons()
        {
            GUILayout.BeginArea(listbuttonsview);

            GUILayout.BeginHorizontal();
            GUI.enabled = selected > 0;
            if (GUILayout.Button("Move Up"))
            {
                MoveSelectedUp();
            }
            GUI.enabled = selected < transitionsProperty.arraySize - 1;
            if (GUILayout.Button("Move Down"))
            {
                MoveSelectedDown();
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if(GUILayout.Button("Add"))
            {
                selected = transitionsProperty.arraySize;
                transitionsProperty.InsertArrayElementAtIndex(selected);
            }
            if (selected < 0)
                GUI.enabled = false;
            if (GUILayout.Button("Delete"))
            {
                if(selected >= 0)
                {
                    transitionsProperty.DeleteArrayElementAtIndex(selected);
                    if (selected >= transitionsProperty.arraySize)
                        selected--;
                }
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        void DrawData()
        {
            GUILayout.BeginArea(dataview);
            dataScrollPos = GUI.BeginScrollView(datascrollrect, dataScrollPos, datascrollview, false, false, GUIStyle.none, GUI.skin.GetStyle("verticalScrollbar"));
            if (selected >= 0)
            {
                Rect area = new Rect(datascrollview);
                area.height = datascrollrect.height;
                GUILayout.BeginArea(area);
                EditorGUILayout.PropertyField(transitionsProperty.GetArrayElementAtIndex(selected));
                GUILayout.EndArea();
            }
            GUI.EndScrollView();
            GUILayout.EndArea();
        }

        void DrawButtons()
        {
            GUILayout.BeginArea(buttonsview);

            GUILayout.BeginHorizontal();
            Rect b1 = GUILayoutUtility.GetAspectRect(6f);
            if (GUI.Button(b1, "SAVE"))
            {
                if(sm == null)
                    sm = serializedObject.targetObject as ATStateMachine;
                sm.OverwriteTransitionsList(transitionsProperty);
                Debug.Log("TRANSITIONS SAVED: " + serializedObject.targetObject.name);
            }
            Rect b2 = GUILayoutUtility.GetAspectRect(6f);
            if (GUI.Button(b2, "EXIT"))
            {
                Close();
            }
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        void DefineNamesList()
        {
            SerializedProperty property;
            int type;
            transitionNames.Clear();
            var i = transitionsProperty.GetEnumerator();
            while (i.MoveNext())
            {
                property = i.Current as SerializedProperty;
                type = property.FindPropertyRelative("transitionType").intValue;
                transitionNames.Add(listnames[type]);
            }
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
    }

}