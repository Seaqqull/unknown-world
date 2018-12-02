using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnknownWorld.Weapon.Ammo
{
    public class AmmoController : MonoBehaviour
    {
        [SerializeField] [Range(1, ushort.MaxValue)] private ushort m_magazineCapacity = 1;
        [SerializeField] [Range(1, ushort.MaxValue)] private float m_reloadScale = 1.0f;
        [SerializeField] [Range(1, ushort.MaxValue)] private ushort m_available = 0;
        [SerializeField] [Range(1, ushort.MaxValue)] private ushort m_capacity = 0;
        [SerializeField] private string m_buttonReload = "Reload";
        [SerializeField] private bool m_isDirectCallOnly = true;        
        [SerializeField] private bool m_isAmmoUnlimited = false;
        [SerializeField] private GameObject m_bullet;

        private UnknownWorld.Weapon.Data.WeaponCharacteristic m_weaponSpecification;
        private UnknownWorld.Weapon.Data.IWeaponAction m_weaponHandler;
        private UnknownWorld.Weapon.Data.AmmoState m_state;
        private ushort m_magazineCount = 0;
        private ushort m_magazineAmmo = 0;
        private float m_reloadSpeed;
        private bool m_isActive;

        public UnknownWorld.Weapon.Data.AmmoState State
        {
            get { return this.m_state; }
        }
        public GameObject BulletObject
        {
            get { return this.m_bullet; }
        }
        public bool IsDirectCallOnly
        {
            get { return this.m_isDirectCallOnly; }
            set { this.m_isDirectCallOnly = value; }
        }
        public bool IsAmmoUnlimited
        {
            get { return this.m_isAmmoUnlimited; }
            set { this.m_isAmmoUnlimited = value; }
        }
        public float ReloadScale
        {
            get { return this.m_reloadScale; }
            set { this.m_reloadScale = value; }
        }
        public ushort Available
        {
            get { return this.m_available; }
            set
            {
                if (value > this.m_capacity)
                    this.m_available = this.m_capacity;
                else
                    this.m_available = value;
            }
        }        
        public ushort Capacity
        {
            get { return this.m_capacity; }
            set
            {
                if ((value < this.m_capacity) &&
                    (this.m_available > value))
                {
                    this.m_available = value;
                }
                this.m_capacity = value;
            }
        }
        public uint BulletId
        {
            get
            {
                return this.m_bullet.
                    GetComponent<UnknownWorld.Weapon.Ammo.Bullet>().Id;
            }
        }
        public Bullet Bullet
        {
            get { return this.m_bullet.GetComponent<Bullet>(); }
        }


        private void Start()
        {
            Reload();
        }

        private void Update()
        {
            if ((!m_isActive) ||
                (m_isDirectCallOnly))return;

            if ((Input.GetButtonDown(m_buttonReload)) &&
                (m_state != Data.AmmoState.NoAmmo) &&
                (m_state != Data.AmmoState.ReloadWaiting) &&
                (m_state != Data.AmmoState.ShotNotAllowed) &&
                (m_state != Data.AmmoState.ReloadPerforming))
            {
                m_state = Data.AmmoState.ReloadWaiting;
                m_weaponHandler.OnReloadProbability();
            }
        }

        
        private void Reload()
        {
            m_magazineCount = (ushort)(m_available / m_magazineCapacity);
            m_magazineAmmo = (ushort)(m_available % m_magazineCapacity);

            if ((m_magazineAmmo != m_magazineCapacity) &&
                (m_magazineCount > 0))
            {
                m_magazineAmmo = m_magazineCapacity;
                m_magazineCount--;
            }
            m_state = StateDetermination();
        }

        private void Shoot(ushort shootCount)
        {
            if (m_isAmmoUnlimited) return;

            m_magazineAmmo -= shootCount;
            m_available -= shootCount;
        }


        public bool OnReload()
        {
            if ((m_available > 0) &&
                (m_state == Data.AmmoState.ReloadWaiting))
            {
                m_state = Data.AmmoState.ReloadPerforming;
                Invoke("Reload", m_weaponSpecification.ReloadSpeed * m_reloadScale);
                return true;
            }
            return false;
        }

        public bool DoReload()
        {
            if ((m_available > 0) &&
                ((m_state != Data.AmmoState.ReloadWaiting) &&
                 (m_state != Data.AmmoState.ReloadPerforming)))
            {
                m_state = Data.AmmoState.ReloadPerforming;
                Invoke("Reload", m_weaponSpecification.ReloadSpeed * m_reloadScale);
                return true;
            }
            return false;
        }

        public void DeActivate()
        {
            m_state = StateDetermination();
            m_weaponSpecification = null;
            m_weaponHandler = null;
            m_isActive = false;            
        }

        public void CancelReload()
        {
            m_state = StateDetermination();
        }

        public void AddAmmo(ushort count)
        {
            if (count > m_capacity - m_available)
                m_available = m_capacity;
            else
                m_available += count;

            if ((m_state != Data.AmmoState.ReloadWaiting) && 
                (m_state != Data.AmmoState.ReloadPerforming))
                m_state = StateDetermination();
        }

        public UnknownWorld.Weapon.Data.AmmoState DoShot(ushort bulletsToPerformShot = 1)
        {
            if ((m_state == Data.AmmoState.ReloadWaiting) || 
                (m_state == Data.AmmoState.ReloadPerforming)) return m_state;

            m_state = StateDetermination(bulletsToPerformShot);
            
            if (m_state != Data.AmmoState.Shootable) return m_state;

            Shoot(bulletsToPerformShot);

            return Data.AmmoState.Shootable;
        }

        public UnknownWorld.Weapon.Data.AmmoState StateDetermination(ushort bulletsToPerformShot = 1)
        {
            return (!m_isActive) ? UnknownWorld.Weapon.Data.AmmoState.ShotNotAllowed :
                   (m_isAmmoUnlimited) ? UnknownWorld.Weapon.Data.AmmoState.Shootable :
                   (bulletsToPerformShot > m_available) ? UnknownWorld.Weapon.Data.AmmoState.NoAmmo :
                   (bulletsToPerformShot > m_magazineAmmo) ? UnknownWorld.Weapon.Data.AmmoState.EmptyMagazine :
                   UnknownWorld.Weapon.Data.AmmoState.Shootable;
        }

        public void Activate(UnknownWorld.Weapon.Data.IWeaponAction methods, UnknownWorld.Weapon.Data.WeaponCharacteristic weapon)
        {
            m_weaponSpecification = weapon;
            m_weaponHandler = methods;
            m_isActive = true;
        }
    }
}
