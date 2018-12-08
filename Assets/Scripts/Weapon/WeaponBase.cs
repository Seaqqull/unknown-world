using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnknownWorld.Weapon.Ammo;
using UnknownWorld.Weapon.Shooting;

namespace UnknownWorld.Weapon
{
    public abstract class WeaponBase : MonoBehaviour, UnknownWorld.Weapon.Data.IWeaponAction
    {
        [SerializeField] protected Data.WeaponType m_type = Data.WeaponType.Unknown;
        [SerializeField] protected Data.WeaponState m_state = Data.WeaponState.Inactive;

        [SerializeField] protected string m_buttonShootingMode = "ShotMode";
        [SerializeField] protected string m_buttonAmmoType = "AmmoType";

        [SerializeField] protected bool m_isDirectCallOnly = true;
        [SerializeField] protected bool m_isReloadInstant;

        [SerializeField] protected LayerMask m_targetMask;

        [SerializeField] protected UnknownWorld.Weapon.Data.WeaponData m_data;
        [SerializeField] protected UnknownWorld.Weapon.Data.WeaponCharacteristic m_characteristic;

        [SerializeField] protected int m_shootingModeIndex;
        [SerializeField] protected List<Shooting.ShotController> m_shootingModes;

        [SerializeField] protected int m_ammoTypeIndex;
        [SerializeField] protected List<Ammo.AmmoController> m_ammoTypes;        

        public List<ShotController> ShootingModes
        {
            get
            {
                return this.m_shootingModes ??
                    (this.m_shootingModes = new List<ShotController>());
            }
            set { this.m_shootingModes = value; }
        }
        public List<AmmoController> AmmoTypes
        {
            get
            {
                return this.m_ammoTypes ??
                    (this.m_ammoTypes = new List<AmmoController>());
            }
            set { this.m_ammoTypes = value; }
        }
        public Transform BulletParent
        {
            get { return this.m_data.BulletParent; }
            set { this.m_data.BulletParent = value; }
        }        
        public Data.WeaponState State
        {
            get { return this.m_state; }
            set
            {
                if (value == this.m_state) return;
                
                if (DoStateChange(value))
                    this.m_state = value;
            }
        }
        public bool IsDirectCallOnly
        {
            get { return this.m_isDirectCallOnly; }
            set { this.m_isDirectCallOnly = value; }
        }
        public Data.WeaponType Type
        {
            get { return this.m_type; }
        }
        public bool IsReloadInstant
        {
            get { return this.m_isReloadInstant; }
            set { this.m_isReloadInstant = value; }
        }
        public LayerMask TargetMask
        {
            get { return this.m_targetMask; }
            set { this.m_targetMask = value; }
        }
        public float ReloadSpeed
        {
            get { return this.m_characteristic.ReloadSpeed; }
            set { this.m_characteristic.ReloadSpeed = value; }
        }
        public float ShootSpeed
        {
            get { return this.m_characteristic.ShootSpeed; }
            set { this.m_characteristic.ShootSpeed = value; }
        }
        public float BurstSpeed
        {
            get { return this.m_characteristic.BurstSpeed; }
            set { this.m_characteristic.BurstSpeed = value; }
        }
        public float ShotTime
        {
            get { return 60.0f / m_characteristic.ShootSpeed; }
        }
        public float Damage
        {
            get { return this.m_characteristic.Damage; }
            set { this.m_characteristic.Damage = value; }
        }
        public float Range
        {
            get { return this.m_characteristic.Range; }
            set { this.m_characteristic.Range = value; }
        }


        protected virtual void Start()
        {

        }

        protected virtual void Update()
        {
            if ((m_isDirectCallOnly) ||
                (m_state == Data.WeaponState.Dropped) ||
                (m_state == Data.WeaponState.Inactive)) return;

            // Change current ammo
            if (Input.GetButtonDown(m_buttonAmmoType))
            {
                ChangeAmmoType(true);
            }

            // Change current shooting mode
            if (Input.GetButtonDown(m_buttonShootingMode))
            {
                ChangeShootingMode(true);
            }
        }


        protected virtual void Shot()
        {
            UnknownWorld.Weapon.Ammo.Bullet bullet = Instantiate(m_ammoTypes[m_ammoTypeIndex].BulletObject, (m_data.BulletParent) ? m_data.BulletParent : transform)
                    .GetComponent<UnknownWorld.Weapon.Ammo.Bullet>();

            bullet.transform.position = m_data.BulletStartPosition.position;

            bullet.SetBulletParams(Damage, m_characteristic.BulletSpeed, m_characteristic.Range, m_targetMask);
           
            bullet.Launch();
        }

        protected virtual void Drop()
        {
            DeActivate();

            m_state = Data.WeaponState.Dropped;
            transform.parent = null;
            transform.parent = Data.WeaponHelper.WeaponContainer.transform;
        }
                
        protected virtual bool DoStateChange(Data.WeaponState newState)
        {
            switch (newState)
            {
                case Data.WeaponState.Inactive:
                    DeActivate();
                    return true;
                case Data.WeaponState.Inaction:
                    Activate();
                    return true;
                case Data.WeaponState.Shoot:
                    return DoShot();
                case Data.WeaponState.Reaload:
                    return DoReload();
                case Data.WeaponState.Dropped:
                    Drop();
                    return true;
                default:
                    return false;
            }
        }


        public virtual void Activate()
        {
            m_state = Data.WeaponState.Inaction;

            m_ammoTypes[m_ammoTypeIndex].Activate(this, m_characteristic);
            m_data.Bullet = m_ammoTypes[m_ammoTypeIndex].Bullet;

            m_shootingModes[m_shootingModeIndex].Activate(this, m_characteristic, m_data);
        }

        public virtual void DeActivate()
        {
            m_state = Data.WeaponState.Inactive;

            m_shootingModes[m_shootingModeIndex].DeActivate();
            m_ammoTypes[m_ammoTypeIndex].DeActivate();
            m_data.Bullet = null;
        }


        public bool DoShot()
        {
            if (m_ammoTypes[m_ammoTypeIndex].DoShot(m_shootingModes[m_shootingModeIndex].BulletsToPerformShot)
                    == Data.AmmoState.Shootable)
            {
                if (!m_shootingModes[m_shootingModeIndex].DoShot(ShotTime, false))
                {
                    m_shootingModes[m_shootingModeIndex].CancelShot();
                    return false;
                }
                // shot bullet
                Invoke("Shot", m_characteristic.ShotDelay);
                return true;
            }

            m_shootingModes[m_shootingModeIndex].CancelShot();
            return false;
        }

        public bool DoReload()
        {
            if (m_shootingModes[m_shootingModeIndex].State == Data.ShotState.ReadyToShot)
            {
                if ((m_ammoTypes[m_ammoTypeIndex].DoReload()) &&
                    (m_ammoTypes[m_ammoTypeIndex].State == Data.AmmoState.ReloadPerforming))
                {
                    // make reloading animation
                    return true;
                }
                m_ammoTypes[m_ammoTypeIndex].CancelReload();
                return false;
            }
            m_ammoTypes[m_ammoTypeIndex].CancelReload();
            return false;
        }

        public void OnShotProbability()
        {
            if (m_ammoTypes[m_ammoTypeIndex].DoShot(m_shootingModes[m_shootingModeIndex].BulletsToPerformShot)
                    == Data.AmmoState.Shootable)
            {
                if (!m_shootingModes[m_shootingModeIndex].OnShot(ShotTime))
                {
                    m_shootingModes[m_shootingModeIndex].CancelShot();
                    return;
                }
                // shot bullet
                Invoke("Shot", m_characteristic.ShotDelay);
            }
            else
                m_shootingModes[m_shootingModeIndex].CancelShot();
        }

        public void OnReloadProbability()
        {
            if (m_shootingModes[m_shootingModeIndex].State == Data.ShotState.ReadyToShot)
            {
                if ((m_ammoTypes[m_ammoTypeIndex].OnReload()) &&
                    (m_ammoTypes[m_ammoTypeIndex].State == Data.AmmoState.ReloadPerforming))
                {
                    // make reloading animation
                }
                else
                    m_ammoTypes[m_ammoTypeIndex].CancelReload();
            }
            else
                m_ammoTypes[m_ammoTypeIndex].CancelReload();
        }
        
        public bool ChangeAmmoType(bool modeDestination)
        {
            if ((m_ammoTypes.Count <= 1) ||
                (m_ammoTypes[m_ammoTypeIndex].State == Data.AmmoState.ReloadWaiting) ||
                (m_ammoTypes[m_ammoTypeIndex].State == Data.AmmoState.ReloadPerforming) ||
                (m_shootingModes[m_shootingModeIndex].State != Data.ShotState.ReadyToShot)) return false;

            int newAmmoIndex = (modeDestination) ?
                m_ammoTypeIndex + 1 : m_ammoTypeIndex - 1;

            if ((newAmmoIndex < 0) ||
                (newAmmoIndex >= m_ammoTypes.Count))
            {
                newAmmoIndex = (modeDestination) ? 0 : m_ammoTypes.Count - 1;
            }

            m_ammoTypes[m_ammoTypeIndex].DeActivate();
            m_ammoTypes[newAmmoIndex].Activate(this, m_characteristic);

            m_ammoTypeIndex = newAmmoIndex;
            m_data.Bullet = m_ammoTypes[m_ammoTypeIndex].Bullet;

            return true;
            // Ammo type changing animation & some interface actions
        }

        public bool ChangeShootingMode(bool modeDestination)
        {
            if ((m_shootingModes.Count <= 1) ||
                (m_ammoTypes[m_ammoTypeIndex].State == Data.AmmoState.ReloadWaiting) ||
                (m_ammoTypes[m_ammoTypeIndex].State == Data.AmmoState.ReloadPerforming) ||
                (m_shootingModes[m_shootingModeIndex].State != Data.ShotState.ReadyToShot)) return false;

            int newModeIndex = (modeDestination) ?
                m_shootingModeIndex + 1 : m_shootingModeIndex - 1;

            if ((newModeIndex < 0) ||
                (newModeIndex >= m_shootingModes.Count))
            {
                newModeIndex = (modeDestination) ? 0 : m_shootingModes.Count - 1;
            }

            m_shootingModes[m_shootingModeIndex].DeActivate();
            m_shootingModes[newModeIndex].Activate(this, m_characteristic, m_data);
            m_shootingModeIndex = newModeIndex;

            return true;
            // Shot mode changing animation & some interface actions
        }                

    }
}
