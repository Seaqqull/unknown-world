using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnknownWorld.Path
{
    public class PriorityClosenessPath : PathContainer
    {
        public override Vector3 GetDestination(ref int destinationIndex)
        {
            if (m_points.Count == 0)
                throw new System.Exception("empty path");

            if (m_isRandom)
                return GetRandomPoint(ref destinationIndex);

            float distanceTemp = 0.0f,
                  distance = int.MaxValue;
            ushort priority = 0;
            
            for (int i = 0; i < m_points.Count; i++)
            {
                distanceTemp = Vector3.Distance(m_ownerTransform.position, m_points[i].Transform.position);
                if ((m_points[i].Priority < priority) ||
                    ((m_points[i].Priority == priority) && (distanceTemp >= distance))) continue;

                distance = distanceTemp;
                destinationIndex = i;
            }
            return m_points[destinationIndex].Transform.position;
        }
    }
}
