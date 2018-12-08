using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnknownWorld.Visualizer
{
    public class AudioSourceVisualizer : MonoBehaviour {

        [SerializeField] private UnknownWorld.Sound.SoundContainer m_audios;
        [SerializeField] private Color m_colorZone = Color.black;
        [SerializeField] private Color m_colorNoiseBad = Color.red;
        [SerializeField] private Color m_colorNoiseGood = Color.green;
        [SerializeField] private bool m_drawDetection = true;
        [SerializeField] private bool m_drawOnSelected = true;
        [SerializeField] private int m_drawZone = 0;
        [SerializeField] private float m_step = 0.1f;
        [SerializeField] private float m_alpha = 0.05f;
        

        private void OnDrawGizmos()
        {
            if ((m_drawOnSelected)||
                (m_audios.Audios.Count == 0) ||
                ((m_drawZone < 0) || (m_drawZone >= m_audios.Audios.Count))) return;

            DrawZone();
        }

        private void OnDrawGizmosSelected()
        {
            if ((!m_drawOnSelected) ||
                (m_audios.Audios.Count == 0) ||
                ((m_drawZone < 0) || (m_drawZone >= m_audios.Audios.Count))) return;

            DrawZone();
        }


        private void DrawZone()
        {
#if UNITY_EDITOR
            if (m_drawDetection)
            {
                float progress;

                for (float i = m_audios.Audios[m_drawZone].OutherRadiusDetection; i >= m_audios.Audios[m_drawZone].InnerRadiusDetection; i -= m_step)
                {
                    progress = 1 - UnknownWorld.Utility.Methods.VectorOperations.Map(i, m_audios.Audios[m_drawZone].InnerRadiusDetection, m_audios.Audios[m_drawZone].OutherRadiusDetection, 0, 1);

                    UnityEditor.Handles.color = new Color(
                        Mathf.Lerp(m_colorNoiseBad.r, m_colorNoiseGood.r, progress),
                        Mathf.Lerp(m_colorNoiseBad.g, m_colorNoiseGood.g, progress),
                        Mathf.Lerp(m_colorNoiseBad.b, m_colorNoiseGood.b, progress), m_alpha);

                    UnityEditor.Handles.DrawSolidArc(transform.position, Vector3.up, transform.position, 360, i);
                }
            }
            else
            {
                UnityEditor.Handles.color = m_colorZone;

                UnityEditor.Handles.DrawWireArc(transform.position,
                    Vector3.up, Vector3.forward, 360, m_audios.Audios[m_drawZone].InnerRadiusDetection);
                UnityEditor.Handles.DrawWireArc(transform.position,
                    Vector3.up, Vector3.forward, 360, m_audios.Audios[m_drawZone].OutherRadiusDetection);
            }
#endif
        }
    }
}