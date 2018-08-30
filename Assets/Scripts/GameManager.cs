using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnknownWorld
{
    [System.Serializable]
    public class CameraKeeperData {
        public Transform m_target;
        public Transform m_lookAt;
    }


    public class GameManager : MonoBehaviour
    {

        public CameraController[] m_camerasInLevel; // at [0] default camera
        public CameraKeeperData[] m_cameraTagets; // at [0] always must be the player

        private int? m_activeCamera = null;
        public int m_targetCamera = -1;

        // Use this for initialization
        void Start()
        {
            SetCameraTargetFromPlayer();
        }

        // Update is called once per frame
        void Update()
        {
            if (m_activeCamera.HasValue && m_targetCamera != m_activeCamera)
                SetAcctiveCamera();
        }


        private void SetCameraTargetFromPlayer()
        {
            if(m_targetCamera != -1)
                m_camerasInLevel[m_targetCamera].ActivateCamera(m_cameraTagets[m_targetCamera].m_target, m_cameraTagets[m_targetCamera].m_lookAt);
            m_activeCamera = m_targetCamera == -1 ? 0 : m_targetCamera;
        }

        private void SetAcctiveCamera(int cameraId = -1) {
            if (cameraId == -1)
                cameraId = m_targetCamera;
            m_camerasInLevel[m_activeCamera ?? 0].ActivateCamera(m_cameraTagets[m_activeCamera ?? 0].m_target, m_cameraTagets[m_activeCamera ?? 0].m_lookAt);
            m_camerasInLevel[cameraId].ActivateCamera(m_cameraTagets[cameraId].m_target, m_cameraTagets[cameraId].m_lookAt);
        }

    }
}
