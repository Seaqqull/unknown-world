using UnityEngine;

namespace UnknownWorld
{
    [System.Serializable]
    public class SimpleData
    {
        [SerializeField]
        private LayerMask m_targetMask;
        public LayerMask TargetMask
        {
            get
            {
                return this.m_targetMask;
            }

            set
            {
                this.m_targetMask = value;
            }
        }

        [SerializeField]
        private LayerMask m_obstacleMask;
        public LayerMask ObstacleMask
        {
            get
            {
                return this.m_obstacleMask;
            }

            set
            {
                this.m_obstacleMask = value;
            }
        }

        private Transform m_transform;
        public Transform Transform
        {
            get
            {
                return this.m_transform;
            }

            set
            {
                this.m_transform = value;
            }
        }

        [SerializeField, Range(0, 360)]
        private float m_angle;
        public float Angle
        {
            get
            {
                return this.m_angle;
            }

            set
            {
                this.m_angle = value;
            }
        }

        [SerializeField, Range(0, 500)]
        private float m_radius;
        public float Radius
        {
            get
            {
                return this.m_radius;
            }

            set
            {
                this.m_radius = value;
            }
        }
    }
    [System.Serializable]
    public class CircleTarget
    {
        [SerializeField]
        private TracePointContainer m_points;
        public TracePointContainer Points
        {
            get
            {
                return this.m_points;
            }

            set
            {
                this.m_points = value;
            }
        }

        [SerializeField]
        private ThirdPersonUserControl m_target;
        public ThirdPersonUserControl Target
        {
            get
            {
                return this.m_target;
            }

            set
            {
                this.m_target = value;
            }
        }
    }
}
