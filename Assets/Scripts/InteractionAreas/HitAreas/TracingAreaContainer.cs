using UnityEngine;

namespace UnknownWorld.Area.Target
{
    [System.Serializable]
    public class TracingAreaContainer : MonoBehaviour
    {
        private UnknownWorld.Behaviour.PersonBehaviour m_character;
        private TracingArea[] m_tracePoints;

        public TracingArea[] TracingPoints
        {
            get
            {
                return this.m_tracePoints;
            }
        }


        void Awake()
        {
            m_character = GetComponent<UnknownWorld.Behaviour.PersonBehaviour>();
            m_tracePoints = GetComponentsInChildren<TracingArea>();

            // ignore colision between character and target
            Physics.IgnoreLayerCollision(11, 10);
        }


        public int GetPointsCount()
        {
            return m_tracePoints.Length;
        }

        public Transform GetPointTransform(int index)
        {
            if (index >= m_tracePoints.Length && index < 0)
                return null;
            return m_tracePoints[index].transform;
        }

        public UnknownWorld.Area.Data.HitAreaState SetPointState(int index)
        {
            if ((index >= m_tracePoints.Length) && (index < 0))
                return UnknownWorld.Area.Data.HitAreaState.Unknown;
            return m_tracePoints[index].AreaState;
        }

        public void SetPointState(int index, UnknownWorld.Area.Data.HitAreaState areaState)
        {
            if ((index >= m_tracePoints.Length) && (index < 0))
                return;
            m_tracePoints[index].AreaState = areaState;
        }
        
    }
}
