using System;
using UnityEngine;

namespace UnknownWorld
{
    [RequireComponent(typeof (UnityEngine.AI.NavMeshAgent))]
    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class AICharacterControl : MonoBehaviour
    {
        private UnityEngine.AI.NavMeshAgent m_agent; // the navmesh agent required for the path finding
        private ThirdPersonCharacter m_character; // the character we are controlling
        private Transform m_target; // target to aim for      

        public UnityEngine.AI.NavMeshAgent Agent
        {
            get { return this.m_agent; }
        }
        public ThirdPersonCharacter Character
        {
            get { return this.m_character; }
        }
        public Transform Target
        {
            get { return this.m_target; }
        }


        private void Start()
        {
            // get the components on the object we need ( should not be null due to require component so no need to check )
            m_agent = GetComponentInChildren<UnityEngine.AI.NavMeshAgent>();
            m_character = GetComponent<ThirdPersonCharacter>();

            m_agent.updateRotation = false;
            m_agent.updatePosition = true;
        }

        private void Update()
        {
            if (m_target != null)
                m_agent.SetDestination(m_target.position);

            if (m_agent.remainingDistance > m_agent.stoppingDistance)
                m_character.Move(m_agent.desiredVelocity, false, false);
            else
                m_character.Move(Vector3.zero, false, false);
        }

    }
}
