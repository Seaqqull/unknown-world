using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnknownWorld.Area.Observer
{
    public class SearchingCircleSonar : SearchingArea
    {
        protected UnknownWorld.Behaviour.AIBehaviour m_ownerAi;


        protected override void Awake()
        {
            base.Awake();

            m_ownerAi = (base.m_owner as UnknownWorld.Behaviour.AIBehaviour);
        }


        private void ShowTargets()
        {
            Vector3 positionWithOffset = m_socket.transform.position + m_data.Offset;
            UnityEditor.Handles.color = m_colorTarget;

            if (m_ownerAi)
            {
                for (int i = 0; i < m_ownerAi.GetTargetsCount(); i++)
                {
                    BitArray mask = m_ownerAi.GetMask(m_ownerAi.GetSubjectId(i), m_id);

                    for (int j = 0; j < mask.Length; j++)
                    {
                        if (mask[j])
                            UnityEditor.Handles.DrawLine(positionWithOffset, m_ownerAi.GetAreaContainer(i).TracingAreas[j].transform.position);
                    }
                }
            }
        }

        [System.Obsolete("This method can be used instead of standart handle drawing -> DrawCircleHandle")]
        private void DrawCircleGizmo()
        {
            Vector3 positionWithOffset = m_socket.transform.position + m_data.Offset;
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
            Vector3 positionWithOffset = m_socket.transform.position + m_data.Offset;
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

        protected override void SetIsActive(bool isActive)
        {
            if (m_isActive == isActive) return;

            base.SetIsActive(isActive);

            if (!isActive)
                m_ownerAi.ClearAreaMasks(m_id);            
        }

        protected override IEnumerator FindTargetsWithDelay(float delay)
        {
            while (true)
            {
                yield return new WaitForSeconds(delay);
                if ((!m_ownerAi.IsActive) || (!m_ownerAi.IsManagerActive)) continue;

                m_ownerAi.ClearAreaMasks(m_id);

                if (!IsActive) continue;

                for (int i = 0; i < m_ownerAi.GetTargetsCount(); i++)
                {
                    if (!m_ownerAi.IsTargetActive(i)) continue;

                    IsTargetWithinArea(m_ownerAi.GetAreaContainer(i).TracingAreas,
                        m_ownerAi.GetMask(m_ownerAi.GetSubjectId(i), m_id));
                }
            }
        }


        public override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            DrawCircleHandle();

            ShowTargets();
        }

        public override bool IsTargetWithinArea(UnknownWorld.Area.Target.TracingArea[] target, BitArray affectionMask, params object[] list)
        {
            bool isContainerAffected = false;

            Vector3 positionWithOffset = m_socket.transform.position + m_data.Offset;
            Vector3 vectorsSubstraction;

            for (int i = 0; i < target.Length; i++)
            {
                if ((target[i].State == Data.HitAreaState.Disabled) ||
                   (target[i].State == Data.HitAreaState.Unknown))
                    continue;

                vectorsSubstraction = target[i].transform.position - positionWithOffset;

                if ((((1<<target[i].Collider.gameObject.layer) & m_data.TargetMask) == 0) ||
                    (vectorsSubstraction.magnitude > m_data.Radius) ||
                    (m_data.Angle != 360) ||
                    //(Vector3.Angle(transform.forward, vectorsSubstraction.normalized) > m_data.Angle / 2)
                    (Vector3.Angle((Quaternion.Euler(m_data.Rotation) * m_socket.transform.forward), vectorsSubstraction.normalized) > m_data.Angle / 2))
                    continue;
                
                isContainerAffected = true;
                affectionMask[i] = true;
            }

            return isContainerAffected;
        }

    }
}
