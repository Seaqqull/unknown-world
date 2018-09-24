using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityScript;
//UnityStandardAssets.Characters.ThirdPerson

namespace UnknownWorld {    
    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class ThirdPersonUserControl : MonoBehaviour
    {
        [System.Serializable]
        public class CharacterCharacteristics
        {
            public float m_movementSpeed;
            public float m_rotationSpeed;
        }

        private ThirdPersonCharacter m_Character; // A reference to the ThirdPersonCharacter on the object
        public CameraController cameraSettings;                  // A reference to the main camera in the scenes transform
        private Vector3 m_CamForward;             // The current forward direction of the camera
        private Vector3 m_Move;
        private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.

        private float m_MovementInputValue;
        private float m_TurnInputValue;
        
        public CharacterCharacteristics m_playerCharacteristic;

        private void Start()
        {
            m_Character = GetComponent<ThirdPersonCharacter>();
        }

        void Reset()
        {

        }

        private void Update()
        {
            if (!m_Jump)
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }
            // read inputs
            m_TurnInputValue = CrossPlatformInputManager.GetAxis("Horizontal");
            m_MovementInputValue = CrossPlatformInputManager.GetAxis("Vertical");           
        }


        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
            MoveAndRotate();
        }

        private void MoveAndRotate() {
            bool crouch = Input.GetKey(KeyCode.C);

            // calculate move direction to pass to character
            // we use world-relative directions in the case of no main camera
            m_Move = m_MovementInputValue * transform.forward * m_playerCharacteristic.m_movementSpeed 
                   + m_TurnInputValue     * transform.right   * m_playerCharacteristic.m_rotationSpeed;

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
