using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnknownWorld.Weapon.Data;

namespace UnknownWorld.Weapon.Shooting
{
    public abstract class ShotController : MonoBehaviour
    {
        [SerializeField] protected UnknownWorld.Weapon.Data.WeaponModeType m_type = WeaponModeType.Unknown;
        [SerializeField] protected ushort m_bulletsToPerformShot = 1;
        [SerializeField] protected bool m_isDirectCallOnly = true;

        protected UnknownWorld.Weapon.Data.ShotState m_state = Data.ShotState.ReadyToShot;
        protected UnknownWorld.Weapon.Data.WeaponCharacteristic m_weaponSpecification;
        protected UnknownWorld.Weapon.Data.IWeaponAction m_weaponHandler;
        protected UnknownWorld.Weapon.Data.WeaponData m_weaponData;        
        protected Quaternion m_targetDirection;        
        protected float m_waitTimeSinceShot;        
        protected float m_timeSinceShot;        
        protected bool m_isTargetSetted;        
        protected bool m_isActive;
        protected bool m_isShot;

        public ushort BulletsToPerformShot
        {
            get { return this.m_bulletsToPerformShot; }            
        }
        public Quaternion TargetDirection
        {
            get { return this.m_targetDirection; }
        }
        public bool IsDirectCallOnly
        {
            get { return this.m_isDirectCallOnly; }
            set { this.m_isDirectCallOnly = value; }
        }
        public WeaponModeType Type
        {
            get { return this.m_type; }
        }
        public ShotState State
        {
            get { return this.m_state; }
        }


        protected virtual void Update()
        {            
            if ((!m_isActive) &&
                (m_state != Data.ShotState.ShotDelay)) return;

            if (m_state == Data.ShotState.ShotDelay)
            {
                m_timeSinceShot += Time.deltaTime;
                if (m_timeSinceShot >= m_waitTimeSinceShot)
                {
                    m_state = Data.ShotState.ReadyToShot;
                    m_isTargetSetted = false;
                }
            }
        }


        // when no ammo
        public void CancelShot()
        {
            m_state = ShotState.ReadyToShot;
            m_isTargetSetted = false;
        }

        // when select current shot controller
        public virtual void Activate(UnknownWorld.Weapon.Data.IWeaponAction methods, UnknownWorld.Weapon.Data.WeaponCharacteristic specification, UnknownWorld.Weapon.Data.WeaponData data)
        {
            m_weaponSpecification = specification;
            m_weaponHandler = methods;            
            m_weaponData = data;
            m_isActive = true;
        }

        // when select another shot controller
        public virtual void DeActivate()
        {
            m_weaponSpecification = null;
            m_weaponHandler = null;
            m_weaponData = null;
            m_isActive = false;            
        }

        public virtual bool OnShot(float shotInactionDelay, bool isTargetNeed = true)
        {
            if ((isTargetNeed) &&
                ((!m_isTargetSetted) || 
                 (m_state != ShotState.ShotWaiting))) return false;

            if ((!isTargetNeed) &&
                (m_state != ShotState.ShotWaiting)) return false;

            m_state = ShotState.ShotPerforming;

            m_waitTimeSinceShot = shotInactionDelay;
            m_timeSinceShot = 0.0f;

            m_state = ShotState.ShotDelay;

            return true;
        }

        public virtual bool DoShot(float shotInactionDelay, bool isTargetNeed = true)
        {
            if ((isTargetNeed) &&
                (!m_isTargetSetted)) return false;

            m_state = ShotState.ShotPerforming;

            m_waitTimeSinceShot = shotInactionDelay;
            m_timeSinceShot = 0.0f;

            m_state = ShotState.ShotDelay;

            return true;
        }

        public virtual bool SetShotTarget(Vector3 target, bool isCameraCoordinates)
        {
            m_isTargetSetted = false;
            if (isCameraCoordinates)
            {
                m_isTargetSetted = UnknownWorld.Weapon.Data.WeaponHelper.
                    GetMouseHitDirection(m_weaponData.BulletStartPosition.position, target, m_weaponSpecification.Range * m_weaponData.Bullet.RangeScale, out m_targetDirection, ignoreY: false);
                return m_isTargetSetted;
            }

            m_isTargetSetted = true;
            m_targetDirection = UnknownWorld.Weapon.Data.WeaponHelper.
                GetTargetDirection(m_weaponData.BulletStartPosition.position, target, ignoreY: true);
            return true;
        }
    }
}
