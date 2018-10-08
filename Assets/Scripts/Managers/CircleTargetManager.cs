using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnknownWorld
{
    public class CircleTargetManager : MonoBehaviour
    {
        [SerializeField]
        private CircleTarget[] m_targets;
        public CircleTarget[] Targets
        {
            get
            {
                return this.m_targets;
            }

            set
            {
                this.m_targets = value;
            }
        }
    }
}
