using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnknownWorld
{
    public class SimpleInteractionAnnunciator : AInteracitonAnnunciator<TracePoint[]>
    {
        public bool isTargetWithinArea(SimpleData annuncicator, TracePoint[] target, bool[] affectionMask)
        {
            Collider[] targetsInRadius = Physics.
                OverlapSphere(annuncicator.Transform.position, annuncicator.Radius, annuncicator.TargetMask);
            bool isContainerAffected = false;

            for (int i = 0; i < targetsInRadius.Length; i++)
            {
                Transform affectedTransform = targetsInRadius[i].transform;
                Vector3 dirToTarget = (affectedTransform.position - annuncicator.Transform.position).normalized;

                if (Vector3.Angle(annuncicator.Transform.forward, dirToTarget) < annuncicator.Angle / 2)
                {
                    float distaceToTarget = Vector3.
                        Distance(annuncicator.Transform.position, affectedTransform.position);

                    if (!Physics.Raycast(annuncicator.Transform.position, dirToTarget, distaceToTarget, annuncicator.ObstacleMask))
                    {
                        for (int j = 0; j < target.Length; j++)
                        {
                            if (affectedTransform == target[j].transform)
                            {
                                isContainerAffected = true;
                                affectionMask[j] = true;                                
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
