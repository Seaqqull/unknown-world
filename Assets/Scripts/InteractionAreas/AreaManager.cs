using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnknownWorld.Area
{
    [System.Serializable]
    public class AreaManager : MonoBehaviour
    {
        [SerializeField] private List<UnknownWorld.Area.Data.AreaTarget> m_targets;
        [SerializeField] private float m_searchingDelay = 0.3f;
        [SerializeField] private bool m_isManagerActive = true;

        private List<UnknownWorld.Area.Data.AreaAffectionMask> m_areasMask;
        private List<UnknownWorld.Behaviour.AIBehaviour> m_cameras;
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
                if (this.m_areasMask == null)
                {
                    this.m_areasMask = new List<UnknownWorld.Area.Data.AreaAffectionMask>();
                }
                return this.m_areasMask;
            }
        }
        public List<UnknownWorld.Behaviour.AIBehaviour> Cameras
        {
            get
            {
                if (this.m_cameras == null)
                {
                    this.m_cameras = new List<UnknownWorld.Behaviour.AIBehaviour>();
                }
                return this.m_cameras;
            }
        }
        public List<UnknownWorld.Area.Data.AreaTarget> Targets
        {
            get
            {
                if (this.m_targets == null)
                {
                    this.m_targets = new List<UnknownWorld.Area.Data.AreaTarget>();
                }
                return this.m_targets;
            }
        }
        public BitArray AffectedTargetMask
        {
            get
            {
                if (this.m_affectedTargetMask == null)
                {
                    this.m_affectedTargetMask = new BitArray(0);
                }
                return this.m_affectedTargetMask;
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
                }
                else
                {
                    this.m_coroutine = StartCoroutine("FindAffectedTargetsWithDelay", this.m_searchingDelay);
                }
            }
        }
        public uint Id
        {
            get
            {
                return this.m_id;
            }
        }
        

        protected virtual void Awake()
        {
            IsActive = m_isManagerActive;
            m_id = m_idCounter++;
        }

        protected virtual void Start()
        {
            m_cameras = GetComponentsInChildren<UnknownWorld.Behaviour.AIBehaviour>().
                OfType<UnknownWorld.Behaviour.AIBehaviour>().ToList();
            
            m_affectedTargetMask = new BitArray(Targets.Count); 


            for (int i = 0; i < m_targets.Count; i++)
            {
                for (int j = 0; j < m_cameras.Count; j++)
                {
                    for (int k = 0; k < m_cameras[j].Areas.Count; k++)
                    {
                        AreasMask.Add(new UnknownWorld.Area.Data.AreaAffectionMask(
                            m_targets[i].Points.TracingPoints.Length,
                            m_targets[i].Subject.Id, m_cameras[j].Id, m_cameras[j].Areas[k].Id));
                    }                    
                }                
            }

            UpdateTargetsIndexes();
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
                if (m_areasMask[i].AreaAddresses.targetId == targetId) indexes.Add(i);
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

        public void ClearMasks(uint cameraId, uint areaId)
        {
            for (int i = 0; i < AreasMask.Count; i++)
            {
                if (AreasMask[i].AreaAddresses.areaId == areaId &&
                    AreasMask[i].AreaAddresses.cameraId == cameraId)
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
                    BitArray currentTargetMask = new BitArray(m_targets[i].Subject.AreaContainer.TracingPoints.Length);
                    for (int j = 0; j < m_targetIndexes[i].Length; j++)
                    {
                        currentTargetMask.Or(m_areasMask[m_targetIndexes[i][j]].AffectedMask);
                    }
                    return currentTargetMask;
                }
            }
            return null;
        }

        public void ClearMask(uint targetId, uint cameraId, uint areaId)
        {
            for (int i = 0; i < AreasMask.Count; i++)
            {
                if (AreasMask[i].AreaAddresses.targetId == targetId &&
                    AreasMask[i].AreaAddresses.areaId == areaId &&
                    AreasMask[i].AreaAddresses.cameraId == cameraId)
                {
                    AreasMask[i].AffectedMask.SetAll(false);
                    break;
                }
            }
        }

        public int? GetMaskSize(uint targetId, uint cameraId, uint areaId)
        {
            for (int i = 0; i < AreasMask.Count; i++)
            {
                if (AreasMask[i].AreaAddresses.targetId == targetId &&
                    AreasMask[i].AreaAddresses.areaId == areaId &&
                    AreasMask[i].AreaAddresses.cameraId == cameraId)
                {
                    return AreasMask[i].AffectedMask.Length;
                }
            }
            return null;
        }

        public BitArray GetMask(uint targetId, uint cameraId, uint areaId)
        {
            for (int i = 0; i < AreasMask.Count; i++)
            {
                if (AreasMask[i].AreaAddresses.targetId == targetId &&
                    AreasMask[i].AreaAddresses.areaId == areaId &&
                    AreasMask[i].AreaAddresses.cameraId == cameraId)
                {
                    return AreasMask[i].AffectedMask;
                }
            }
            return null;
        }
        
        public void SetMask(uint targetId, uint cameraId, uint areaId, BitArray mask)
        {
            for (int i = 0; i < AreasMask.Count; i++)
            {
                if (AreasMask[i].AreaAddresses.targetId == targetId &&
                    AreasMask[i].AreaAddresses.areaId == areaId &&
                    AreasMask[i].AreaAddresses.cameraId == cameraId)
                {
                    AreasMask[i].AffectedMask = mask;
                    break;
                }
            }
        }
        
    }
}
