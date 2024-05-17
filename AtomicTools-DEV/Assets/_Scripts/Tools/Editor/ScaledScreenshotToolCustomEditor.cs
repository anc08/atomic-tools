using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AtomicTools
{
    [CustomEditor(typeof(ScaledScreenshotTool))]
    public class ScaledScreenshotToolCustomEditor : Editor
    {
        ScaledScreenshotTool _target;

        void OnEnable()
        {
            _target = (ScaledScreenshotTool)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Take Screenshot"))
            {
                _target.TakeScreenshot();
            }
        }
    }
}