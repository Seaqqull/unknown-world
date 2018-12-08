using UnityEngine;
using UnknownWorld.Behaviour.Data;

namespace UnknownWorld.Behaviour
{
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(Rigidbody))]    
    [RequireComponent(typeof(Animator))]

    public class AiAnimationController : MonoBehaviour, Data.IPersonAction
    {
        [SerializeField] [Range(1f, 4f)]private float m_gravityMultiplier = 2f;
        [SerializeField] private float m_jumpPower = 12f;

        [SerializeField] private float m_groundCheckDistance = 0.3f;
        [SerializeField] private float m_stationaryTurnSpeed = 180;
        [SerializeField] private float m_movingTurnSpeed = 360;

        [SerializeField] [Range(0.0f, 1.0f)] private float m_moveSpeedMultiplier = 1f;
        [SerializeField] [Range(0.0f, 1.0f)] private float m_animSpeedMultiplier = 1f;
        
        [SerializeField] private float m_runCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
        //[SerializeField] private CameraController m_cameraSettings; // A reference to the main camera in the scenes transform        

        public float MoveSpeedMultiplier
        {
            get { return this.m_moveSpeedMultiplier; }
            set { this.m_moveSpeedMultiplier = value; }
        }
        public float AnimSpeedMultiplier
        {
            get { return this.m_animSpeedMultiplier; }
            set { this.m_animSpeedMultiplier = value; }
        }
        public Data.AnimationState State
        {
            get { return this.m_state; }
        }
        public bool IsGrounded
        {
            get { return this.m_isGrounded; }
        }
        public bool IsDead
        {
            get { return this.m_isDead; }
        }

        private float m_origGroundCheckDistance;
        private const float m_legHalf = 0.5f;
        private Data.AnimationState m_state;
        private CapsuleCollider m_capsule;
        private Vector3 m_capsuleCenter;
        private Vector3 m_groundNormal;
        private float m_capsuleHeight;
        private float m_forwardAmount;
        private Rigidbody m_rigidbody;
        private Animator m_animator;
        private float m_turnAmount;
        private bool m_isCrouching;
        private bool m_isGrounded;
        private bool m_isDead;

        private void Start()
        {
            m_capsule = GetComponent<CapsuleCollider>();
            m_rigidbody = GetComponent<Rigidbody>();
            m_animator = GetComponent<Animator>();
            m_capsuleHeight = m_capsule.height;
            m_capsuleCenter = m_capsule.center;

            m_rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            m_origGroundCheckDistance = m_groundCheckDistance;
        }

        private void OnAnimatorMove()
        {        
            // we implement this function to override the default root motion.
            // this allows us to modify the positional speed before it's applied.
            if (m_isGrounded && Time.deltaTime > 0)
            {
                Vector3 v = (m_animator.deltaPosition * m_moveSpeedMultiplier) / Time.deltaTime;

                // we preserve the existing y part of the current velocity.
                v.y = m_rigidbody.velocity.y;
                m_rigidbody.velocity = v;
            }
        }


        private void SetWaiting()
        {
            m_state = Data.AnimationState.Waiting;
            m_animator.SetBool("Attack", false);
            m_animator.SetBool("Reloading", false);
        }

        private void CheckGroundStatus()
        {
            RaycastHit hitInfo;
#if UNITY_EDITOR
            // helper to visualise the ground check ray in the scene view
            Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_groundCheckDistance));
#endif
            // 0.1f is a small offset to start the ray from inside the character
            // it is also good to note that the transform position in the sample assets is at the base of the character
            if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_groundCheckDistance))
            {
                m_groundNormal = hitInfo.normal;
                m_isGrounded = true;
                m_animator.applyRootMotion = true;
                m_state = Data.AnimationState.Grounded;
            }
            else
            {
                m_isGrounded = false;
                m_groundNormal = Vector3.up;
                m_animator.applyRootMotion = false;

                m_state = Data.AnimationState.InAir;
            }
        }

        private void ApplyExtraTurnRotation()
        {
            // help the character turn faster (this is in addition to root rotation in the animation)
            float turnSpeed = Mathf.Lerp(m_stationaryTurnSpeed, m_movingTurnSpeed, Mathf.Abs(m_forwardAmount));
            transform.Rotate(0, m_turnAmount * turnSpeed * Time.deltaTime, 0);
        }

        private void HandleAirborneMovement()
        {
            // apply extra gravity from multiplier:
            Vector3 extraGravityForce = (Physics.gravity * m_gravityMultiplier) - Physics.gravity;
            m_rigidbody.AddForce(extraGravityForce);

            m_groundCheckDistance = m_rigidbody.velocity.y < 0 ? m_origGroundCheckDistance : 0.01f;
        }

        private void UpdateAnimator(Vector3 move)
        {
            // update the animator parameters
            m_animator.SetFloat("Forward", m_forwardAmount, 0.1f, Time.deltaTime);
            m_animator.SetFloat("Turn", m_turnAmount, 0.1f, Time.deltaTime);
            m_animator.SetBool("Crouch", m_isCrouching);
            m_animator.SetBool("OnGround", m_isGrounded);
            if (!m_isGrounded)
            {
                m_animator.SetFloat("Jump", m_rigidbody.velocity.y);
            }

            // calculate which leg is behind, so as to leave that leg trailing in the jump animation
            // (This code is reliant on the specific run cycle offset in our animations,
            // and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
            float runCycle =
                Mathf.Repeat(
                    m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime + m_runCycleLegOffset, 1);
            float jumpLeg = (runCycle < m_legHalf ? 1 : -1) * m_forwardAmount;
            if (m_isGrounded)
            {
                m_animator.SetFloat("JumpLeg", jumpLeg);
            }

            // the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
            // which affects the movement speed because of the root motion.
            if (m_isGrounded && move.magnitude > 0)
            {
                m_animator.speed = m_animSpeedMultiplier;
            }
            else
            {
                // don't use that while airborne
                m_animator.speed = 1;
            }
        }

        private void ScaleCapsuleForCrouching(bool crouch)
        {
            if (m_isGrounded && crouch)
            {
                if (m_isCrouching) return;
                m_capsule.height = m_capsule.height / 2f;
                m_capsule.center = m_capsule.center / 2f;
                m_isCrouching = true;
            }
            else
            {
                m_capsule.height = m_capsuleHeight;
                m_capsule.center = m_capsuleCenter;
                m_isCrouching = false;
            }
        }

        private void HandleGroundedMovement(bool crouch, bool jump)
        {
            // check whether conditions are right to allow a jump:
            if (jump && !crouch && m_animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
            {
                // jump!
                m_rigidbody.velocity = new Vector3(m_rigidbody.velocity.x, m_jumpPower, m_rigidbody.velocity.z);
                m_isGrounded = false;
                m_animator.applyRootMotion = false;
                m_groundCheckDistance = 0.1f;

                m_state = Data.AnimationState.Jump;
            }
        }


        public void Dead()
        {
            m_isDead = true;

            m_forwardAmount = 0.0f;
            m_turnAmount = 0.0f;

            m_animator.SetFloat("Forward", m_forwardAmount);
            m_animator.SetFloat("Turn", m_turnAmount);
            m_animator.SetBool("Dead", true);
        }

        public void Rotate(Vector3 move)
        {
            if ((m_state != Data.AnimationState.Attacking) ||
                (m_state != Data.AnimationState.Reloading))
                return;

            if (move.magnitude > 1f) move.Normalize();

            move = transform.InverseTransformDirection(move);
            CheckGroundStatus();

            move = Vector3.ProjectOnPlane(move, m_groundNormal);
            m_turnAmount = Mathf.Atan2(move.x, move.z);

            ApplyExtraTurnRotation();

            UpdateAnimator(move);
        }

        public bool IsActionPerfomerable()
        {
            if (m_isDead)
                return false;

            switch (m_state)
            {
                case Data.AnimationState.Grounded:
                case Data.AnimationState.Unknown:
                case Data.AnimationState.Waiting:
                case Data.AnimationState.Crouch:
                case Data.AnimationState.Walk:
                case Data.AnimationState.Run:
                    return true;
                case Data.AnimationState.Jump:
                case Data.AnimationState.InAir:
                case Data.AnimationState.Attacking:
                case Data.AnimationState.Reloading:
                    return false;
                default:
                    return false;
            }
        }

        public void Attack(float animationDuration)
        {
            if (!IsActionPerfomerable())
                return;

            m_forwardAmount = 0.0f;
            m_turnAmount = 0.0f;            

            m_animator.SetFloat("Forward", m_forwardAmount);
            m_animator.SetFloat("Turn", m_turnAmount);
            m_animator.SetBool("Attack", true);

            m_state = Data.AnimationState.Attacking;
            
            Invoke("SetWaiting", animationDuration);
        }

        public void Reload(float animationDuration)
        {
            if (!IsActionPerfomerable())
                return;

            m_forwardAmount = 0.0f;
            m_turnAmount = 0.0f;

            m_animator.SetFloat("Forward", m_forwardAmount);
            m_animator.SetFloat("Turn", m_turnAmount);
            m_animator.SetBool("Reload", true);

            m_state = Data.AnimationState.Attacking;

            Invoke("SetWaiting", animationDuration);
        }

        public void Move(Vector3 move, bool crouch, bool jump)
        {
            if ((m_state == Data.AnimationState.Attacking) ||
                (m_state == Data.AnimationState.Reloading))
                return;
            // convert the world relative moveInput vector into a local-relative
            // turn amount and forward amount required to head in the desired
            // direction.
            if (move.magnitude > 1f) move.Normalize();

            move = transform.InverseTransformDirection(move);
            CheckGroundStatus();

            move = Vector3.ProjectOnPlane(move, m_groundNormal);
            m_turnAmount = Mathf.Atan2(move.x, move.z);

            m_forwardAmount = move.z;

            ApplyExtraTurnRotation();

            // control and velocity handling is different when grounded and airborne:
            if (m_isGrounded)
            {
                HandleGroundedMovement(crouch, jump);
            }
            else
            {
                HandleAirborneMovement();
            }

            if (m_state == Data.AnimationState.Grounded)
            {
                if (m_forwardAmount > 0.55f)
                    m_state = Data.AnimationState.Run;
                else if (m_forwardAmount > 0.05f)
                    m_state = Data.AnimationState.Walk;
                else
                    m_state = Data.AnimationState.Waiting;
                if (crouch)
                    m_state = Data.AnimationState.Crouch;
            }

            ScaleCapsuleForCrouching(crouch);

            // send input and other state parameters to the animator
            UpdateAnimator(move);
        }

        public bool GetAnimationStateInfo(int layerIndex, string stateName)
        {
            return m_animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
        }
    }
}
