using System.Collections.Generic;
using UnityEngine;

namespace AtomicTools
{
    /*
     * AtomicTools::ATStateTransition
     * Author: Adam Cohen
     * https://adamncohen08.wixsite.com/adam-cohen-game-dev
     * Data structure defining a possible transition between states in an ATStateMachine
     */
    [System.Serializable]
    public struct ATStateTransition
    {
        public TransitionType transitionType;

        [Header("CONDITIONS:")]
        [Tooltip("The rule under which to evaluate whether this transition can succeed.")]
        public TransitionConditionEvaluation conditionEvaluation;
        public List<ATTransitionCondition> transitionConditions;

        [Header("STATES:")]
        [Tooltip("States from which this transition can occur.")] public List<ATState> fromState;
        [Tooltip("The state that the object will be in when this transition occurs successfully.")] public ATState toState;

        // Optional fields
        [AtomicTools.TagSelector]
        [Tooltip("If list is empty, will work with all tags")] public List<string> triggerEnterTags;
        [TagSelector]
        [Tooltip("If list is empty, will work with all tags")] public List<string> collisionTags;
        [Tooltip("How long after switching to fromState the timer will attempt to use this transition")] public float timerLength;

        [Header("SUCCESS:")]
        [Tooltip("The name of the method in the unique behavior script to invoke when this transition successfully passes.")] public string successMethodName;


        /*
         * Methods
         */
        public bool EvaluateCanSwitch(ATStateMachine sourceref)
        {
            // check valid state
            if (!fromState.Contains(sourceref.GetCurrentState())) return false;

            // error trapping
            if (transitionConditions.Count <= 0)
                return true;

            // this looks confusing but it's not trust me
            foreach (ATTransitionCondition condition in transitionConditions)
            {
                if (conditionEvaluation == TransitionConditionEvaluation.AllTrue && !condition.EvaluateCondition(sourceref))
                    return false;
                else if (conditionEvaluation == TransitionConditionEvaluation.AnyTrue && condition.EvaluateCondition(sourceref))
                    return true;
            }
            return conditionEvaluation == TransitionConditionEvaluation.AllTrue;
        }

        // ONLY FOR EDITOR USAGE
        //public bool menuOpen;
        public int selectedMethod;
    }
}