using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnknownWorld.Weapon.Data
{
    [System.Serializable]
    public enum WeaponModeType
    {
        Unknown,
        Single,
        Burst,
        Automatic,
        Stream
    }

    [System.Serializable]
    public enum WeaponState
    {
        Unknown,
        Inactive, // when weapon not selected
        Inaction, // when weapon 
        Shoot, // when shooting
        Reaload, // when weapon reloading
        Dropped // when weapon dropped
    }

    [System.Serializable]
    public enum WeaponType
    {
        Unknown,
        Arm,
        Pistol,
        SemiAutomaticRifle,
        AutomaticRifle,
        Sniper
    }

    [System.Serializable]
    public enum ShotState
    {
        Unknown,
        ReadyToShot, // when can perform shot
        ShotWaiting, // when click shot
        ShotPerforming, // when perform shot
        ShotDelay // when delay between shots
    }

    [System.Serializable]
    public enum AmmoState
    {
        Unknown,
        ReloadWaiting,
        ReloadPerforming,
        Shootable,
        EmptyMagazine,
        NoAmmo,
        ShotNotAllowed
    }

    
    public interface IWeaponAction
    {
        bool DoShot();

        bool DoReload();

        void OnShotProbability();

        void OnReloadProbability();

        bool ChangeAmmoType(bool modeDestination);

        bool ChangeShootingMode(bool modeDestination);
    }


    [System.Serializable]
    public class WeaponCharacteristic
    {
        [SerializeField] [Range(0, ushort.MaxValue)] private float m_damage = 0.0f;
        [SerializeField] [Range(0, ushort.MaxValue)] private float m_range = 0.0f;
        [SerializeField] [Range(0, ushort.MaxValue)] private float m_reloadSpeed = 5.0f;
        [SerializeField] [Range(0, ushort.MaxValue)] private float m_shootSpeed = 60.0f;
        [SerializeField] [Range(0, ushort.MaxValue)] private float m_bulletSpeed = 1.0f;
        [SerializeField] [Range(0, ushort.MaxValue)] private float m_burstSpeed = 0.0f;
        [SerializeField] [Range(0, ushort.MaxValue)] private float m_shotDelay = 0.0f;        

        public float BulletSpeed
        {
            get { return this.m_bulletSpeed; }
            set { this.m_bulletSpeed = value; }
        }
        public float ReloadSpeed
        {
            get { return this.m_reloadSpeed; }
            set { this.m_reloadSpeed = value; }
        }
        public float ShootSpeed
        {
            get { return this.m_shootSpeed; }
            set { this.m_shootSpeed = value; }
        }
        public float BurstSpeed
        {
            get { return this.m_burstSpeed; }
            set { this.m_burstSpeed = value; }
        }
        public float ShotDelay
        {
            get { return this.m_shotDelay; }
            set { this.m_shotDelay = value; }
        }
        public float Damage
        {
            get { return this.m_damage; }
            set { this.m_damage = value; }
        }
        public float Range
        {
            get { return this.m_range; }
            set { this.m_range = value; }
        }
    }

    [System.Serializable]
    public class WeaponData
    {
        [SerializeField] private Transform m_bulletParent;
        [SerializeField] private Transform m_bulletStart;
        
        private UnknownWorld.Weapon.Ammo.Bullet m_bullet;

        public UnknownWorld.Weapon.Ammo.Bullet Bullet
        {
            get { return this.m_bullet; }
            set { this.m_bullet = value; }
        }
        public Transform BulletStartPosition
        {
            get { return this.m_bulletStart; }
            set { this.m_bulletStart = value; }
        }
        public Transform BulletParent
        {
            get { return this.m_bulletParent; }
            set { this.m_bulletParent = value; }
        }
    }


    public static class WeaponHelper
    {
        public static UnknownWorld.Manager.GameManager GameManager;
        public static GameObject SimpleBulletPrefab;
        public static GameObject WeaponContainer;
        public static GameObject BulletContainer;


        public static bool isMouseHitPosition(Vector3 mousePosition, float maximumShootRange, out Vector3 hitPosition)
        {                        
            RaycastHit hit;

            
            Ray rayMouse = Camera.main.ScreenPointToRay(mousePosition);//GameManager.GetActiveCamera().ScreenPointToRay(mousePosition);
            if (Physics.Raycast(rayMouse.origin, rayMouse.direction, out hit, maximumShootRange))
            {
                hitPosition = hit.point;
                return true;
            }
            hitPosition = Vector3.zero;
            return false;
        }

        public static bool isHitPosition(Vector3 objPosition, Vector3 targetPosition, float maximumShootRange, out Vector3 hitPosition)
        {
            RaycastHit hit;
            
            if (Physics.Raycast(objPosition, (objPosition - targetPosition).normalized, out hit, maximumShootRange))
            {
                hitPosition = hit.point;
                return true;
            }
            hitPosition = Vector3.zero;
            return false;
        }

        public static Quaternion GetTargetDirection(Vector3 objPosition, Vector3 targetPosition, bool ignoreX = false, bool ignoreY = false, bool ignoreZ = false)
        {
            if (ignoreX)
                targetPosition.x = objPosition.x;
            if (ignoreY)
                targetPosition.y = objPosition.y;
            if (ignoreZ)
                targetPosition.z = objPosition.z;

            return Quaternion.LookRotation(objPosition - targetPosition);
        }

        public static bool GetMouseHitDirection(Vector3 objPosition, Vector3 mousePosition, float maximumShootRange, out Quaternion hitDirection, bool ignoreX = false, bool ignoreY = false, bool ignoreZ = false)
        {
            Vector3 hitPosition;

            if (isMouseHitPosition(mousePosition, maximumShootRange, out hitPosition))
            {
                hitDirection = GetTargetDirection(objPosition, hitPosition, ignoreX, ignoreY, ignoreZ);
                return true;
            }
            hitDirection = Quaternion.identity;
            return false;
        }

    }
}
