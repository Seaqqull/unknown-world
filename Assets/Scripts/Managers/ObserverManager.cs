using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnknownWorld.Manager
{
    public class ObserverManager : MonoBehaviour
    {
        private List<UnknownWorld.Behaviour.CharacterBehaviour> m_characters;       
        private List<UnknownWorld.Manager.AreaManager> m_areas;
        private List<BitArray> m_characterHitMask;
        private Coroutine m_coroutine;
        protected bool m_isActive;

        public bool m_isAreaActive = true;
        [Range(0.0f, 1.0f)] public float m_searchingDelay = 0.15f;

        public List<BitArray> CharacterHitMask
        {
            get
            {
                return this.m_characterHitMask ??
                    (this.m_characterHitMask = new List<BitArray>());
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
                    this.m_coroutine = StartCoroutine("SearchForAffectedCharacters", this.m_searchingDelay);
                }
            }
        }


        private void Start()
        {
            m_characters = FindObjectsOfType<UnknownWorld.Behaviour.CharacterBehaviour>().
                OfType<UnknownWorld.Behaviour.CharacterBehaviour>().ToList();
            m_areas = FindObjectsOfType<UnknownWorld.Manager.AreaManager>().
                OfType<UnknownWorld.Manager.AreaManager>().ToList();
            IsActive = m_isAreaActive;

            UpdateCharacterHitMask();            
        }
        
        private void Update()
        {

        }


        private void ClearCharacterHitMask()
        {
            for (int i = 0; i < m_characterHitMask.Count; i++)
            {
                m_characterHitMask[i].SetAll(false);
            }
        }

        // Call every time, when characters count have been changed
        private void UpdateCharacterHitMask()
        {
            CharacterHitMask.Clear();
            
            for (int i = 0; i < m_characters.Count; i++)
            {
                m_characterHitMask.Add(new BitArray(m_characters[i].AreaContainer.TracingAreas.Length));
            }            
        }        

        private int GetCharacterPositionById(uint searchingId)
        {
            for (int i = 0; i < m_characters.Count; i++)
            {
                if (m_characters[i].Id == searchingId) return i;
            }
            return -1;
        }

        private IEnumerator SearchForAffectedCharacters(float delay)
        {
            while (true)
            {
                yield return new WaitForSeconds(delay);

                HashSet<uint> affectedCharacterIds = new HashSet<uint>();
                ClearCharacterHitMask();

                for (int i = 0; i < m_areas.Count; i++) // get all affected characters
                {
                    affectedCharacterIds.UnionWith(m_areas[i].GetAffectedCharactersId());
                }//getHitMaskByCharacterId

                foreach (uint characterId in affectedCharacterIds) // get characters affected ids
                {
                    for (int j = 0; j < m_areas.Count; j++)
                    {
                        BitArray affectedCharacterMask = m_areas[j].GetHitMaskByCharacterId(characterId);

                        if (affectedCharacterMask != null)
                        {
                            m_characterHitMask[GetCharacterPositionById(characterId)].Or(affectedCharacterMask);
                        }
                    }
                }

                for (int i = 0; i < m_characterHitMask.Count; i++) // set all characters point state
                {
                    for (int j = 0; j < m_characterHitMask[i].Count; j++)
                    {
                        if ((!m_characters[i].IsActive) ||
                            (m_characters[i].AreaContainer.TracingAreas[j].State == Area.Data.HitAreaState.Disabled))
                            continue;

                        m_characters[i].AreaContainer.SetAreaState(j, 
                            (!m_characterHitMask[i][j]) ? Area.Data.HitAreaState.Enabled : Area.Data.HitAreaState.Accessible);
                    }
                }
            }
        }


        public void ClearCharacterMask(uint characterId)
        {
            for (int i = 0; i < m_areas.Count; i++)
            {
                m_areas[i].ClearByTargetId(characterId);
            }
        }

    }
}
