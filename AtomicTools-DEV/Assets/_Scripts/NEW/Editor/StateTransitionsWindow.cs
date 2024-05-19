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
        private static string[] listnames = { "Trigger Enter Transition", "Collision Enter Transition", "Timer Transition", "Hook Transition" };

        private static SerializedProperty transitionsProperty;
        private static SerializedObject serializedObject;
        private SerializedProperty objProperty;
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

        public static void ShowWindow(ATStateMachine machine, SerializedProperty property)
        {
            if (listStyle == null || selectedStyle == null)
            {
                InitTextures();
                InitStyles();
            }
            window = GetWindow<StateTransitionsWindow>("Transition Editor");
            window.minSize = new Vector2(600, 300);
            serializedObject = new SerializedObject(machine as Object);
        }

        void OnEnable()
        {
            
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
                GetData();
                DrawList();
                DrawListButtons();
                DrawData();
                DrawButtons();
            }
            catch { Close(); }
        }

        void GetData()
        {
            objProperty = serializedObject.FindProperty("_stateTransitions");
            transitionsProperty = objProperty.Copy();
        }

        void DrawLayouts()
        {
            if(window == null) { Close(); return; }
            windowpos = window.position;

            listview.width = EditorGUIUtility.currentViewWidth * 0.35f;
            listview.height = windowpos.height - 90;
            listview.x = 0;
            listview.y = 0;


            float dif = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            listscrollrect.width = listview.width;
            listscrollrect.height = listview.height - dif;
            listscrollrect.x = listview.x;
            listscrollrect.y = dif;

            listscrollview.width = listview.width;
            listscrollview.height = transitionNames.Count * dif;
            listscrollview.x = 0;
            listscrollview.y = dif;

            listbuttonsview.width = listview.width;
            listbuttonsview.height = 40;
            listbuttonsview.x = 0;
            listbuttonsview.y = windowpos.height - 90;

            dataview.width = EditorGUIUtility.currentViewWidth * 0.65f;
            dataview.height = windowpos.height - 50;
            dataview.x = EditorGUIUtility.currentViewWidth * 0.35f;
            dataview.y = 0;

            datascrollrect.width = EditorGUIUtility.currentViewWidth * 0.6f;
            datascrollrect.height = dataview.height - dif;
            datascrollrect.x = EditorGUIUtility.currentViewWidth * 0.375f;
            datascrollrect.y = 0;

            datascrollview.width = datascrollrect.width;
            datascrollview.height = selected >= 0 ? EditorGUI.GetPropertyHeight(transitionsProperty.GetArrayElementAtIndex(selected)) : 0;
            datascrollview.x = datascrollrect.x;
            datascrollview.y = 0;

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
            GUILayout.BeginArea(datascrollrect);
            //dataScrollPos = GUI.BeginScrollView(datascrollrect, dataScrollPos, datascrollview, false, false, GUIStyle.none, GUI.skin.GetStyle("verticalScrollbar"));

            if (selected >= 0)
                EditorGUILayout.PropertyField(transitionsProperty.GetArrayElementAtIndex(selected));

            //GUI.EndScrollView();
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