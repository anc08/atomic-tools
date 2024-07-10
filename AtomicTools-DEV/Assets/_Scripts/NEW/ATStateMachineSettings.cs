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

    /// <summary>
    /// ScriptableObject class <c>ATStateMachineSettings</c> defines the states within a state machine.
    /// </summary>
    [CreateAssetMenu(menuName = "AtomicTools/State Machine Settings Asset")]
    public class ATStateMachineSettings : ScriptableObject
    {
        [SerializeField] private List<string> _states = new List<string>();
        //[SerializeField] private ATStateMachineBehavior _behaviorDefinitions;

        /*
         * PUBLIC FUNCTIONS
         */
        /// <summary>
        /// Method <c>GetStates</c> gets a list of the states within this settings asset.
        /// </summary>
        /// <returns>List<string> containing the names of the states defined by this asset</string></returns>
        public List<string> GetStates() { return _states; }

        /// <summary>
        /// Method <c>GetStateIndex</c> finds the index of a state within this settings asset given its name.
        /// </summary>
        /// <param name="s">name of the state to find</param>
        /// <returns>int index of the state</returns>
        public int GetStateIndex(string s) { return _states.IndexOf(s); }

        /// <summary>
        /// Method <c>GetStateName</c> finds the name of a state within this settings asset given its index.
        /// </summary>
        /// <param name="index">index of the state to find</param>
        /// <returns>string name of the state</returns>
        public string GetStateName(int index) { return _states.Count <= index ? "" : _states[index]; }
    }
}