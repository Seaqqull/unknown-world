using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace UnknownWorld.Navigation
{
    [RequireComponent(typeof(NavMeshSurface))]
    [RequireComponent(typeof(BoxCollider))]
    [System.Serializable]
    public class NavigationRegion : MonoBehaviour
    {
        [SerializeField] [Range(0, ushort.MaxValue)] private float m_minUpdateDelay = 1.0f;
        
        private static uint m_idCounter = 0;        
        private NavMeshSurface m_navMesh;
        private float m_timeSinceUpdate;
        private BoxCollider m_collider;
        private uint m_id;

        public uint Id
        {
            get { return this.m_id; }
        }


        private void Awake()
        {
            m_id = m_idCounter++;

            m_navMesh = GetComponent<NavMeshSurface>();
            m_collider = GetComponent<BoxCollider>();
        }

        private void Update()
        {
            m_timeSinceUpdate += Time.deltaTime;
        }


        public void OnCollisionEnter(Collision collision)
        {
            if (((1 << collision.gameObject.layer) & LayerMask.NameToLayer("Enemy")) == 0)
                return;

            collision.gameObject.GetComponent<UnknownWorld.Behaviour.AIBehaviour>().NavigationId = m_id;
        }        


        public void Rebake()
        {
            if (m_timeSinceUpdate < m_minUpdateDelay)
                return;

            m_timeSinceUpdate = 0;
            m_navMesh.RemoveData();
            m_navMesh.BuildNavMesh();
        }
    }
}