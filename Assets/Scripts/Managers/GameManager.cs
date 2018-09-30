using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnknownWorld
{
    public class GameManager : MonoBehaviour
    {
        [System.Serializable]
        public class CameraKeeperData
        {
            public Transform m_target;/*Cant be deleted, get from m_camera*/
            public Transform m_lookAt;/*Cant be deleted, get from m_camera*/
            public CameraController m_camera;
        }

        public CameraKeeperData[] m_cameraTagets; // at [0] always must be the player and default controller
        
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
                m_cameraTagets[m_targetCamera].m_camera.ActivateCamera(m_cameraTagets[m_targetCamera].m_target, m_cameraTagets[m_targetCamera].m_lookAt);
            m_targetCamera = 0;
        }

        private void SetAcctiveCamera(int cameraId = -1) {            
            m_cameraTagets[m_activeCamera.Value].m_camera.DiactivateCamera();
            if (cameraId != -1)
                m_activeCamera = m_targetCamera;

            m_cameraTagets[m_activeCamera.Value].m_camera.ActivateCamera(
                m_cameraTagets[m_activeCamera.Value].m_target, m_cameraTagets[m_activeCamera.Value].m_lookAt
            );
            
        }

    }
}
