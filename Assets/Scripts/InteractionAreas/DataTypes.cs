using UnityEngine;

namespace UnknownWorld
{
    [System.Serializable]
    public class SimpleData
    {
        public LayerMask m_targetMask;
        public LayerMask m_obstacleMask;

        private Transform transform;
        [HideInInspector]
        public Transform Transform
        {
            get
            {
                return transform;
            }

            set
            {
                transform = value;
            }
        }

        [Range(0, 360)]
        public float m_angle;
        [Range(0, 500)]
        public float m_radius;

    }
    [System.Serializable]
    public class CircleTarget
    {
        public TracePointContainer m_points;
        public ThirdPersonUserControl m_target;        

        [HideInInspector]
        public bool isInRange;
    }
}
