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

    /// <summary>
    /// Class <c>ATTransitionCondition</c> defines a condition for an ATStateTransition to check when evaluating its ability to pass.
    /// </summary>
    [System.Serializable]
    public struct ATTransitionCondition
    {
        public ConditionType conditionType;
        [TagSelector]
        [Tooltip("An object with one of these tags must be within the trigger for the condition to pass")] public List<string> tagsInTrigger;
        [TagSelector]
        [Tooltip("An object with one of these tags must be in collision for the condition to pass")] public List<string> tagsInCollision;
        [TagSelector]
        [Tooltip("The method that performs the comparison. Must return a boolean with whether the comparison passed.")] public string customComparisonMethod;

        public bool EvaluateCondition(ATStateMachine sourceref)
        {
            switch (conditionType)
            {
                case ConditionType.TagInTrigger:
                    return sourceref.FindTagsInTrigger(tagsInTrigger) != null;
                case ConditionType.TagInCollision:
                    return sourceref.FindTagsInCollision(tagsInCollision) != null;
                case ConditionType.CustomComparison:
                    return sourceref.CustomComparison(customComparisonMethod);
                default:
                    Debug.Log("No condition chosen or invalid condition type; evaluation default to TRUE");
                    return true;
            }
        }

        public override string ToString()
        {
            string str = "";
            switch (conditionType)
            {
                case ConditionType.TagInTrigger:
                    str += "One of these tags in trigger: ";
                    if (tagsInTrigger == null || tagsInTrigger.Count == 0)
                    {
                        str += "NONE DEFINED";
                    }
                    else
                    {
                        str += tagsInTrigger[0];
                        for (int i = 1; i < tagsInTrigger.Count; i++)
                        {
                            str += ", " + tagsInTrigger[i];
                        }
                    }    
                    break;

                case ConditionType.TagInCollision:
                    str += "One of these tags in collision: ";
                    if (tagsInCollision == null || tagsInCollision.Count == 0)
                    {
                        str += "NONE DEFINED";
                    }
                    else
                    {
                        str += tagsInCollision[0];
                        for (int i = 1; i < tagsInCollision.Count; i++)
                        {
                            str += ", " + tagsInCollision[i];
                        }
                    }
                    break;

                case ConditionType.CustomComparison:
                    str += "Custom comparison: behavior method " + customComparisonMethod;
                    break;

                default:
                    break;
            }

            return str;
        }
    }
}