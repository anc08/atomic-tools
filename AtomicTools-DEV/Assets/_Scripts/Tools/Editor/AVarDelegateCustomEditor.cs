using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(AVarDelegate))]
public class AVarDelegatePropertyDrawer : PropertyDrawer
{
    SerializedProperty script;
    SerializedProperty method;
    SerializedProperty list;
    SerializedProperty index;

    private List<string> methodNames = new List<string>();
    private string[] emptyNames = new string[] {"Link behavior script to choose a method."};

    private void GetInfo(SerializedProperty property)
    {
        script = property.FindPropertyRelative("_script");
        method = property.FindPropertyRelative("_method");
        index = property.FindPropertyRelative("_selectedIndex");

        methodNames.Clear();
        list = property.FindPropertyRelative("_methodNames");
        for(int i = 0; i < list.arraySize; i++)
            methodNames.Add(list.GetArrayElementAtIndex(i).stringValue);
        methodNames.Insert(0,"None");
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        GetInfo(property);

        EditorGUI.indentLevel++;

        var origFontStyle = EditorStyles.label.fontStyle;
        EditorStyles.label.fontStyle = FontStyle.Bold;
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(script);
        if(EditorGUI.EndChangeCheck())
        {
            index.intValue = 0;
        }
        EditorStyles.label.fontStyle = origFontStyle;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Method");

        int newMethodIndex = index.intValue;
        if(methodNames.Count > 1)
        {
            EditorGUI.BeginChangeCheck();
            //method.stringValue = methodNames[methodIndex = EditorGUILayout.Popup(methodIndex, methodNames.ToArray())];
            newMethodIndex = EditorGUILayout.Popup(index.intValue, methodNames.ToArray());
            if(EditorGUI.EndChangeCheck())
            {
                index.intValue = newMethodIndex;
                method.stringValue = methodNames[newMethodIndex];
            }
                
        }
        else
        {
            index.intValue = 0;
            GUI.enabled = false;
            newMethodIndex = EditorGUILayout.Popup(index.intValue, emptyNames);
            GUI.enabled = true;
        }
        EditorGUILayout.EndHorizontal();
        EditorGUI.indentLevel--;

        EditorGUI.EndProperty();
    }
}

[CustomEditor(typeof(EditorMethodTest))]
public class EditorMethodTestCustomEditor : Editor
{
    SerializedProperty method;
    EditorMethodTest tester;

    void OnEnable()
    {
        method = serializedObject.FindProperty("methodToTest");
        tester = (EditorMethodTest)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(method);
        if(GUILayout.Button("Test Method"))
        {
            tester.TestCall();
        }

        serializedObject.ApplyModifiedProperties();
    }
}