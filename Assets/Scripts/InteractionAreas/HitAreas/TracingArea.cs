using UnityEngine;
using UnknownWorld.Area.Data;

namespace UnknownWorld.Area.Target
{
    [System.Serializable]
    public abstract class TracingArea : MonoBehaviour
    {
        protected static Color EDITOR_GIZMO_COLOR = Color.blue;

        [SerializeField] protected UnknownWorld.Area.Data.HitAreaState m_areaState = HitAreaState.Activated;
        [SerializeField] private Vector3 m_offset;
        [SerializeField] protected string m_name;

        protected UnknownWorld.Area.Target.TracingAreaContainer m_areaContainer;
        protected Color m_gizmoColor = EDITOR_GIZMO_COLOR;
        protected Vector3 m_defaultOffset;
        protected Collider m_colider;

        public HitAreaState AreaState
        {
            get
            {
                return this.m_areaState;
            }

            set
            {
                this.m_areaState = value;
                SetAreaColor();
            }
        }
        public Collider Collider
        {
            get
            {
                return this.m_colider;
            }
        }
        public Vector3 Offset
        {
            get { return this.m_offset; }
        }
        public string Name
        {
            get
            {
                return this.m_name;
            }
        }
                
        public Color m_gizmoColorAccessible = Color.red;
        public Color m_gizmoColorInactive = Color.grey;
        public Color m_gizmoColorActive = Color.blue;
        public float m_gizmoSize = 0.1f;
        

        protected virtual void Awake()
        {
            m_areaContainer = GetComponentInParent<UnknownWorld.Area.Target.TracingAreaContainer>();
            AreaState = HitAreaState.Activated;
            transform.position += m_offset;
            m_defaultOffset = m_offset;
            m_colider = GetCollider();
            m_offset = Vector3.zero;
        }

        protected virtual void OnDestroy()
        {
            AreaState = HitAreaState.Activated;

            transform.Translate(-m_defaultOffset);            
            m_offset = m_defaultOffset;            
        }
        
        protected virtual void SetAreaColor()
        {
            switch (m_areaState)
            {
                case Data.HitAreaState.Accessible:
                    this.m_gizmoColor = this.m_gizmoColorAccessible;
                    break;
                case Data.HitAreaState.Deactivated:
                    this.m_gizmoColor = this.m_gizmoColorInactive;
                    break;
                case Data.HitAreaState.Activated:
                    this.m_gizmoColor = this.m_gizmoColorActive;
                    break;
                default:
                    break;
            }
        }

        protected virtual Collider GetCollider()
        {
            return GetComponent<Collider>();
        }


        protected abstract void OnDrawGizmos();

    }
}
