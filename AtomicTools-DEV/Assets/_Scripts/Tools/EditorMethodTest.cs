using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Demo class to showcase how easy AVarDelegate hopefully is to use
/// Has custom editor GUI with a button that will execute TestCall() on click
/// </summary>
public class EditorMethodTest : MonoBehaviour
{
    [SerializeField] AVarDelegate methodToTest;

    public void TestCall()
    {
        methodToTest?.CallMethod();
    }
}
