using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnknownWorld
{
    public class CameraSettings : MonoBehaviour
    {
        public enum InputChoice
        {
            KeyboardAndMouse, Controller,
        }

        [Serializable]
        public struct InvertSettings
        {
            public bool invertX;
            public bool invertY;
        }


        public Transform follow;
        public Transform lookAt;
        public InputChoice inputChoice;
        public InvertSettings keyboardAndMouseInvertSettings;
        public InvertSettings controllerInvertSettings;
        public bool allowRuntimeCameraSettingsChanges;


        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void Reset()
        {
            Transform keyboardAndMouseCameraTransform = transform.Find("KeyboardAndMouseFreeLookRig");

            ThirdPersonUserControl playerController = FindObjectOfType<ThirdPersonUserControl>();
            if (playerController != null /*&& playerController.name == "Ellen"*/)
            {
                follow = playerController.transform;

                lookAt = follow.Find("HeadTarget");

                if (playerController.cameraSettings == null)
                    playerController.cameraSettings = this;
            }
        }

        void UpdateCameraSettings()
        {
            //keyboardAndMouseCamera.Follow = follow;
            //keyboardAndMouseCamera.LookAt = lookAt;
            //keyboardAndMouseCamera.m_XAxis.m_InvertInput = keyboardAndMouseInvertSettings.invertX;
            //keyboardAndMouseCamera.m_YAxis.m_InvertInput = keyboardAndMouseInvertSettings.invertY;

            //controllerCamera.m_XAxis.m_InvertInput = controllerInvertSettings.invertX;
            //controllerCamera.m_YAxis.m_InvertInput = controllerInvertSettings.invertY;
            //controllerCamera.Follow = follow;
            //controllerCamera.LookAt = lookAt;

            //keyboardAndMouseCamera.Priority = inputChoice == InputChoice.KeyboardAndMouse ? 1 : 0;
            //controllerCamera.Priority = inputChoice == InputChoice.Controller ? 1 : 0;
        }
    }

}
