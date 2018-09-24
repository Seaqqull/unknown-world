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

        public struct CameraDefaultSettings
        {
            public Quaternion m_rotation;
            public Vector3 m_position;
        }

        private CameraDefaultSettings m_settingDefault;

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

        private float m_rotationVelocity;
        public float m_rotationScale = 1.0f;


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
            
            /* Camera rotation */
            transform.rotation = Quaternion.Slerp(transform.rotation, m_settingDefault.m_rotation * m_targetPosition.rotation, m_rotationVelocity);
            m_rotationVelocity = (m_rotationVelocity * m_rotationScale) + Time.deltaTime;
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

            m_settingDefault.m_rotation = m_targetPosition.rotation;
            m_settingDefault.m_position = m_targetPosition.position;
        }

        public void DiactivateCamera() {
            m_isCameraActive = false;

            m_targetPosition = null;
            m_targetlookAtPosition = null;

            transform.position = m_settingDefault.m_position;
            transform.rotation = m_settingDefault.m_rotation;
        }
    }

}
