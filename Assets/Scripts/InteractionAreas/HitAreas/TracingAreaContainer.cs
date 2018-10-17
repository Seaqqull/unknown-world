using UnityEngine;

namespace UnknownWorld.Area.Target
{
    [System.Serializable]
    public class TracingAreaContainer : MonoBehaviour
    {
        private UnknownWorld.Behaviour.PersonBehaviour m_person;
        private TracingArea[] m_tracingAreas;

        public TracingArea[] TracingAreas
        {
            get
            {
                return this.m_tracingAreas;
            }
        }


        void Awake()
        {
            m_person = GetComponent<UnknownWorld.Behaviour.PersonBehaviour>();
            m_tracingAreas = GetComponentsInChildren<TracingArea>();
        }


        public int GetPointsCount()
        {
            return m_tracingAreas.Length;
        }

        public Transform GetPointTransform(int index)
        {
            if (index >= m_tracingAreas.Length && index < 0)
                return null;
            return m_tracingAreas[index].transform;
        }

        public UnknownWorld.Area.Data.HitAreaState GetPointState(int index)
        {
            if ((index >= m_tracingAreas.Length) && (index < 0))
                return UnknownWorld.Area.Data.HitAreaState.Unknown;
            return m_tracingAreas[index].State;
        }

        public void SetPointState(int index, UnknownWorld.Area.Data.HitAreaState areaState)
        {
            if ((index >= m_tracingAreas.Length) && (index < 0))
                return;
            m_tracingAreas[index].State = areaState;
        }
        
    }
}
