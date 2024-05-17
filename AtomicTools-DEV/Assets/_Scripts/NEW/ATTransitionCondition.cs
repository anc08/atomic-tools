using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtomicTools
{
    /*
     * AtomicTools::ATTransitionCondition
     * Author: Adam Cohen
     * https://adamncohen08.wixsite.com/adam-cohen-game-dev
     * Data structure defining a condition to determine whether a transition passes or fails in an ATStateMachine
     */
    [System.Serializable]
    public struct ATTransitionCondition
    {
        public ConditionType conditionType;
        //[TagSelector]
        [Tooltip("An object with one of these tags must be within the trigger for the condition to pass")] public List<string> tagsInTrigger;
        //[TagSelector]
        [Tooltip("An object with one of these tags must be in collision for the condition to pass")] public List<string> tagsInCollision;
        //[TagSelector]
        [Tooltip("The method that performs the comparison. Must return a boolean with whether the comparison passed.")] public string customComparisonMethod;

        public bool EvaluateCondition(ATStateMachine sourceref)
        {
            switch (conditionType)
            {
                case ConditionType.CHOOSE:
                    Debug.Log("No condition; evaluation default to TRUE");
                    return true;
                case ConditionType.TagInTrigger:
                    return sourceref.FindTagsInTrigger(tagsInTrigger) != null;
                case ConditionType.TagInCollision:
                    return sourceref.FindTagsInCollision(tagsInCollision) != null;
                case ConditionType.CustomComparison:
                    Debug.Log("Transition condition " + conditionType + ": functionality not implemented, defaulting to false");
                    break;
                default:
                    Debug.Log("Invalid condition type");
                    break;
            }
            return false;
        }
    }
}