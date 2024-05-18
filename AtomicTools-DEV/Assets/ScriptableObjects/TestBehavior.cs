using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBehavior : ATStateMachineBehavior
{
    public void Test()
    {
        Debug.Log("TEST");
    }

    public bool BoolTest()
    {
        return true;
    }
}
