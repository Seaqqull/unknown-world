using UnityEngine;
using UnknownWorld.Area.Data;

namespace UnknownWorld.Area.Target
{
    [System.Serializable]
    public abstract class TracingArea : MonoBehaviour
    {
        protected static Color editor_gizmo_color = Color.blue;

        [SerializeField] protected UnknownWorld.Area.Data.HitAreaState m_state = HitAreaState.Enabled;
        [SerializeField] protected string m_name;

        protected Color m_gizmoColor = editor_gizmo_color;
        protected Collider m_colider;

        public HitAreaState State
        {
            get
            {
                return this.m_state;
            }

            set
            {                
                SetState(value);
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
            State = HitAreaState.Enabled;

            m_colider = GetCollider();
        }

        protected virtual void OnDestroy()
        {
            m_state = HitAreaState.Enabled;   
        }

        
        protected virtual void SetAreaColor()
        {
            switch (m_state)
            {
                case Data.HitAreaState.Accessible:
                    this.m_gizmoColor = this.m_gizmoColorAccessible;
                    break;
                case Data.HitAreaState.Disabled:
                    this.m_gizmoColor = this.m_gizmoColorInactive;
                    break;
                case Data.HitAreaState.Enabled:
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

        protected virtual void SetState(HitAreaState incomeState)
        {
            m_state = incomeState;
        }
        

        protected abstract void OnDrawGizmos();
        
    }
}
