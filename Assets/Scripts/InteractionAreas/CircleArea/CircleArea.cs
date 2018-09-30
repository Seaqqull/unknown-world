using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnknownWorld
{
    public class CircleArea : MonoBehaviour
    {
        private SimpleInteractionAnnunciator m_warden;

        [HideInInspector]
        public List<CircleTarget> m_affectedTargets;
        [HideInInspector]
        public CircleTargetManager m_targetManager;        
        public SimpleData m_data;
        

        private float m_searchDelay = 0.2f;


        // Use this for initialization
        private void Start()
        {
            m_data.Transform = GetComponent<Transform>();
            m_warden = new SimpleInteractionAnnunciator();
            m_targetManager = GetComponentInParent<CircleTargetManager>();

            StartCoroutine("FindTargetsWithDelay", m_searchDelay);
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
                m_affectedTargets.Clear();

                for (int i = 0; i < m_targetManager.m_targets.Length; i++)
                {
                    for (int j = 0; j < m_targetManager.m_targets[i].m_points.TracePoints.Length; j++)
                    {
                        if (m_warden.isTargetWithinArea(m_data, m_targetManager.m_targets[i].m_points.TracePoints[j]))
                        {
                            m_affectedTargets.Add(m_targetManager.m_targets[i]);
                            m_targetManager.m_targets[i].m_points.TracePoints[j].IsAreaAccessible = true;
                        }
                        else
                            m_targetManager.m_targets[i].m_points.TracePoints[j].IsAreaAccessible = false;
                    }
                    
                    
                }
            }
        }
    }
}
