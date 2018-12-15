using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnknownWorld.Manager
{
    public class NavigationManager : MonoBehaviour {

        private List<UnknownWorld.Navigation.NavigationRegion> m_regions;

        public List<UnknownWorld.Navigation.NavigationRegion> Regions
        {
            get
            {
                return (this.m_regions) ?? 
                    (this.m_regions = new List<UnknownWorld.Navigation.NavigationRegion>());
            }
            set
            {
                this.m_regions = value;
            }
        }

        private void Awake()
        {
            m_regions = FindObjectsOfType<UnknownWorld.Navigation.NavigationRegion>().
                OfType<UnknownWorld.Navigation.NavigationRegion>().ToList();
        }

        public void Rebake(uint id)
        {
            for (int i = 0; i < m_regions.Count; i++)
            {
                if (m_regions[i].Id == id)
                {
                    m_regions[i].Rebake();
                    return;
                }                
            }
        }
    }
}