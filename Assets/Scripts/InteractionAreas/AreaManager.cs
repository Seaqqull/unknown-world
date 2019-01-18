using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnknownWorld.Manager
{
    [System.Serializable]
    public class AreaManager : MonoBehaviour
    {
        [SerializeField] private bool m_isManagerActive = true;

        [SerializeField] [Range(0.0f, 1.0f)] private float m_searchingDelay = 0.3f;

        [SerializeField] private List<UnknownWorld.Area.Data.AreaTarget> m_targets;        
        
        private List<UnknownWorld.Area.Data.AreaAffectionMask> m_areasMask;
        private List<UnknownWorld.Behaviour.AIBehaviour> m_observers;
        private NavigationManager m_navigationManager;
        private BitArray m_affectedTargetMask; // Update every time, when target.count changed        
        protected static uint m_idCounter = 0;
        private int[][] m_targetIndexes;
        private Coroutine m_coroutine;
        protected bool m_isActive;
        protected uint m_id;

        public List<UnknownWorld.Area.Data.AreaAffectionMask> AreasMask
        {
            get
            {
                return this.m_areasMask ?? 
                    (this.m_areasMask = new List<UnknownWorld.Area.Data.AreaAffectionMask>());
            }
        }
        public List<UnknownWorld.Behaviour.AIBehaviour> Observers
        {
            get
            {
                return this.m_observers ?? 
                    (this.m_observers = new List<UnknownWorld.Behaviour.AIBehaviour>());
            }
        }
        public List<UnknownWorld.Area.Data.AreaTarget> Targets
        {
            get
            {
                return this.m_targets ??
                    (this.m_targets = new List<UnknownWorld.Area.Data.AreaTarget>());
            }
        }
        public BitArray AffectedTargetMask
        {
            get
            {
                return this.m_affectedTargetMask ??
                    (this.m_affectedTargetMask = new BitArray(0));
            }
        }
        public int[][] TargetIndexes
        {
            get
            {
                return this.m_targetIndexes;
            }
        }
        public bool IsActive
        {
            get
            {
                return this.m_isActive;
            }

            set
            {
                if (value == this.m_isActive)
                    return;

                this.m_isActive = value;

                if (!this.m_isActive)
                {
                    StopCoroutine(this.m_coroutine);

                    for (int i = 0; i < m_observers.Count; i++)
                    {
                        ClearMasks(m_observers[i].Id);
                    }
                }
                else
                {
                    this.m_coroutine = StartCoroutine("FindAffectedTargetsWithDelay", this.m_searchingDelay);
                }
            }
        }
        public uint Id
        {
            get { return this.m_id; }
        }
        

        protected virtual void Awake()
        {
            IsActive = m_isManagerActive;
            m_id = m_idCounter++;

            m_navigationManager = FindObjectOfType<NavigationManager>();
        }

        protected virtual void Start()
        {
            m_observers = GetComponentsInChildren<UnknownWorld.Behaviour.AIBehaviour>().
                OfType<UnknownWorld.Behaviour.AIBehaviour>().ToList();
            
            m_affectedTargetMask = new BitArray(Targets.Count); 


            for (int i = 0; i < m_targets.Count; i++)
            {
                m_targets[i].UpdateAreaManager();
                for (int j = 0; j < m_observers.Count; j++)
                {
                    for (int k = 0; k < m_observers[j].Areas.Count; k++)
                    {
                        AreasMask.Add(new UnknownWorld.Area.Data.AreaAffectionMask(
                            m_targets[i].AreaContainer.TracingAreas.Length,
                            m_targets[i].Subject.Id, m_observers[j].Id, m_observers[j].Areas[k].Id, m_observers[j].Areas[k].Type));
                    }                    
                }                
            }

            UpdateTargetsIndexes();
        }

        protected virtual void Update()
        {
#if UNITY_EDITOR
            IsActive = m_isManagerActive;
#endif
        }

        // call every time, when AreaMasks have been changed to update indexes of target in affection mask
        protected void UpdateTargetsIndexes()
        {
            List<int> tmpIndexes;
            m_targetIndexes = new int[m_targets.Count][];

            for (int i = 0; i < m_targets.Count; i++)
            {
                tmpIndexes = GetTargetIndexesInAffectionMask(m_targets[i].Subject.Id);
                m_targetIndexes[i] = tmpIndexes.ToArray();
            }
        }

        protected List<int> GetTargetIndexesInAffectionMask(uint targetId)
        {
            List<int> indexes = new List<int>();
            for (int i = 0; i < m_areasMask.Count; i++)
            {
                if (m_areasMask[i].AreaAddresses.TargetId == targetId) indexes.Add(i);
            }
            return indexes;
        }

        protected virtual IEnumerator FindAffectedTargetsWithDelay(float delay)
        {
            while (true)
            {
                yield return new WaitForSeconds(delay);

                bool isTargetAffected;

                for (int i = 0; i < m_targetIndexes.Length; i++) // possible targets
                {
                    isTargetAffected = false;
                    
                    for (int j = 0; j < m_targetIndexes[i].Length; j++) // found indexes
                    {                        
                        for (int k = 0; k < m_areasMask[m_targetIndexes[i][j]].AffectedMask.Length; k++) // mask
                        {
                            if (m_areasMask[m_targetIndexes[i][j]].AffectedMask[k]) isTargetAffected = true;
                        }
                    }
                    m_affectedTargetMask[i] = isTargetAffected;
                }

                // new
                for (int i = 0; i < m_observers.Count; i++)
                {
                    if ((!m_observers[i].IsDeath) ||
                        (m_observers[i].AffectionState == Utility.Data.DataState.Unknown) ||
                        (m_observers[i].AffectionState == Utility.Data.DataState.Processed)) // only if first time or previous data was processed
                    {
                        m_observers[i].AffectionInfo = GetObserverAffectionInfo(m_observers[i].Id);
                        m_observers[i].AffectionState = Utility.Data.DataState.Updated;
                    }
                }
            }
        }


        public void MakeAllAIAsObstacle()
        {
            for (int i = 0; i < m_observers.Count; i++)
            {
                m_observers[i].IsObstacle = true;
            }
        }

        public void RevokeAllAIAsObstacle()
        {
            for (int i = 0; i < m_observers.Count; i++)
            {
                m_observers[i].IsObstacle = false;
            }
        }

        public void RebakeNavigation(uint id)
        {
            m_navigationManager.Rebake(id);
        }

        public void MakeAIAsObstacle(uint aiId)
        {
            for (int i = 0; i < m_observers.Count; i++)
            {
                if (m_observers[i].Id == aiId)
                {
                    m_observers[i].IsObstacle = true;
                    return;
                }                
            }
        }

        public void RevokeAIAsObstacle(uint aiId)
        {
            for (int i = 0; i < m_observers.Count; i++)
            {
                if (m_observers[i].Id == aiId)
                {
                    m_observers[i].IsObstacle = false;
                    return;
                }
            }
        }

        public void MakeAllAIAsObstacleExcept(uint aiId)
        {
            for (int i = 0; i < m_observers.Count; i++)
            {
                if (m_observers[i].Id != aiId)
                    m_observers[i].IsObstacle = true;
            }
        }
        
        public void RevokeAllAIAsObstacleExcept(uint aiId)
        {
            for (int i = 0; i < m_observers.Count; i++)
            {
                if (m_observers[i].Id != aiId)
                    m_observers[i].IsObstacle = false;
            }
        }


        public void ClearMasks(uint observerId)
        {
            for (int i = 0; i < AreasMask.Count; i++)
            {
                if (AreasMask[i].AreaAddresses.ObserverId == observerId)
                    AreasMask[i].AffectedMask.SetAll(false);
            }
        }

        public void ClearByTargetId(uint targetId)
        {
            for (int i = 0; i < AreasMask.Count; i++)
            {
                if (AreasMask[i].AreaAddresses.TargetId == targetId)
                    AreasMask[i].AffectedMask.SetAll(false);
            }
        }

        public HashSet<uint> GetAffectedCharactersId()
        {
            HashSet<uint> ids = new HashSet<uint>();
            for (int i = 0; i < m_affectedTargetMask.Count; i++)
            {
                if (m_affectedTargetMask[i]) ids.Add(m_targets[i].Subject.Id);
            }
            return ids;
        }

        public void ClearMasks(uint observerId, uint areaId)
        {
            for (int i = 0; i < AreasMask.Count; i++)
            {
                if (AreasMask[i].AreaAddresses.AreaId == areaId &&
                    AreasMask[i].AreaAddresses.ObserverId == observerId)
                {
                    AreasMask[i].AffectedMask.SetAll(false);
                }
            }
        }

        public BitArray GetHitMaskByCharacterId(uint targetId)
        {
            for (int i = 0; i < m_targets.Count; i++)
            {
                if (m_targets[i].Subject.Id == targetId && m_affectedTargetMask[i])
                {
                    BitArray currentTargetMask = new BitArray(m_targets[i].Subject.AreaContainer.TracingAreas.Length);
                    for (int j = 0; j < m_targetIndexes[i].Length; j++)
                    {
                        currentTargetMask.Or(m_areasMask[m_targetIndexes[i][j]].AffectedMask);
                    }
                    return currentTargetMask;
                }
            }
            return null;
        }

        public bool IsTargetValid(Path.IntermediatePoint point)
        {
            for (int i = 0; i < m_targets.Count; i++)
            {
                if (m_targets[i].Subject.FollowingPoint.Point == point)
                {
                    return !m_targets[i].Subject.IsDeath;
                }
            }

            return false;
        }

        public void ClearMask(uint targetId, uint observerId, uint areaId)
        {
            for (int i = 0; i < AreasMask.Count; i++)
            {
                if (AreasMask[i].AreaAddresses.AreaId == areaId &&
                    AreasMask[i].AreaAddresses.TargetId == targetId &&
                    AreasMask[i].AreaAddresses.ObserverId == observerId)
                {
                    AreasMask[i].AffectedMask.SetAll(false);
                    break;
                }
            }
        }

        public int? GetMaskSize(uint targetId, uint observerId, uint areaId)
        {
            for (int i = 0; i < AreasMask.Count; i++)
            {
                if (AreasMask[i].AreaAddresses.AreaId == areaId &&
                    AreasMask[i].AreaAddresses.TargetId == targetId &&
                    AreasMask[i].AreaAddresses.ObserverId == observerId)
                {
                    return AreasMask[i].AffectedMask.Length;
                }
            }
            return null;
        }

        public BitArray GetMask(uint targetId, uint observerId, uint areaId)
        {
            for (int i = 0; i < AreasMask.Count; i++)
            {
                if (AreasMask[i].AreaAddresses.AreaId == areaId &&
                    AreasMask[i].AreaAddresses.TargetId == targetId &&
                    AreasMask[i].AreaAddresses.ObserverId == observerId)
                {
                    return AreasMask[i].AffectedMask;
                }
            }
            return null;
        }

        public UnknownWorld.Behaviour.CharacterBehaviour GetTargetInfo(uint targetId)
        {
            for (int i = 0; i < Targets.Count; i++)
            {
                if (Targets[i].Subject.Id == targetId)
                    return Targets[i].Subject;
            }
            return null;
        }

        public void SetMask(uint targetId, uint observerId, uint areaId, BitArray mask)
        {
            for (int i = 0; i < AreasMask.Count; i++)
            {
                if (AreasMask[i].AreaAddresses.AreaId == areaId &&
                    AreasMask[i].AreaAddresses.TargetId == targetId &&
                    AreasMask[i].AreaAddresses.ObserverId == observerId)
                {
                    AreasMask[i].AffectedMask = mask;
                    break;
                }
            }
        }

        public List<UnknownWorld.Area.Data.AreaAffectionMask> GetAreaAffectionInfo(uint areaId)
        {
            List<UnknownWorld.Area.Data.AreaAffectionMask> findedTargets = new List<Area.Data.AreaAffectionMask>();

            for (int i = 0; i < m_areasMask.Count; i++)
            {
                if (m_areasMask[i].AreaAddresses.AreaId == areaId)
                    findedTargets.Add(m_areasMask[i]);
            }
            return findedTargets;
        }

        public List<UnknownWorld.Area.Data.AreaAffectionMask> GetTargetAffectionInfo(uint targetId)
        {
            List<UnknownWorld.Area.Data.AreaAffectionMask> findedTargets = new List<Area.Data.AreaAffectionMask>();

            for (int i = 0; i < m_areasMask.Count; i++)
            {
                if (m_areasMask[i].AreaAddresses.TargetId == targetId)
                    findedTargets.Add(m_areasMask[i]);
            }
            return findedTargets;
        }

        public List<UnknownWorld.Area.Data.AreaAffectionMask> GetObserverAffectionInfo(uint observerId)
        {
            List<UnknownWorld.Area.Data.AreaAffectionMask> findedTargets = new List<Area.Data.AreaAffectionMask>();

            for (int i = 0; i < m_areasMask.Count; i++)
            {
                if (m_areasMask[i].AreaAddresses.ObserverId == observerId)
                    findedTargets.Add(m_areasMask[i]);
            }
            return findedTargets;
        }

        public UnknownWorld.Area.Data.AreaAffectionMask GetSingleAffectionInfo(uint targetId, uint observerId, uint areaId)
        {
            for (int i = 0; i < m_areasMask.Count; i++)
            {
                if ((m_areasMask[i].AreaAddresses.AreaId == areaId) &&
                    (m_areasMask[i].AreaAddresses.TargetId == targetId) &&
                    (m_areasMask[i].AreaAddresses.ObserverId == observerId))
                {
                    return m_areasMask[i];
                }
            }
            return null;
        }

    }
}
