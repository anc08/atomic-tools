using AtomicTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributeTesting : MonoBehaviour
{
    //const string[] x = { "hi", "bye" };
    [SerializeField] ATStateMachineSettings _settings;
    List<string> _options;

    public ATState state;

    private void OnValidate()
    {
        state.settings = _settings;
    }
}
