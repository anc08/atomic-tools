using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtomicTools
{
    /*
     * AtomicTools::ATStateMachineSettings
     * Author: Adam Cohen
     * https://adamncohen08.wixsite.com/adam-cohen-game-dev
     * Scriptable object that defines the states within a state machine.
     */
    [CreateAssetMenu]
    public class ATStateMachineSettings : ScriptableObject
    {
        [SerializeField] private List<string> _states = new List<string>();
        //[SerializeField] private ATStateMachineBehavior _behaviorDefinitions;

        /*
         * PUBLIC FUNCTIONS
         */
        public List<string> GetStates() { return _states; }
        public int GetStateIndex(string s) { return _states.IndexOf(s); }
        public string GetStateName(int index) { return _states.Count <= index ? "" : _states[index]; }
    }
}