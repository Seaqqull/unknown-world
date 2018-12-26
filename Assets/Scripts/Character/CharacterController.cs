using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityScript;


namespace UnknownWorld.Behaviour
{
    public class CharacterController : MonoBehaviour
    {
        [SerializeField] private CameraController m_cameraSettings; // A reference to the main camera in the scenes transform

        private CharacterAnimationController m_animation; // A reference to the ThirdPersonCharacter on the object
        private CharacterBehaviour m_behaviour;
        private float m_movementInputValue;
        private float m_turnInputValue;        
        private Vector3 m_move;
        private bool m_crouch;
        private bool m_jump; // the world-relative desired move direction, calculated from the camForward and user input.                       
        private bool m_run;


        private void Awake()
        {
            m_animation = GetComponent<CharacterAnimationController>();
            m_behaviour = GetComponent<CharacterBehaviour>();
        }

        private void Update()
        {            
            if ((m_behaviour.IsDeath) ||
                (!m_behaviour.IsActive))
                return;

            if (!m_jump)
            {
                m_jump = (m_behaviour.IsStaminaAction(m_behaviour.StaminaConsumption.JumpCost)) ? CrossPlatformInputManager.GetButtonDown("Jump") : false;
            }
            // read inputs
            if (m_animation.State != Data.AnimationState.InAir)
            {
                m_movementInputValue = CrossPlatformInputManager.GetAxis("Vertical");
                m_turnInputValue = CrossPlatformInputManager.GetAxis("Horizontal");
            }                      
        }

        private void FixedUpdate()
        {
            if ((m_behaviour.IsDeath) &&
                (!m_animation.IsDead))
            {
                m_move = Vector3.zero;
                m_animation.Dead();
            }
            else if (!m_behaviour.IsDeath)
                MoveAndRotate();
        }

        private void MoveAndRotate()
        {
            m_crouch = Input.GetKey(KeyCode.C);

            // calculate move direction to pass to character
            // we use world-relative directions in the case of no main camera
            m_move = m_movementInputValue * transform.forward * m_behaviour.MovementSpeed
                   + m_turnInputValue * transform.right * m_behaviour.RotationSpeed;

#if !MOBILE_INPUT
            // walk speed multiplier
            if ((!Input.GetKey(KeyCode.LeftShift)) ||
                (!m_behaviour.IsStaminaAction(m_behaviour.StaminaConsumption.RunCost * Time.deltaTime)))
            {
                m_move *= 0.5f;
                m_run = false;
            }
            else
            {
                m_run = true;
            }
#endif

            // pass all parameters to the character control script
            m_animation.Move(m_move, m_crouch, m_jump);
            PerformStaminaConsumption();
            m_jump = false;
        }

        private void PerformStaminaConsumption()
        {
            if (m_animation.IsGrounded && m_run && !m_crouch)
                m_behaviour.DoStaminaAction(m_behaviour.StaminaConsumption.RunCost * Time.deltaTime);

            if(/*m_animation.GroundCheckDistance == 0.1f*/m_animation.State == Data.AnimationState.Jump && m_jump)
                m_behaviour.DoStaminaAction(m_behaviour.StaminaConsumption.JumpCost);
        }
    }
}