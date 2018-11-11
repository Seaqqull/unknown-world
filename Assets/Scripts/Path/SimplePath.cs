using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace UnknownWorld.Path
{
    public class SimplePath : PathContainer
    {
        public override Vector3 GetDestination(ref int destinationIndex)
        {
            if (m_points.Count == 0)
                throw new System.Exception("empty path");

            if (m_isRandom)
                return GetRandomPoint(ref destinationIndex);

            return GetNextPoint(ref destinationIndex);
        }
    }
}
