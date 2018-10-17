using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnknownWorld.Area.Observer
{
    [RequireComponent(typeof(UnknownWorld.Behaviour.PersonBehaviour))]
    public abstract class SearchingArea : MonoBehaviour
    {           
        [SerializeField] [Range(0, 1)] public float m_searchingDelay = 0.2f;
        [SerializeField] public Color m_colorZone = Color.white;
        [SerializeField] public Color m_colorTarget = Color.red;         
        [SerializeField] protected int m_priority = 0;

        protected UnknownWorld.Behaviour.PersonBehaviour m_owner;
        protected static uint m_idCounter = 0;
        private Coroutine m_coroutine;
        protected bool m_isActive;
        protected uint m_id;

        public bool IsActive
        {
            get
            {
                return this.m_isActive;
            }

            set
            {
                if (value == this.m_isActive)
                    return;

                this.m_isActive = value;

                if (!this.m_isActive)
                {
                    StopCoroutine(this.m_coroutine);
                }
                else
                {
                    this.m_coroutine = StartCoroutine("FindTargetsWithDelay", this.m_searchingDelay);
                }
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
        public bool m_isAreaActive = true;


        protected virtual void Awake()
        {
            m_owner = GetComponent<UnknownWorld.Behaviour.PersonBehaviour>();

            m_id = m_idCounter++;

            IsActive = m_isAreaActive;
        }


        public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
        {
            if (!angleIsGlobal)
            {
                angleInDegrees += transform.eulerAngles.y;
            }
            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }


        protected abstract IEnumerator FindTargetsWithDelay(float delay);


        public abstract void OnDrawGizmos();        

        public abstract bool IsTargetWithinArea(UnknownWorld.Area.Target.TracingArea[] target, BitArray affectionMask, params object[] list);        

    }
}
