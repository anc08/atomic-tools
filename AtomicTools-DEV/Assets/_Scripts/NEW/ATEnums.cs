using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * AtomicTools::ATEnums
 * Author: Adam Cohen
 * https://adamncohen08.wixsite.com/adam-cohen-game-dev
 * Extra data used in the ATStateMachine modular state machine system.
 */
namespace AtomicTools
{
    // What type of event causes a state transition to trigger
    public enum TransitionType
    {
        TriggerEnterTag,
        CollisionTag,
        Timer,
        Hook
    };

    // Types of transition conditions
    public enum ConditionType
    {
        CHOOSE,
        TagInTrigger,
        TagInCollision,
        CustomComparison
    };

    // The rule under which to evaluate whether a transition can succeed
    public enum TransitionConditionEvaluation
    {
        AnyTrue,
        AllTrue,
        AnyFalse,
        AllFalse
    };

    [System.Serializable]
    public struct ATState
    {
        public ATStateMachineSettings settings;
        public int state;
    }
}