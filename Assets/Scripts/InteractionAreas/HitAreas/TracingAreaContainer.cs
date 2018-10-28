using UnityEngine;

namespace UnknownWorld.Area.Target
{
    [System.Serializable]
    public class TracingAreaContainer : MonoBehaviour
    {
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
            m_tracingAreas = GetComponentsInChildren<TracingArea>();
        }


        public int GetPointsCount()
        {
            return m_tracingAreas.Length;
        }

        public void EnableAllAreas()
        {
            for (int i = 0; i < m_tracingAreas.Length; i++)
                m_tracingAreas[i].State = Data.HitAreaState.Enabled;
        }

        public void DisableAllAreas()
        {
            for (int i = 0; i < m_tracingAreas.Length; i++)
                m_tracingAreas[i].State = Data.HitAreaState.Disabled;
        }

        public void DisableArea(int index)
        {
            m_tracingAreas[index].State = Data.HitAreaState.Disabled;
        }

        public void DisableArea(string name)
        {
            for (int i = 0; i < m_tracingAreas.Length; i++)
                if (m_tracingAreas[i].Name == name)
                {
                    m_tracingAreas[i].State = Data.HitAreaState.Disabled;
                    break;
                }
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

        public void SetAreaState(int index, UnknownWorld.Area.Data.HitAreaState areaState)
        {
            if ((index >= m_tracingAreas.Length) && (index < 0))
                return;
            m_tracingAreas[index].State = areaState;
        }
  
    }
}
