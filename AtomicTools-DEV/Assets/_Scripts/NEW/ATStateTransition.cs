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

    /// <summary>
    /// Struct <c>ATStateTransition</c> describes a set of conditions for an ATStateMachine to transition from one state to another.
    /// </summary>
    [System.Serializable]
    public struct ATStateTransition
    {
        private ATStateMachine _machine;
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
        [Tooltip("The ID that will be compared with when the state machine's CallIDTrigger function is called")] public string callId;

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

        /// <summary>
        /// Method <c>AttachToMachine</c> sets a connection between an ATStateTransition struct and its parent machine.
        /// </summary>
        /// <param name="machine"></param>
        public void AttachToMachine(ATStateMachine machine)
        {
            _machine = machine;
        }

        /// <summary>
        /// Method <c>GetMachine</c> gets the machine this transition is attached to.
        /// </summary>
        /// <returns>The associated ATStateMachine object</returns>
        public ATStateMachine GetMachine()
        {
            return _machine;
        }

        public override string ToString()
        {
            string str = "ATStateTransition - " + transitionType.ToString();
            str += "\nFrom State=" + fromState;
            str += "\nTo State=" + toState;
            str += "\nTriggered By=";

            switch (transitionType)
            {
                case TransitionType.TriggerEnterTag:
                    str += "Tags entering trigger: ";
                    if (triggerEnterTags == null || triggerEnterTags.Count == 0)
                        str += "ANY";
                    else
                        str += triggerEnterTags[0];
                    for (int i = 1; i < triggerEnterTags.Count; i++)
                    {
                        str += ", " + triggerEnterTags[i];
                    }
                    break;

                case TransitionType.CollisionTag:
                    str += "Collisions with tags: ";
                    if (collisionTags == null || collisionTags.Count == 0)
                        str += "ANY";
                    else
                        str += triggerEnterTags[0];
                    for (int i = 1; i < collisionTags.Count; i++)
                    {
                        str += ", " + collisionTags[i];
                    }
                    break;

                case TransitionType.Timer:
                    str += "Timer length" + timerLength;
                    break;

                default:
                    break;
            }

            str += "\nRequiring=";
            if (transitionConditions.Count <= 0)
            {
                str += "NO DEFINED CONDITIONS";
                return str;
            }

            switch (conditionEvaluation)
            {
                case TransitionConditionEvaluation.AllTrue:
                    str += "ALL OF:\n";
                    break;
                case TransitionConditionEvaluation.AnyTrue:
                    str += "ANY OF:\n";
                    break;
                case TransitionConditionEvaluation.AnyFalse:
                    str += "ANY FALSE OF:\n";
                    break;
                case TransitionConditionEvaluation.AllFalse:
                    str += "NONE OF:\n";
                    break;
                default:
                    break;
            }

            foreach(ATTransitionCondition condition in transitionConditions)
            {
                str += "\n" + condition.ToString();
            }

            return str;
        }

        // ONLY FOR EDITOR USAGE
        //public bool menuOpen;
        public int selectedMethod;
    }
}