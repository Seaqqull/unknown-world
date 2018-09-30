using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnknownWorld
{
    [CustomEditor(typeof(CircleArea))]
    public class SimpleInteractionVizualizer : Editor
    {
        private CircleArea area;
        public Color m_circleColor = Color.white, 
                     m_lineColor = Color.red;


        private void OnSceneGUI()
        {
            area = (CircleArea)target;

            Handles.color = m_circleColor;
            Handles.DrawWireArc(area.transform.position, Vector3.up, Vector3.forward, 360, area.m_data.m_radius);
            Vector3 viewAngleA = area.DirFromAngle(-area.m_data.m_angle / 2, false);
            Vector3 viewAngleB = area.DirFromAngle(area.m_data.m_angle / 2, false);

            Handles.DrawLine(area.transform.position, area.transform.position + viewAngleA * area.m_data.m_radius);
            Handles.DrawLine(area.transform.position, area.transform.position + viewAngleB * area.m_data.m_radius);

            ShowTargets();            
        }

        private void ShowTargets()
        {            
            Handles.color = m_lineColor;

            foreach (CircleTarget visibleTarget in area.m_affectedTargets)
            {
                for (int i = 0; i < visibleTarget.m_points.TracePoints.Length; i++) {
                    if(visibleTarget.m_points.TracePoints[i].IsAreaAccessible)
                        Handles.DrawLine(area.transform.position, visibleTarget.m_points.TracePoints[i].transform.position);
                }
                
            }
        }        
    }
}

