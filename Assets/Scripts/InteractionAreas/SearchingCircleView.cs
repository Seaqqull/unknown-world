using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnknownWorld.Area.Observer
{
    public class SearchingCircleView : SearchingArea
    {
        protected UnknownWorld.Behaviour.AIBehaviour m_ownerAi;


        protected override void Awake()
        {            
            base.Awake();
            m_ownerAi = (m_owner as UnknownWorld.Behaviour.AIBehaviour);
        }


        private void ShowTargets()
        {
            Vector3 positionWithOffset = transform.position + m_data.Offset;
            UnityEditor.Handles.color = m_colorTarget;

            if (m_ownerAi)
            {
                for (int i = 0; i < m_ownerAi.GetTargetCount(); i++)
                {
                    BitArray mask = m_ownerAi.GetMask(m_ownerAi.GetTargetId(i), m_id);

                    for (int j = 0; j < mask.Length; j++)
                    {
                        if (mask[j])
                            UnityEditor.Handles.DrawLine(positionWithOffset, m_ownerAi.GetPoints(i).TracingPoints[j].transform.position);
                    }
                }
            }
        }

        [System.Obsolete("This method can be used instead of standart handle drawing -> DrawCircleHandle")]
        private void DrawCircleGizmo()
        {
            Vector3 positionWithOffset = transform.position + m_data.Offset;
            Gizmos.color = m_colorZone;

            float theta = 0;
            float step = 0.5f;
            float x = m_data.Radius * Mathf.Cos(theta);
            float y = m_data.Radius * Mathf.Sin(theta);
            Vector3 pos = positionWithOffset + new Vector3(x, 0, y);
            Vector3 newPos = pos;
            Vector3 lastPos = pos;
            for (theta = 0.1f; theta < Mathf.PI * 2; theta += step)
            {
                x = m_data.Radius * Mathf.Cos(theta);
                y = m_data.Radius * Mathf.Sin(theta);
                newPos = positionWithOffset + new Vector3(x, 0, y);
                Gizmos.DrawLine(pos, newPos);
                pos = newPos;
            }
            Gizmos.DrawLine(pos, lastPos);

            if (m_data.Angle != 360.0f)
            {
                UnityEditor.Handles.DrawLine(positionWithOffset,
                    positionWithOffset + DirFromAngle(-m_data.Angle / 2, false) * m_data.Radius);
                UnityEditor.Handles.DrawLine(positionWithOffset,
                    positionWithOffset + DirFromAngle(m_data.Angle / 2, false) * m_data.Radius);
            }
        }

        private void DrawCircleHandle()
        {
            Vector3 positionWithOffset = transform.position + m_data.Offset;
            UnityEditor.Handles.color = m_colorZone;

            UnityEditor.Handles.DrawWireArc(positionWithOffset,
                Vector3.up, Vector3.forward, 360, m_data.Radius);
            if (m_data.Angle != 360.0f)
            {
                UnityEditor.Handles.DrawLine(positionWithOffset,
                positionWithOffset + DirFromAngle(-m_data.Angle / 2, false) * m_data.Radius);
                UnityEditor.Handles.DrawLine(positionWithOffset,
                    positionWithOffset + DirFromAngle(m_data.Angle / 2, false) * m_data.Radius);
            }
        }

        protected override IEnumerator FindTargetsWithDelay(float delay)
        {
            while (true)
            {
                yield return new WaitForSeconds(delay);
                m_ownerAi.ClearMasks(m_id);

                for (int i = 0; i < m_ownerAi.GetTargetCount(); i++)
                {
                    isTargetWithinArea(m_ownerAi.GetPoints(i).TracingPoints,
                        m_ownerAi.GetMask(m_ownerAi.GetTargetId(i), m_id));
                }
            }
        }


        public override void OnDrawGizmos()
        {
            DrawCircleHandle();

            ShowTargets();
        }

        public override bool isTargetWithinArea(UnknownWorld.Area.Target.TracingArea[] target, BitArray affectionMask)
        {
            Vector3 positionWithOffset = transform.position + m_data.Offset;
            Collider[] targetsInRadius = Physics.
                OverlapSphere(positionWithOffset, m_data.Radius, m_data.TargetMask);
            bool isContainerAffected = false;

            for (int i = 0; i < targetsInRadius.Length; i++)
            {
                Transform affectedTransform = targetsInRadius[i].transform;
                Vector3 dirToTarget = (affectedTransform.position - positionWithOffset).normalized;
                for (int j = 0; j < target.Length; j++)
                {
                    if ((affectedTransform == target[j].transform) &&
                        (Vector3.Angle(transform.forward, dirToTarget) < m_data.Angle / 2))
                    {
                        float distaceToTarget = Vector3.
                            Distance(positionWithOffset, affectedTransform.position);

                        if (!Physics.
                            Raycast(positionWithOffset, dirToTarget,
                                distaceToTarget, m_data.ObstacleMask))
                        {
                            isContainerAffected = true;
                            affectionMask[j] = true;
                            break;
                        }
                    }
                }
            }
            return isContainerAffected;
        }
     
    }
}
