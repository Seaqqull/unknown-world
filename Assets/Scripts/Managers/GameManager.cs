using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnknownWorld.Manager
{
    public class GameManager : MonoBehaviour
    {
        [System.Serializable]
        public class CameraKeeperData
        {
            public CameraController m_camera;
            public Transform m_target; // Cant be deleted, get from m_camera
            public Transform m_lookAt; // Cant be deleted, get from m_camera/            
        }


        private int? m_activeCamera = null;

        public CameraKeeperData[] m_cameraTagets; // at [0] always must be the player and default controller                
        public int m_targetCamera = -1;


        // Use this for initialization
        private void Start()
        {
            SetCameraTargetFromPlayer();

            // ignore colision between character and target
            Physics.IgnoreLayerCollision(11, 10);
        }

        // Update is called once per frame
        private void Update()
        {
            if (m_activeCamera.HasValue && m_targetCamera != m_activeCamera)
                SetActiveCamera();
        }


        private void SetCameraTargetFromPlayer()
        {
            if(m_targetCamera != -1)
                m_cameraTagets[m_targetCamera].m_camera.ActivateCamera(m_cameraTagets[m_targetCamera].m_target, m_cameraTagets[m_targetCamera].m_lookAt);
            m_targetCamera = 0;
        }

        private void SetActiveCamera(int cameraId = -1) {            
            m_cameraTagets[m_activeCamera.Value].m_camera.DiactivateCamera();
            if (cameraId != -1)
                m_activeCamera = m_targetCamera;

            m_cameraTagets[m_activeCamera.Value].m_camera.ActivateCamera(
                m_cameraTagets[m_activeCamera.Value].m_target, m_cameraTagets[m_activeCamera.Value].m_lookAt
            );
            
        }

    }
}
