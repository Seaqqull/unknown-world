using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnknownWorld
{
    public class CircleArea : MonoBehaviour
    {
        public SimpleData m_data;

        private SimpleInteractionAnnunciator m_warden;

        private List<CircleTarget> m_affectedTargets;
        public List<CircleTarget> AffectedTargets
        {
            get
            {
                if (this.m_affectedTargets == null)
                {
                    this.m_affectedTargets = new List<CircleTarget>();
                }
                return this.m_affectedTargets;
            }

            set
            {
                this.m_affectedTargets = value;
            }
        }

        private List<bool[]> m_affectedMask;
        public List<bool[]> AffectedMask
        {
            get
            {
                if (this.m_affectedMask == null)
                {
                    this.m_affectedMask = new List<bool[]>(m_targetManager.Targets.Length);
                }
                return this.m_affectedMask;
            }

            set
            {
                this.m_affectedMask = value;
            }
        }

        private CircleTargetManager m_targetManager;

        private float m_searchDelay = 0.2f;
        private Coroutine m_coroutine;

        public bool m_isAreaActive = true;
        private bool m_isActive;
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
                    this.m_coroutine = StartCoroutine("FindTargetsWithDelay", this.m_searchDelay);
                }
            }
        }


        // Use this for initialization
        private void Start()
        {
            m_data.Transform = GetComponent<Transform>();
            m_warden = new SimpleInteractionAnnunciator();
            m_targetManager = GetComponentInParent<CircleTargetManager>();

            IsActive = m_isAreaActive;

            initializeAffectedMask();
        }

        private void initializeAffectedMask()
        {
            AffectedMask.Clear();
            for (int i = 0; i < m_targetManager.Targets.Length; i++) {
                m_affectedMask.Add(new bool[m_targetManager.Targets[i].Points.TracePoints.Length]);
            }
        }

        private void clearAffectedMask()
        {
            for (int i = 0; i < m_targetManager.Targets.Length; i++)
            {
                System.Array.Clear(m_affectedMask[i], 0, m_affectedMask[i].Length);
            }
        }

        // Update is called once per frame
        private void Update()
        {

        }

        public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
        {
            if (!angleIsGlobal)
            {
                angleInDegrees += transform.eulerAngles.y;
            }
            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }

        IEnumerator FindTargetsWithDelay(float delay)
        {
            while (true)
            {
                yield return new WaitForSeconds(delay);
                AffectedTargets.Clear();
                clearAffectedMask();

                for (int i = 0; i < m_targetManager.Targets.Length; i++)
                {
                    if (m_warden.isTargetWithinArea(m_data, m_targetManager.Targets[i].Points.TracePoints, m_affectedMask[i]))
                    {
                        m_affectedTargets.Add(m_targetManager.Targets[i]);
                    }                                                  
                }
            }
        }
    }
}
