using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnknownWorld
{
    [System.Serializable]
    public class TracePoint : MonoBehaviour
    {
        public Vector3 m_offset;
        public string m_name;

        public bool m_isActive = true;
        private bool m_isPointAccessible = false;
        public bool IsPointAccessible
        {
            get
            {
                return this.m_isPointAccessible;
            }

            set
            {
                this.m_isPointAccessible = value;
            }
        }

        // ad some gozmo image
        public float m_gizmoSize = 0.1f;
        private Color m_gizmoColor = Color.blue;

        private Transform m_defaultTransform;
        private Vector3 m_defaultOffset;
        private BoxCollider m_box;
        public BoxCollider Box
        {
            get
            {
                return this.m_box;
            }
        }

        private void Start()
        {
            m_defaultTransform = transform;
            transform.position += m_offset;
            m_defaultOffset = m_offset;
            m_offset = Vector3.zero;
            m_isPointAccessible = false;
            m_box = GetComponent<BoxCollider>();
            m_box.size = new Vector3(m_gizmoSize, m_gizmoSize, m_gizmoSize);
        }

        private void OnDestroy()
        {
            transform.Translate(-m_defaultOffset);
            m_offset = m_defaultOffset;
            m_isPointAccessible = false;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = m_gizmoColor;
            Gizmos.DrawWireSphere(transform.position + m_offset, m_gizmoSize);
        }

    }
}
