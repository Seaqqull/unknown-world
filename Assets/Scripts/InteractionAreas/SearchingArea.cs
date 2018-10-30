using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnknownWorld.Area.Observer
{
    [RequireComponent(typeof(UnknownWorld.Behaviour.PersonBehaviour))]
    public abstract class SearchingArea : MonoBehaviour
    {
        [SerializeField] protected UnknownWorld.Area.Data.ObservationType m_type = UnknownWorld.Area.Data.ObservationType.Undefined;
        [SerializeField] [Range(0, 1)] public float m_searchingDelay = 0.2f;
        [SerializeField] public Color m_colorZone = Color.white;
        [SerializeField] public Color m_colorTarget = Color.red;
        [SerializeField] protected bool m_isAreaActive = true;
        [SerializeField] protected int m_priority = 0;
        [SerializeField] protected Transform m_socket;        

        protected UnknownWorld.Behaviour.PersonBehaviour m_owner;
        private Coroutine m_searchingCorotation;
        protected static uint m_idCounter = 0;    
        protected bool m_isActive;
        protected uint m_id;

        public UnknownWorld.Area.Data.ObservationType Type
        {
            get
            {
                return this.m_type;
            }
        }
        public bool IsActive
        {
            get
            {
                return this.m_isActive;
            }
            set
            {
                SetIsActive(value);
            }

        }
        public int Priority
        {
            get
            {
                return this.m_priority;
            }
        }
        public uint Id
        {
            get
            {
                return this.m_id;
            }
        }

        public UnknownWorld.Area.Data.AreaData m_data;
        

        protected virtual void Awake()
        {
            m_owner = GetComponent<UnknownWorld.Behaviour.PersonBehaviour>();
            IsActive = m_isAreaActive;
            m_id = m_idCounter++;
            

            m_socket = m_socket ?? this.transform;
        }

        protected virtual void Update()
        {
            IsActive = m_isAreaActive;// only for editor
        }

        protected virtual void OnDrawGizmos()
        {
            if (!m_socket)
                m_socket = transform;
        }


        protected virtual void SetIsActive(bool isActive)
        {
            if (isActive == m_isActive)
                return;

            m_isActive = isActive;

            if (!m_isActive)
            {
                StopCoroutine(m_searchingCorotation);
            }
            else
            {
                m_searchingCorotation = StartCoroutine("FindTargetsWithDelay", m_searchingDelay);
            }
        }

        protected abstract IEnumerator FindTargetsWithDelay(float delay);


        public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
        {
            if (!angleIsGlobal)
            {               
                angleInDegrees += m_socket.eulerAngles.y + m_data.Rotation.y;
            }
            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }        
        
        public abstract bool IsTargetWithinArea(UnknownWorld.Area.Target.TracingArea[] target, BitArray affectionMask, params object[] list);        

    }
}
