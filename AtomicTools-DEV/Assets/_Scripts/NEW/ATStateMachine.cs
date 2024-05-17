using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;

namespace AtomicTools
{
    /*
     * ATStateMachine
     * Author: Adam Cohen
     * https://adamncohen08.wixsite.com/adam-cohen-game-dev
     * Primary script of a modular state machine system, to be attached to any object that uses the state machine
     * Dependencies: ATStateTransition.cs, ATTransitionCondition.cs, ATEnums.cs, ATStateMachineBehavior.cs
     */
    [System.Serializable]
    public class ATStateMachine : MonoBehaviour
    {
        [Header("SETTINGS")]
        [SerializeField] private ATStateMachineSettings _settings;
        [Tooltip("All unique behavior methods should be in this script.")][SerializeField] private ATStateMachineBehavior _uniqueBehavior;
        [Tooltip("If this is checked, the state transitions will automatically re-initialize on awake.")][SerializeField] private bool _initTransitionsOnAwake = true;

        [Header("State Machine")]
        [Tooltip("Default starting state is index 0. Check this to use a different starting state.")][SerializeField] private bool _overrideStartingState = false;
        [Tooltip("The state to start in. Only applies if previous option is checked.")][SerializeField] private ATState _startingState;
        [SerializeField] private List<ATStateTransition> _stateTransitions;

        // State tracking
        private ATState _state = new ATState();
        private List<string> _behaviorMethods;
        private List<int> _triggerEnterTransitions = new List<int>();
        private List<int> _collisionTransitions = new List<int>();
        private List<List<int>> _timerTransitions = new List<List<int>>(); // NEEDS TO BE CHECKED
        List<Collider> _objectsInTrigger = new List<Collider>();    // MAKE INIT AUTO CREATE A LAYER MASK FOR TRIGGER DETECTIONS
        List<Collision> _objectsInCollision = new List<Collision>();

        // Private helper fields
        float _stateTimer;
        public List<bool> _runTimer = new List<bool>();

        /*  STARTUP FUNCTIONS  */

        private void Awake()
        {
            if (_overrideStartingState)
                _state.state = _startingState.state;
            if (_initTransitionsOnAwake)
                InitializeTransitions();
        }

        void Update()
        {
            if (_runTimer[_state.state])
                UpdateStateTimer(_state.state);
        }

        public void InitializeTransitions()
        {
            _triggerEnterTransitions.Clear();
            _collisionTransitions.Clear();
            foreach (List<int> list in _timerTransitions)
                list.Clear();

            for (int i = 0; i < _stateTransitions.Count; i++)
            {
                switch (_stateTransitions[i].transitionType)
                {
                    case TransitionType.TriggerEnterTag:
                        _triggerEnterTransitions.Add(i);

                        break;
                    case TransitionType.CollisionTag:
                        _collisionTransitions.Add(i);
                        break;
                    case TransitionType.Timer:
                        foreach (int frm in _stateTransitions[i].fromState)
                            _timerTransitions[(int)frm].Add(i);   // [TODO] REDO THIS
                        break;
                    default:
                        Debug.LogError("Transition index " + i + ": invalid transition type");
                        break;
                }
            }
        }

        /*  COLLISION HANDLERS */

        void OnTriggerEnter(Collider other)
        {
            // Check if object is interactable item
            if (other.CompareTag("TODO"))
                _objectsInTrigger.Add(other); // Add object to tracking list

            foreach (int index in _triggerEnterTransitions) // Check whether any transitions are available
            {
                if (_stateTransitions[index].triggerEnterTags.Contains(other.tag) && _stateTransitions[index].EvaluateCanSwitch(this))
                {
                    //ProcessSuccessfulStateSwitch(index);
                    break;
                }
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (_objectsInTrigger.Contains(other))
                _objectsInTrigger.Remove(other); // Remove object from tracking list
        }

        void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("TODO"))
                _objectsInCollision.Add(other); // Add object to tracking list

            foreach (int index in _collisionTransitions) // Check whether any transitions are available
            {
                if (_stateTransitions[index].collisionTags.Contains(other.gameObject.tag) && _stateTransitions[index].EvaluateCanSwitch(this))
                {
                    //ProcessSuccessfulStateSwitch(index);
                    break;
                }
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (_objectsInCollision.Contains(other))
                _objectsInCollision.Remove(other); // Remove object from tracking list
        }

        /* CONDITION VALIDATION FUNCTIONS */
        public Collider FindTagsInTrigger(List<string> targetTags)
        {
            foreach (Collider obj in _objectsInTrigger)
            {
                foreach (string tag in targetTags)
                {
                    if (obj.CompareTag(tag))
                        return obj;
                }
            }
            return null;
        }

        public Collision FindTagsInCollision(List<string> targetTags)
        {
            foreach (Collision obj in _objectsInCollision)
            {
                foreach (string tag in targetTags)
                {
                    if (obj.gameObject.CompareTag(tag))
                        return obj;
                }
            }
            return null;
        }

        /*  OTHER FUNCTIONS  */
        public void UpdateStateTimer(int stateIndex)
        {
            if (_timerTransitions[stateIndex] != null)
            {
                _stateTimer += Time.deltaTime;
                foreach (int i in _timerTransitions[stateIndex])
                {
                    if (_stateTimer >= _stateTransitions[i].timerLength)
                        ReportTimerFinished(i);
                }
            }
        }

        private void ResetStateTimer()
        {
            _stateTimer = 0f;
        }

        public void ReportTimerFinished(int transitionIndex)
        {
            if (_stateTransitions[transitionIndex].EvaluateCanSwitch(this))
                ProcessSuccessfulStateSwitch(transitionIndex);
        }

        /*  STATE MACHINE FUNCTIONS  */
        private void ForceStateSwitch(int newState)
        {
            _state.state = newState;
            ResetStateTimer();
            _uniqueBehavior?.InvokeBehavior("OnStateSwitch" + _state + " - " + _settings.GetStateName(_state.state));
        }

        private void ProcessSuccessfulStateSwitch(ATStateTransition transition)
        {
            /*Debug.Log("SUCCESSFUL TRANSITION: " + state + " --> " + transition.toState);
            state = transition.toState;
            ResetStateTimer();
            if (transition.successMethodName != "None")
                _uniqueBehavior?.InvokeBehavior(transition.successMethodName);
            _uniqueBehavior?.InvokeBehavior("OnStateSwitch" + state);
            if (transition.triggerDeathImmediately)
                TriggerDeath(transition);
            //GameManager.Instance.ReportDeath(transition.deathReport);
            if (transition.triggerTaskCompletion)
                GameManager.Instance.ReportTaskCompletion(transition.taskCompletionReport);

            InteractableItem thisInteractable = GetComponent<InteractableItem>();
            if (thisInteractable != null)
            {
                if (transition.removeAttribute)
                    thisInteractable.RemoveAttribute(transition.attributeToRemove);
                if (transition.addAttribute)
                    thisInteractable.AddAttribute(transition.attributeToAdd);
            }
            else if (transition.removeAttribute || transition.addAttribute)
                Debug.LogWarning("State transition " + state + " > " + transition.toState + ": Attempted to add or remove attribute, no InteractableItem component found.");*/
        }

        private void ProcessSuccessfulStateSwitch(int transitionIndex)
        {
            ProcessSuccessfulStateSwitch(_stateTransitions[transitionIndex]);
        }

        public ATStateMachineSettings GetSettings()
        {
            return _settings;
        }

        public int GetCurrentState()
        {
            return _state.state;
        }

        public void SetState(int newState)
        {
            _state.state = newState;
        }

        /*  EDITOR HELPER METHODS - DO NOT CALL  */
        void OnValidate()
        {
            DefineMethodNames();
            _state.settings = _settings;
        }

        public void DefineMethodNames()
        {
            if (_behaviorMethods == null)
                _behaviorMethods = new List<string>();
            _behaviorMethods.Clear();
            if (_uniqueBehavior == null)
                return;

            Type t = _uniqueBehavior.GetType();
            MethodInfo[] info = t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            _behaviorMethods = new List<string>();
            foreach (MethodInfo m in info)
            {
                _behaviorMethods.Add(m.Name);
            }
        }

        public List<string> GetBehaviorMethodNames()
        {
            if (_behaviorMethods == null)
                return new List<string>();
            return _behaviorMethods;
        }

        public List<ATStateTransition> GetTransitionsList()
        {
            return _stateTransitions;
        }

        public void OverwriteTransitionsList(List<ATStateTransition> newlist)
        {
            _stateTransitions = newlist;
        }

    }
}