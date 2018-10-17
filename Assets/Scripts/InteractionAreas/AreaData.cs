using System.Collections;
using UnityEngine;

namespace UnknownWorld.Area.Data
{
    [System.Serializable]
    public enum ObserverState
    {
        Dead,
        Waiting,
        Escaping,
        ChasingSound,
        ChasingEnemy,
        FollowingRoute,
        ReturningRoute
    }

    [System.Serializable]
    public enum HitAreaState
    {
        Unknown,
        Activated,
        Accessible,
        Deactivated
    }

    [System.Serializable]
    public class AreaAffectionMask
    {
        [System.Serializable]
        public struct AreaAddress
        {
            private uint m_areaId;
            private uint m_cameraId;
            private uint m_targetId;

            public uint areaId
            {
                get
                {
                    return this.m_areaId;
                }
            }
            public uint cameraId
            {
                get
                {
                    return this.m_cameraId;
                }
            }
            public uint targetId
            {
                get
                {
                    return this.m_targetId;
                }
            }

            public AreaAddress(uint targetId = 0, uint cameraId = 0, uint areaId = 0)
            {
                this.m_cameraId = cameraId;
                this.m_targetId = targetId;
                this.m_areaId = areaId;
            }
        }

        private AreaAddress m_areaAddresses;
        private BitArray m_affectedMask;

        public AreaAddress AreaAddresses
        {
            get
            {
                return this.m_areaAddresses;
            }
        }
        public BitArray AffectedMask
        {
            get
            {
                return this.m_affectedMask;
            }
            set
            {
                this.m_affectedMask = value;
            }
        }


        public AreaAffectionMask(int maskSize, uint targetId = 0, uint cameraId = 0, uint areaId = 0)
        {
            m_areaAddresses = new AreaAddress(targetId, cameraId, areaId);
            m_affectedMask = new BitArray(maskSize);
        }

    }

    [System.Serializable]
    public class AreaTarget
    {
        [SerializeField] private UnknownWorld.Area.Target.TracingAreaContainer m_points;
        [SerializeField] private UnknownWorld.Behaviour.CharacterBehaviour m_subject;

        public UnknownWorld.Area.Target.TracingAreaContainer Points
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
        public UnknownWorld.Behaviour.CharacterBehaviour Subject
        {
            get
            {
                return this.m_subject;
            }

            set
            {
                this.m_subject = value;
            }
        }

    }

    [System.Serializable]
    public class AreaData
    {
        [SerializeField] [Range(0, 500)] private float m_radius;
        [SerializeField] [Range(0, 360)] private float m_angle;
        [SerializeField] private LayerMask m_obstacleMask;
        [SerializeField] private LayerMask m_targetMask;
        [SerializeField] private Vector3 m_offset;
               
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
        public Vector3 Offset
        {
            set
            {
                this.m_offset = value;
            }
            get
            {
                return this.m_offset;
            }
        }
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

    }
}
