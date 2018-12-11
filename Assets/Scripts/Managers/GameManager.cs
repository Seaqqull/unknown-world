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

        public int m_targetCamera = -1;
        public CameraKeeperData[] m_cameraTagets; // at [0] always must be the player and default controller                        


        // Use this for initialization
        private void Awake()
        {
            SetCameraTargetFromPlayer();

            // ignore colision between character and target
            Physics.IgnoreLayerCollision(11, 10);
            Physics.IgnoreLayerCollision(11, 14);
            Physics.IgnoreLayerCollision(12, 15);

            // initialize path data
            SetPathStandartData();

            // initialize weapon data
            SetWeaponStandartData();
        }

        // Update is called once per frame
        private void Update()
        {
            if (m_activeCamera.HasValue && m_targetCamera != m_activeCamera)
                SetActiveCamera();
        }


        private void SetPathStandartData()
        {
            if (!UnknownWorld.Path.Data.PathHelper.PathPrefab)
                UnknownWorld.Path.Data.PathHelper.PathPrefab = Resources.Load("Path/IntermediatePoint") as GameObject;
            if (!UnknownWorld.Path.Data.PathHelper.PathSpawner)
                UnknownWorld.Path.Data.PathHelper.PathSpawner = GameObject.Find("Path Targets");

            UnknownWorld.Path.Data.PathHelper.PathWaitTime = 1.0f;
        }

        private void SetWeaponStandartData()
        {
            if (!UnknownWorld.Weapon.Data.WeaponHelper.SimpleBulletPrefab)
                UnknownWorld.Weapon.Data.WeaponHelper.SimpleBulletPrefab = Resources.Load("Weapon/SimpleBullet") as GameObject;
            if (!UnknownWorld.Weapon.Data.WeaponHelper.GameManager)
                UnknownWorld.Weapon.Data.WeaponHelper.GameManager = this;
            if (!UnknownWorld.Weapon.Data.WeaponHelper.WeaponContainer)
                UnknownWorld.Weapon.Data.WeaponHelper.WeaponContainer = GameObject.Find("--- Weapons ---");
            if (!UnknownWorld.Weapon.Data.WeaponHelper.BulletContainer)
                UnknownWorld.Weapon.Data.WeaponHelper.BulletContainer = GameObject.Find("--- Bullets ---");            
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

        public Camera GetActiveCamera()
        {
            return m_cameraTagets[m_activeCamera.Value].m_camera.Camera;
        }
    }
}
