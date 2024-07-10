using AtomicTools;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/*
 * AtomicTools::ATStateMachineBehavior
 * Author: Adam Cohen
 * https://adamncohen08.wixsite.com/adam-cohen-game-dev
 * Base class. All unique behavior scripts must inherit from this.
 * Override basic functionality, and add additional public functions to be used as completion or comparison checks.
 */
public abstract class ATStateMachineBehavior : MonoBehaviour
{
    protected ATStateMachine _stateMachine;
    public void SetMachineRef(ATStateMachine machine) { _stateMachine = machine; }
    public virtual void InvokeBehavior(string methodName) { if (methodName != "None") Invoke(methodName, 0f); }

    public virtual bool InvokeComparison(string methodName)
    {
        if (methodName == "None") return false;
        return (bool)typeof(ATStateMachineBehavior).InvokeMember(methodName, BindingFlags.InvokeMethod, null, this, new object[] { });
    }
    public virtual void OnStateSwitch(ATState state) { }
}
