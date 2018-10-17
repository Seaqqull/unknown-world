using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace UnknownWorld.Behaviour
{
    [System.Serializable]
    public class AIBehaviour : PersonBehaviour
    {
        private List<UnknownWorld.Area.Observer.SearchingArea> m_areas;
        protected UnknownWorld.Area.AreaManager m_areaManager;

        public List<UnknownWorld.Area.Observer.SearchingArea> Areas
        {
            get
            {
                if (this.m_areas == null)
                {
                    this.m_areas = new List<UnknownWorld.Area.Observer.SearchingArea>();
                }
                return this.m_areas;
            }

            set
            {
                this.m_areas = value;
            }
        }
        

        protected override void Awake()
        {
            base.Awake();
            /* AI specific initialization */
            m_areaManager = GetComponentInParent<UnknownWorld.Area.AreaManager>();
            m_areas = GetComponents<UnknownWorld.Area.Observer.SearchingArea>().
                OfType<UnknownWorld.Area.Observer.SearchingArea>().ToList();
        }


        public int GetTargetsCount()
        {
            return m_areaManager.Targets.Count;
        }

        public void ClearAreaMasks(uint areaId)
        {
            m_areaManager.ClearMasks(this.Id, areaId);
        }

        public uint GetSubjectId(int targetPosition)
        {
            return m_areaManager.Targets[targetPosition].Subject.Id;
        }
       
        public BitArray GetMask(uint targetId, uint areaId)
        {
            return m_areaManager.GetMask(targetId, this.Id, areaId);
        }

        public UnknownWorld.Area.Target.TracingAreaContainer GetAreaContainer(int targetPosition)
        {
            return m_areaManager.Targets[targetPosition].AreaContainer;
        }       

    }
}
