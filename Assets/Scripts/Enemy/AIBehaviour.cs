using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnknownWorld.Path.Data;
using UnknownWorld.Area.Data;

namespace UnknownWorld.Behaviour
{
    [System.Serializable]
    public class AIBehaviour : PersonBehaviour
    {
        public enum AIState
        {
            Unknown,
            FollowingPath,
            ReturningPath,
            FollowingSuspicion,
            FollowingTarget,
            EscapingTarget,
            Waiting,
            Attacking,
            Dead
        }

        [SerializeField] [Range(0, ushort.MaxValue)] private float m_attackDistance = 1.0f;
        [SerializeField] [Range(0, 1)] private float m_targetUpdateDelay = 0.1f;
        

        private UnknownWorld.Utility.Data.DataState m_affectionDataState = Utility.Data.DataState.Unknown;
        private List<UnknownWorld.Area.Data.AreaAffectionMask> m_affectionInfo;
        private List<UnknownWorld.Path.Data.PathPoint> m_possibleTargets;
        private List<UnknownWorld.Area.Observer.SearchingArea> m_areas;
        protected UnknownWorld.Manager.AreaManager m_areaManager;        
        private AIState m_state = AIState.FollowingPath;
        private bool m_isSuspicionTargetDetected;
        private bool m_isDirectTargetDetected;
        private Coroutine m_updateCorotation;
        private CapsuleCollider m_colider;


        public List<UnknownWorld.Area.Observer.SearchingArea> Areas
        {
            get
            {
                return this.m_areas ??
                    (this.m_areas = new List<UnknownWorld.Area.Observer.SearchingArea>());
            }
            set { this.m_areas = value; }
        }
        public UnknownWorld.Utility.Data.DataState AffectionState
        {
            get { return this.m_affectionDataState; }
            set { this.m_affectionDataState = value; }
        }
        public List<AreaAffectionMask> AffectionInfo
        {
            get
            {
                return this.m_affectionInfo ??
                    (this.m_affectionInfo = new List<AreaAffectionMask>());
            }
            set { m_affectionInfo = value; }
        }
        public List<PathPoint> PossibleTargets
        {
            get
            {
                return this.m_possibleTargets ??
                    (this.m_possibleTargets = new List<UnknownWorld.Path.Data.PathPoint>());
            }
            set { this.m_possibleTargets = value; }
        }
        public bool IsSuspicionTargetDetected
        {
            get { return this.m_isSuspicionTargetDetected; }
        }
        public bool IsDirectTargetDetected
        {
            get { return this.m_isDirectTargetDetected; }
        }
        public CapsuleCollider Colider
        {
            get { return this.m_colider; }
        }
        public bool IsManagerActive
        {
            get { return this.m_areaManager.IsActive; }
        }
        public AIState State
        {
            get { return this.m_state; }
            set { this.m_state = value; }
        }
                

        protected override void Awake()
        {
            base.Awake();
            /* AI specific initialization */
            m_areaManager = GetComponentInParent<UnknownWorld.Manager.AreaManager>();
            m_areas = GetComponents<UnknownWorld.Area.Observer.SearchingArea>().
                OfType<UnknownWorld.Area.Observer.SearchingArea>().ToList();

            m_colider = GetComponent<CapsuleCollider>();
        }

        protected override void Update()
        {
            base.Update();
        }


        private IEnumerator UpdateTargets(float delay)
        {
            while (true)
            {
                yield return new WaitForSeconds(delay);

                if ((AffectionInfo.Count == 0) ||
                    (m_affectionDataState != Utility.Data.DataState.Updated)) continue;

                PossibleTargets.Clear();

                UnknownWorld.Path.Data.PathPoint temp;
                m_isSuspicionTargetDetected = false;
                m_isDirectTargetDetected = false;
                

                for (int i = 0; i < m_affectionInfo.Count; i++)
                {
                    // check if there some character's affection point was detected
                    if (!m_affectionInfo[i].AffectedMask.Cast<bool>().Contains(true)) // can be replaced by for
                        continue;

                    // getting character affected point
                    temp = new PathPoint(
                        m_areaManager.GetTargetInfo(m_affectionInfo[i].AreaAddresses.TargetId).FollowingPoint);

                    for (int j = 0; j < m_areas.Count; j++)
                    {
                        if (m_affectionInfo[i].AreaAddresses.AreaId == m_areas[j].Id)
                        {
                            temp.Action = PathHelper.ObservationToAction(m_areas[j].Type);
                            temp.Type = PathHelper.ObservationToPoint(m_areas[j].Type);
                            temp.Priority = m_areas[j].Priority;                                                       

                            if (temp.Type == PointType.FollowingSuspicion)
                            {
                                Vector3 position = temp.Transform.position;
                                temp.Point = Instantiate(UnknownWorld.Path.Data.PathHelper.PathPrefab, PathHelper.PathSpawner.transform).
                                    GetComponent<Path.IntermediatePoint>();
                                temp.Transform.position = position;

                                temp.TransferDelay = UnknownWorld.Path.Data.PathHelper.PathWaitTime;                                
                                
                                if (!m_isSuspicionTargetDetected)
                                    m_isSuspicionTargetDetected = true;
                                break;
                            }

                            // change to provide attack radius based on attack distance
                            temp.AccuracyRadius = m_attackDistance;

                            if (!m_isDirectTargetDetected)
                                m_isDirectTargetDetected = true;
                            break;
                        }
                    }
                    PossibleTargets.Add(temp);
                }
                m_affectionDataState = Utility.Data.DataState.Transferred;
            }
        }

        protected override void SetIsActive(bool isActive)
        {
            if (IsActive == isActive) return;

            base.SetIsActive(isActive);

            if (!isActive)
            {
                StopCoroutine(m_updateCorotation);
                m_areaManager.ClearMasks(Id);
            }
            else
            {
                m_updateCorotation = StartCoroutine("UpdateTargets", m_targetUpdateDelay);
            }
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

        public bool IsTargetActive(int targetPosition)
        {
            return m_areaManager.Targets[targetPosition].Subject.IsActive;
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
