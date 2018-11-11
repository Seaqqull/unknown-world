using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnknownWorld.Path
{
    public class IntermediatePoint : MonoBehaviour
    {
        [SerializeField] private Color m_color = Color.black;
        [SerializeField] private float m_size = 0.3f;

        public Color Color
        {
            get { return this.m_color; }
            set { this.m_color = value; }
        }
        public float Size
        {
            get { return this.m_size; }
            set { this.m_size = value; }
        }


        private void OnDrawGizmos()
        {
            Gizmos.color = m_color;
            Gizmos.DrawWireSphere(transform.position, m_size);
        }
    }
}
