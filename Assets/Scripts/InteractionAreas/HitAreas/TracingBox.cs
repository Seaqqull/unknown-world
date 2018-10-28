using UnityEngine;
using UnknownWorld.Area.Data;

namespace UnknownWorld.Area.Target
{
    [System.Serializable]
    public class TracingBox : TracingArea
    {
        protected override void Awake()
        {
            base.Awake();
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        protected override void OnDrawGizmos()
        {
            if ((!base.m_colider)||
                (base.m_state == Data.HitAreaState.Disabled) ||
                (base.m_state == Data.HitAreaState.Unknown)) return;

            Gizmos.color = base.m_gizmoColor;
            Gizmos.
                DrawWireCube(base.m_colider.bounds.center, new Vector3(base.m_gizmoSize, base.m_gizmoSize, base.m_gizmoSize));
        }

        protected override Collider GetCollider()
        {
            return GetComponent<BoxCollider>();
        }

        protected override void SetState(HitAreaState incomeState)
        {
            base.SetState(incomeState);
        }
    }
}
