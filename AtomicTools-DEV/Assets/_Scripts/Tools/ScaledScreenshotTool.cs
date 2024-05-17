using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;

namespace AtomicTools
{
    /*
     * AtomicTools::ScaledScreenshotTool
     * Author: Adam Cohen
     * Dependencies: New Input System
     */
    public class ScaledScreenshotTool : MonoBehaviour
    {
        public string filepath = "";
        [Min(1)] public int scaleMultiplier = 1;
        public InputAction keybind = null;

        private void OnEnable()
        {
            keybind.Enable();
        }

        private void OnDisable()
        {
            keybind.Disable();
        }

        public void TakeScreenshot()
        {
            string sv = filepath + "/screenshot";
            while (File.Exists(sv + ".png")) sv += "_1";
            sv += ".png";

            ScreenCapture.CaptureScreenshot(sv, scaleMultiplier);
            Debug.Log("Screenshot stored at " + sv);
        }

        public void Update()
        {
            if (keybind != null)
            {
                if (keybind.triggered)
                {
                    TakeScreenshot();
                }
            }
        }

        public void OnValidate()
        {
            if (string.IsNullOrEmpty(filepath))
            {
                filepath = "Capture";
            }
        }
    }

}