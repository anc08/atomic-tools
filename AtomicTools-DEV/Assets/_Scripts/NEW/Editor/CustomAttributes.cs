using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using AtomicTools;
using NUnit.Framework;


//Original by DYLAN ENGELMAN http://jupiterlighthousestudio.com/custom-inspectors-unity/
//Altered by Brecht Lecluyse https://www.brechtos.com
[CustomPropertyDrawer(typeof(AtomicTools.TagSelectorAttribute))]
public class TagSelectorPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.String)
        {
            EditorGUI.BeginProperty(position, label, property);
 
            var attrib = this.attribute as AtomicTools.TagSelectorAttribute;
 
            if (attrib.UseDefaultTagFieldDrawer)
            {
                property.stringValue = EditorGUI.TagField(position, label, property.stringValue);
            }
            else
            {
                //generate the taglist + custom tags
                List<string> tagList = new List<string>();
                tagList.Add("<No Tag>");
                tagList.AddRange(UnityEditorInternal.InternalEditorUtility.tags);
                string propertyString = property.stringValue;
                int index = -1;
                if(propertyString =="")
                {
                    //The tag is empty
                    index = 0; //first index is the special <notag> entry
                }
                else
                {
                    //check if there is an entry that matches the entry and get the index
                    //we skip index 0 as that is a special custom case
                    for (int i = 1; i < tagList.Count; i++)
                    {
                        if (tagList[i] == propertyString)
                        {
                            index = i;
                            break;
                        }
                    }
                }
                
                //Draw the popup box with the current selected index
                index = EditorGUI.Popup(position, label.text, index, tagList.ToArray());
 
                //Adjust the actual string value of the property based on the selection
                if(index==0)
                {
                    property.stringValue = "";
                }
                else if (index >= 1)
                {
                    property.stringValue = tagList[index];
                }
                else
                {
                    property.stringValue = "";
                }
            }
 
            EditorGUI.EndProperty();
        }
        else
        {
            EditorGUI.PropertyField(position, property, label);
        }
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
        if(settings != null && state != null)
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