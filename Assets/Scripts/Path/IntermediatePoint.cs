using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnknownWorld.Path
{
    public class IntermediatePoint : MonoBehaviour, IEquatable<IntermediatePoint>
    {
        [SerializeField] private float m_size = 0.3f;
        [SerializeField] private Color m_color = Color.black;

        protected static uint m_idCounter = 0;
        protected uint m_id;

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
        public uint Id
        {
            get { return this.m_id; }
        }


        private void Awake()
        {
            m_id = m_idCounter++;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = m_color;
            Gizmos.DrawWireSphere(transform.position, m_size);
        }


        public bool Equals(IntermediatePoint other)
        {
            return (this.m_id == other.m_id);
        }
    }
}
