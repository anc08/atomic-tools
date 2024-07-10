using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;
using UnityEditor;

namespace AtomicTools
{
    /// <summary>
    /// Struct <c>ATState</c> describes a state for the ATStateMachine class.
    /// </summary>
    [System.Serializable]
    public struct ATState
    {
        public ATStateMachineSettings settings;
        public int state;
    }

    /*
     * ATStateMachine
     * Author: Adam Cohen
     * https://adamncohen08.wixsite.com/adam-cohen-game-dev
     * Primary script of a modular state machine system, to be attached to any object that uses the state machine
     * Dependent on: ATStateTransition.cs, ATTransitionCondition.cs, ATEnums.cs, ATStateMachineBehavior.cs
     */

    /// <summary>
    /// Class <c>ATStateMachine</c> defines a state machine with modular user functionality.
    /// </summary>
    [System.Serializable]
    public class ATStateMachine : MonoBehaviour
    {
        //[Header("SETTINGS")]
        [SerializeField] private ATStateMachineSettings _settings;
        [Tooltip("All unique behavior methods should be in this script.")][SerializeField] private ATStateMachineBehavior _uniqueBehavior;

        //[Header("State Machine")]
        [Tooltip("Default starting state is index 0. Check this to use a different starting state.")][SerializeField] private bool _overrideStartState = false;
        [Tooltip("The state to start in. Only applies if previous option is checked.")][SerializeField] private ATState _startingState;
        [SerializeField] private List<ATStateTransition> _stateTransitions = new List<ATStateTransition>();

        // State tracking
        private ATState _state = new ATState();
        private List<string> _behaviorMethods;
        private List<string> _comparisonMethods;
        private List<int> _triggerEnterTransitions = new List<int>();
        private List<int> _collisionTransitions = new List<int>();
        private List<int> _callIdTransitions = new List<int>();
        Dictionary<int, List<int>> _timerTransitions = new Dictionary<int, List<int>>(); // NEEDS TO BE CHECKED
        List<Collider> _objectsInTrigger = new List<Collider>();    // MAKE INIT AUTO CREATE A LAYER MASK FOR TRIGGER DETECTIONS
        List<Collision> _objectsInCollision = new List<Collision>();

        // Private helper fields
        float _stateTimer;
        public Dictionary<int, bool> _runTimer = new Dictionary<int, bool>();

        // Fields for user interaction
        private bool _timersPaused = false;
        delegate void AlertSwitchSubscribers(int state);
        AlertSwitchSubscribers alertSwitchSubscribers;

        /*  STARTUP FUNCTIONS  */

        private void Awake()
        {
            if (_overrideStartState)
                _state.state = _startingState.state;
        }

        void Update()
        {
            if (!_timersPaused && _runTimer[_state.state])
                UpdateStateTimer(_state.state);
        }

        /// <summary>
        /// Method <c>InitializeTransitions</c> initializes all state transitions attached to this machine.
        /// </summary>
        public void InitializeTransitions()
        {
            _triggerEnterTransitions.Clear();
            _collisionTransitions.Clear();
            _timerTransitions.Clear();
            _callIdTransitions.Clear();
            foreach (int i in _runTimer.Keys)
                _runTimer[i] = false;

            for (int i = 0; i < _stateTransitions.Count; i++)
            {
                _stateTransitions[i].AttachToMachine(this);
                switch (_stateTransitions[i].transitionType)
                {
                    case TransitionType.TriggerEnterTag:
                        _triggerEnterTransitions.Add(i);
                        break;
                    case TransitionType.CollisionTag:
                        _collisionTransitions.Add(i);
                        break;
                    case TransitionType.Timer:
                        foreach (ATState frm in _stateTransitions[i].fromState)
                        {
                            _timerTransitions[frm.state].Add(i);   // [TODO] REDO THIS
                            _runTimer[frm.state] = true;
                        }
                        break;
                    case TransitionType.CallID:
                        _callIdTransitions.Add(i);
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
                    ProcessSuccessfulStateSwitch(index);
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
                    ProcessSuccessfulStateSwitch(index);
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

        public bool CustomComparison(string methodname)
        {
            if (_uniqueBehavior == null)
                return false;
            return _uniqueBehavior.InvokeComparison(methodname);
        }

        /*  OTHER FUNCTIONS  */
        private void UpdateStateTimer(int stateIndex)
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

        private void ReportTimerFinished(int transitionIndex)
        {
            if (_stateTransitions[transitionIndex].EvaluateCanSwitch(this))
                ProcessSuccessfulStateSwitch(transitionIndex);
        }

        /*  STATE MACHINE FUNCTIONS  */
        /// <summary>
        /// Method <c>ForceStateSwitch</c> forces this machine to change to the indicated state without any successful transition evaluations.
        /// </summary>
        /// <param name="newState">The integer index of the state to switch to.</param>
        public void ForceStateSwitch(int newState)
        {
            _state.state = newState;
            ResetStateTimer();
            _uniqueBehavior?.InvokeBehavior("OnStateSwitch" + _state + " - " + _settings.GetStateName(_state.state));
        }

        private void ProcessSuccessfulStateSwitch(ATStateTransition transition)
        {
            Debug.Log("SUCCESSFUL TRANSITION: " + _state + " --> " + transition.toState);
            _state = transition.toState;    // Switch to new state
            ResetStateTimer();
            if (transition.successMethodName != "None")
                _uniqueBehavior?.InvokeBehavior(transition.successMethodName);  // Call success method
            _uniqueBehavior?.OnStateSwitch(_state);  // Virtual method in base class; always gets called

            alertSwitchSubscribers(_state.state);
        }

        private void ProcessSuccessfulStateSwitch(int transitionIndex)
        {
            ProcessSuccessfulStateSwitch(_stateTransitions[transitionIndex]);
        }

        /*  EDITOR HELPER METHODS - DO NOT CALL  */
        void OnValidate()
        {
            if(_state.settings != _settings)
            {
                _state.settings = _settings;
            }
        }

        /// <summary>
        /// WARNING: THIS METHOD IS USED FOR CUSTOM EDITOR SERIALIZATION AND IS NOT MEANT TO BE CALLED BY USERS.
        /// </summary>
        public void DefineMethodNames()
        {
            /// Search assigned behavior script for methods that fit the requirements for Behavior or Comparison methods.
            /// Behavior methods can be set as success actions for state transitions.
            /// Comparison methods can be set as evaluation conditions for state transitions.
            
            if (_behaviorMethods == null) _behaviorMethods = new List<string>();        // Ensure behaviormethods list is ready
            else _behaviorMethods.Clear();
            if (_comparisonMethods == null) _comparisonMethods = new List<string>();    // Ensure comparisonmethods list is ready
            else _comparisonMethods.Clear();

            if (_uniqueBehavior == null) return;                                        // Do nothing if no behavior script is assigned 

            // Get all public methods declared in the behavior script
            Type t = _uniqueBehavior.GetType();
            MethodInfo[] info = t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            Debug.Log("METHODINFOS: " + info);

            foreach (MethodInfo m in info)
            {
                //if(m.GetParameters().Length <= 1)               // behavior and comparison methods must not require parameters
                if(true)
                {
                    if(m.ReturnType == typeof(bool))
                        _comparisonMethods.Add(m.Name);         // Methods with return type bool are custom comparisons
                    else if(m.ReturnType == typeof(void))
                    //else    
                        _behaviorMethods.Add(m.Name);           // Methods with return type void are behavior options
                }
            }
        }

        /// <summary>
        /// WARNING: THIS METHOD IS USED FOR CUSTOM EDITOR SERIALIZATION AND IS NOT MEANT TO BE CALLED BY USERS.
        /// </summary>
        public List<string> GetBehaviorMethodNames()
        {
            if (_behaviorMethods == null)
                return new List<string>();
            return new List<string>(_behaviorMethods);
        }

        /// <summary>
        /// WARNING: THIS METHOD IS USED FOR CUSTOM EDITOR SERIALIZATION AND IS NOT MEANT TO BE CALLED BY USERS.
        /// </summary>
        public List<string> GetComparisonMethodNames()
        {
            if (_comparisonMethods == null)
                return new List<string>();
            return new List<string>(_comparisonMethods);
        }

        /// <summary>
        /// WARNING: THIS METHOD IS USED FOR CUSTOM EDITOR SERIALIZATION AND IS NOT MEANT TO BE CALLED BY USERS.
        /// </summary>
        public void OverwriteTransitionsList(SerializedProperty property)
        {
            _stateTransitions.Clear();
            var i = property.GetEnumerator();
            while (i.MoveNext())
            {
                var item = i.Current as SerializedProperty;
                _stateTransitions.Add((ATStateTransition)item.boxedValue);
            }

            InitializeTransitions();
        }


        /* User Interface Functions */
        /// <summary>
        /// Method <c>CallIDTrigger</c> triggers transitions of type CallID whose Call ID matches the passed in string to attempt to pass.
        /// </summary>
        /// <param name="id">call ID to trigger</param>
        public void CallIDTrigger(string id)
        {
            foreach(int index in _callIdTransitions)
            {
                if (_stateTransitions[index].callId == id)
                {
                    if(_stateTransitions[index].EvaluateCanSwitch(this))
                    {
                        ProcessSuccessfulStateSwitch(index);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Method <c>PauseTimers</c> stops the progression of all Timer transitions until unpaused.
        /// </summary>
        public void PauseTimers()
        {
            _timersPaused = true;
        }

        /// <summary>
        /// Method <c>UnpauseTimers</c> resumes the progression of Timer transitions associated with this machine.
        /// </summary>
        public void UnpauseTimers()
        {
            _timersPaused = false;
        }

        /// <summary>
        /// Method <c>SubscribeToStateUpdates</c> subscribes a given method to receive updates when this machine successfully switches states.
        /// </summary>
        /// <param name="method">A method that takes an integer parameter defined by the state that was switched to and returns void.</param>
        public void SubscribeToStateUpdates(Action<int> method)
        {
            alertSwitchSubscribers += new AlertSwitchSubscribers(method);
        }

        /// <summary>
        /// Method <c>UnsubscribeFromStateUpdates</c> unsubscribes a given method from this machine's state switch updates.
        /// </summary>
        /// <param name="method">A method that takes an integer parameter defined by the state that was switched to and returns void.</param>
        public void UnsubscribeFromStateUpdates(Action<int> method)
        {
            alertSwitchSubscribers -= new AlertSwitchSubscribers(method);
        }

        /// <summary>
        /// Method <c>GetSettings</c> returns the State Machine Settings asset used by this machine.
        /// </summary>
        /// <returns>ATStateMachineSettings asset this machine is associated with</returns>
        public ATStateMachineSettings GetSettings()
        {
            return _settings;
        }

        /// <summary>
        /// Method <c>GetCurrentState</c> returns this machine's current state.
        /// </summary>
        /// <returns>ATState defining the current state this machine is in</returns>
        public ATState GetCurrentState()
        {
            return _state;
        }

        /// <summary>
        /// Method <c>SetState</c> changes the state of this machine. WARNING: DOES NOT TRIGGER NORMAL RESULTS OF SWITCHING STATES. NOT RECOMMENDED FOR USE. INSTEAD USE METHOD <c>ForceStateSwitch()</c>
        /// </summary>
        /// <param name="newState">The integer index of the state to switch to.</param>
        public void SetState(int newState)
        {
            _state.state = newState;
        }

        /// <summary>
        /// Method <c>GetTransitionsList</c> returns a list of the transitions attached to this machine.
        /// </summary>
        /// <returns>List of ATStateTransition structs associated with this machine</returns>
        public List<ATStateTransition> GetTransitionsList()
        {
            return _stateTransitions;
        }

        /// <summary>
        /// Method <c>RemoveTransition</c> removes a transition that matches the one given, if it is attached to this machine. May result in re-initializing.
        /// </summary>
        /// <param name="transition">The ATStateTransition to remove</param>
        public void RemoveTransition(ATStateTransition transition)
        {
            int index = _stateTransitions.IndexOf(transition);
            if (index != -1)
            {
                if (index < _stateTransitions.Count - 1)
                {
                    _stateTransitions.RemoveAt(index);
                    InitializeTransitions();
                }
                else
                {
                    switch (transition.transitionType)
                    {
                        case TransitionType.TriggerEnterTag:
                            _triggerEnterTransitions.Remove(index);

                            break;
                        case TransitionType.CollisionTag:
                            _collisionTransitions.Remove(index);
                            break;
                        case TransitionType.Timer:
                            foreach (ATState frm in transition.fromState)
                            {
                                _timerTransitions[frm.state].Remove(index);   // [TODO] REDO THIS
                                if (_timerTransitions[frm.state].Count == 0)
                                    _runTimer[frm.state] = false;
                            }
                            break;
                        case TransitionType.CallID:
                            _callIdTransitions.Remove(index);
                            break;
                        default:
                            break;
                    }
                    _stateTransitions.RemoveAt(index);
                }
            }
        }
    }
}