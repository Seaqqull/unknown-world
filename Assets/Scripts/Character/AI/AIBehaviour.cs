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
        protected UnknownWorld.Manager.AreaManager m_areaManager;

        public bool IsManagerActive
        {
            get
            {
                return m_areaManager.IsActive;
            }
        }
        public List<UnknownWorld.Area.Observer.SearchingArea> Areas
        {
            get
            {
                return this.m_areas ?? 
                    (this.m_areas = new List<UnknownWorld.Area.Observer.SearchingArea>());
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
            m_areaManager = GetComponentInParent<UnknownWorld.Manager.AreaManager>();
            m_areas = GetComponents<UnknownWorld.Area.Observer.SearchingArea>().
                OfType<UnknownWorld.Area.Observer.SearchingArea>().ToList();
        }

        protected override void Update()
        {
            base.Update();
            IsActive = m_isPersonActive;// only for editor
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

        protected override void SetIsActive(bool isActive)
        {
            if (m_isActive == isActive) return;

            base.SetIsActive(isActive);

            if(!isActive)
                m_areaManager.ClearMasks(m_id);
        }

        public BitArray GetMask(uint targetId, uint areaId)
        {
            return m_areaManager.GetMask(targetId, this.Id, areaId);
        }

        public UnknownWorld.Area.Target.TracingAreaContainer GetAreaContainer(int targetPosition)
        {
            return m_areaManager.Targets[targetPosition].AreaContainer;
        }

        public bool IsTargetActive(int targetPosition)
        {
            return m_areaManager.Targets[targetPosition].Subject.IsActive;
        }

    }
}
