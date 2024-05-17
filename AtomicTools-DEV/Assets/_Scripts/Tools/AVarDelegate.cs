using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

///
/// Custom data type
/// Author: Adam Cohen
/// Serializes in editor, allows you to drag in a script and choose a method from the dropdown
/// Currently can only grab from classes that inherit from MonoBehaviour.
/// Currently can't get return types.
/// Currently can't pass in arguments.
/// Method will be invoked by calling AVarDelegate.CallMethod()
//

[System.Serializable]
public class AVarDelegate : ISerializationCallbackReceiver 
{
    [Tooltip("Script of the defined type or one that inherits it")]
    [SerializeField] private MonoBehaviour _script;
    [Tooltip("Method in script to be called")]
    [SerializeField] private string _method;

    [SerializeField] private int _selectedIndex = 0;
    [SerializeField] private List<string> _methodNames = new List<string>();

    // Refresh list when it changes
    void OnValidate()
    {
        DefineMethodNames();
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize () => this.OnValidate();
    void ISerializationCallbackReceiver.OnAfterDeserialize () {}

    // Get method names and populate list
    private void DefineMethodNames()
    {
        _methodNames.Clear();
        if(_script == null)
            return;

        Type t = _script.GetType();
        MethodInfo[] info = t.GetMethods(BindingFlags.Instance|BindingFlags.Public|BindingFlags.DeclaredOnly);
        _methodNames = new List<string>();
        foreach(MethodInfo m in info)
        {
            _methodNames.Add(m.Name);
        }
        //Debug.Log(String.Join(", ", _methodNames));
    }

    // Get list of method names. Not recommended to be used outside of editor
    public List<string> GetBehaviorMethodNames()
    {
        if(_methodNames == null)
            return new List<string>();
        return _methodNames;
    }

    /// <summary>
    /// Call method
    /// </summary>
    /// <returns> True if the method was successfully called. </returns>
    public void CallMethod()
    {
        if(_script == null || _method == null)
            return;
        Type t = _script.GetType();
        _script.Invoke(_method, 0);
        //return _del();
    }

    /// <summary>
    /// Set method to a new method within the type
    /// </summary>
    /// <param name="newMethod"> The name of the new method to be assigned. </param>
    public void SetMethod(string newMethod)
    {
        if(newMethod != "None" && !_methodNames.Contains(newMethod))
        {
            Debug.LogError(newMethod + ": Invalid method name in variable delegate " + this + " for type " + _script);
            return;
        }

        if(newMethod != "None")
        {
            _method = newMethod;
        }
    }

    /// <summary>
    /// Set method to a new method within the type
    /// </summary>
    /// <param name="index"> The index of the new method to be assigned. </param>
    public void SetMethod(int index)
    {
        if(index < _methodNames.Count)
        {
            _selectedIndex = index;
            _method = _methodNames[index];
        }
    }

    /// <summary>
    /// Get a string representation of the currently assigned method
    /// </summary>
    /// <returns> Format type.methodName </returns>
    public string CurrentlyAssignedMethod()
    {
        return _script + "." + _method;
    }
}