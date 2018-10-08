using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnknownWorld
{
    [System.Serializable]
    public class TracePointContainer : MonoBehaviour
    {
        private ThirdPersonUserControl m_character;

        public TracePoint[] m_points;
        private TracePoint[] m_tracePoints;
        public TracePoint[] TracePoints
        {
            get
            {
                return m_tracePoints;
            }
        }


        private void Awake()
        {
            m_character = GetComponent<ThirdPersonUserControl>();
            updateInternal();
        }
        private void updateInternal()
        {
            m_tracePoints = m_points;

            /* Empty public variable*/
            m_points = null;
            Physics.IgnoreLayerCollision(11, 10);
        }

        public Transform getPointTransform(int index)
        {
            if (index >= m_tracePoints.Length && index < 0)
                return null;
            return m_tracePoints[index].transform;            
        }

        public void setPointState(int index, bool flag)
        {
            if ((index >= m_tracePoints.Length) && (index < 0))
                return;
            m_tracePoints[index].m_isActive = flag;
        }

        public bool? isPointActive(int index)
        {
            if (index >= m_tracePoints.Length && index < 0)
                return null;
            return m_tracePoints[index].m_isActive;
        }

        public int getPointsCount()
        {
            return m_tracePoints.Length;
        }

        public void TakeDamage(int damage)
        {
            /* some damage calculation */
        }
    }
}
