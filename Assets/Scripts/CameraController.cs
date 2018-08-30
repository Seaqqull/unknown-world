using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnknownWorld
{
    public class CameraController : MonoBehaviour
    {
        public enum InputChoice
        {
            KeyboardAndMouse, Controller,
        }

        [System.Serializable]
        public struct InvertSettings
        {
            public bool invertX;
            public bool invertY;
        }

        public Transform m_targetPosition;
        public Transform m_targetlookAtPosition;
        public InputChoice inputChoice;
        public InvertSettings keyboardAndMouseInvertSettings;
        public InvertSettings controllerInvertSettings;

        public bool allowRuntimeCameraSettingsChanges;
        public float m_DampTime = 0.2f;


        private bool m_isCameraActive = false;
        private Camera m_Camera;
        private float m_zoomVelocity;
        private Vector3 m_moveVelocity;

        private void Awake()
        {
            m_Camera = GetComponentInChildren<Camera>();
        }

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void FixedUpdate()
        {
            if(m_isCameraActive)
                Move();         
        }

        private void Move()
        {
            transform.position = Vector3.SmoothDamp(transform.position, m_targetPosition.position, ref m_moveVelocity, m_DampTime);
        }

        private void Zoom()
        {
            /*Some actions to perform zooming on objects*/
            //m_Camera.orthographicSize = Mathf.SmoothDamp(m_Camera.orthographicSize, requiredSize, ref m_zoomVelocity, m_DampTime);
        }

        public void ActivateCamera(Transform cameraTarget, Transform cameraLookAt) {
            m_isCameraActive = true;

            m_targetPosition = cameraTarget;
            m_targetlookAtPosition = cameraLookAt;
        }

        public void DiactivateCamera() {
            m_isCameraActive = false;

            m_targetPosition = null;
            m_targetlookAtPosition = null;
        }
    }

}
