using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnknownWorld
{
    public class SimpleInteractionAnnunciator : AInteracitonAnnunciator<TracePoint[]>
    {
        public bool isTargetWithinArea(SimpleData annuncicator, TracePoint[] target)
        {
            Collider[] targetsInRadius = Physics.
                OverlapSphere(annuncicator.Transform.position, annuncicator.m_radius, annuncicator.m_targetMask);
            bool isContainerAffected = false;

            for (int i = 0; i < target.Length; i++)
                target[i].IsAreaAccessible = false;

            for (int i = 0; i < targetsInRadius.Length; i++)
            {
                Transform affectedTransform = targetsInRadius[i].transform;
                Vector3 dirToTarget = (affectedTransform.position - annuncicator.Transform.position).normalized;

                if (Vector3.Angle(annuncicator.Transform.forward, dirToTarget) < annuncicator.m_angle / 2)
                {
                    float distaceToTarget = Vector3.
                        Distance(annuncicator.Transform.position, affectedTransform.position);

                    if (!Physics.Raycast(annuncicator.Transform.position, dirToTarget, distaceToTarget, annuncicator.m_obstacleMask))
                    {
                        for (int j = 0; j < target.Length; j++)
                        {
                            if (affectedTransform == target[j].transform)
                            {
                                target[j].IsAreaAccessible = true;
                                isContainerAffected = true;
                                break;
                            }
                        }
                        
                    }                    
                }

            }
            return isContainerAffected;
        }
    }

}
