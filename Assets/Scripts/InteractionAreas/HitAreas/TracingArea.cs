using UnityEngine;
using UnknownWorld.Area.Data;

namespace UnknownWorld.Area.Target
{
    [System.Serializable]
    public abstract class TracingArea : MonoBehaviour
    {
        protected static Color EDITOR_GIZMO_COLOR = Color.blue;

        [SerializeField] protected UnknownWorld.Area.Data.HitAreaState m_state = HitAreaState.Activated;
        [SerializeField] protected string m_name;

        protected UnknownWorld.Area.Target.TracingAreaContainer m_areaContainer;
        protected Color m_gizmoColor = EDITOR_GIZMO_COLOR;
        protected Collider m_colider;

        public HitAreaState State
        {
            get
            {
                return this.m_state;
            }

            set
            {
                this.m_state = value;
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
            m_state = HitAreaState.Activated;

            m_colider = GetCollider();
        }

        protected virtual void OnDestroy()
        {
            m_state = HitAreaState.Activated;   
        }
        
        protected virtual void SetAreaColor()
        {
            switch (m_state)
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
