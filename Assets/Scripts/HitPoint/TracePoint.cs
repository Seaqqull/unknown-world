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
        private bool m_isAreaAccessible = false;

        // ad some gozmo image
        public float m_gizmoSize = 0.1f;
        Color m_gizmoColor = Color.blue;

        Transform m_defaultTransform;
        Vector3 m_defaultOffset;
        BoxCollider m_box;

        public bool IsAreaAccessible
        {
            get
            {
                return m_isAreaAccessible;
            }

            set
            {
                m_isAreaAccessible = value;
            }
        }

        public BoxCollider Box
        {
            get
            {
                return m_box;
            }
        }

        private void Start()
        {
            m_defaultTransform = transform;
            transform.position += m_offset;
            m_defaultOffset = m_offset;
            m_offset = Vector3.zero;
            m_isAreaAccessible = false;
            m_box = GetComponent<BoxCollider>();
            m_box.size = new Vector3(m_gizmoSize, m_gizmoSize, m_gizmoSize);
            //m_box.enabled = false;
        }

        private void OnDestroy()
        {
            transform.Translate(-m_defaultOffset);
            m_offset = m_defaultOffset;
            m_isAreaAccessible = false;
        }

        void OnDrawGizmos()
        {
            // Draw a yellow sphere at the transform's position
            Gizmos.color = m_gizmoColor;
            Gizmos.DrawWireSphere(transform.position + m_offset, m_gizmoSize);
        }
        
    }
}
