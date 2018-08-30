using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityScript;
//UnityStandardAssets.Characters.ThirdPerson

namespace UnknownWorld { 

    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class ThirdPersonUserControl : MonoBehaviour
    {
        private ThirdPersonCharacter m_Character; // A reference to the ThirdPersonCharacter on the object
        public CameraSettings cameraSettings;                  // A reference to the main camera in the scenes transform
        private Vector3 m_CamForward;             // The current forward direction of the camera
        private Vector3 m_Move;
        private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.

        private void Start()
        {

            cameraSettings = FindObjectOfType<CameraSettings>();

            if (cameraSettings != null)
            {
                if (cameraSettings.follow == null)
                    cameraSettings.follow = transform;

                if (cameraSettings.lookAt == null)
                    cameraSettings.follow = transform.Find("HeadTarget");
            }
            // get the third person character ( this should never be null due to require component )
            m_Character = GetComponent<ThirdPersonCharacter>();
        }

        void Reset()
        {
            // get the transform of the main camera
            //cameraSettings = FindObjectOfType<CameraSettings>();

            //if (cameraSettings != null)
            //{
            //    if (cameraSettings.follow == null)
            //        cameraSettings.follow = transform;

            //    if (cameraSettings.lookAt == null)
            //        cameraSettings.follow = transform.Find("HeadTarget");
            //}
        }

        private void Update()
        {
            if (!m_Jump)
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }
        }


        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
            // read inputs
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");
            bool crouch = Input.GetKey(KeyCode.C);

            // calculate move direction to pass to character
            if (cameraSettings != null)
            {
                // calculate camera relative direction to move:
                //m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
                //m_Move = v*m_CamForward + h*m_Cam.right;

                //m_Move = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;
                m_Move = v * transform.forward + h * transform.right;
            }
            else
            {
                // we use world-relative directions in the case of no main camera
                m_Move = v*Vector3.forward + h*Vector3.right;
            }
#if !MOBILE_INPUT
			// walk speed multiplier
	        if (Input.GetKey(KeyCode.LeftShift)) m_Move *= 0.5f;
#endif

            // pass all parameters to the character control script
            m_Character.Move(m_Move, crouch, m_Jump);
            m_Jump = false;
        }
    }
}
