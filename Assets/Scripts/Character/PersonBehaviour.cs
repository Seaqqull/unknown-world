using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnknownWorld.Behaviour
{
    [System.Serializable]
    public abstract class PersonBehaviour : MonoBehaviour
    {
        [System.Serializable]
        public class PersonCharacteristics
        {            
            [SerializeField] private bool m_isPersonActive = true;
            [SerializeField] private float m_movementSpeed = 1.0f;
            [SerializeField] private float m_rotationSpeed = 1.0f;

            [SerializeField] private float m_staminaMultiplier = 1.0f;
            [SerializeField] private float m_healthMultiplier = 1.0f;

            [SerializeField] [Range(0, 100)] private float m_percentToExhaustion = 1.0f;
            [SerializeField] [Range(0, 100)] private float m_percentToLowHealth = 10.0f;

            [SerializeField] private bool m_isStaminaLock = false;
            [SerializeField] private bool m_isStaminaRegen = true;
            [SerializeField] private float m_stamina = 100.0f;
            [SerializeField] private float m_staminaMin = 0.0f;
            [SerializeField] private float m_staminaMax = 100.0f;
            [SerializeField] private float m_staminaRegen = 1.0f;
            [SerializeField] private float m_recoveryAfterExhaustion = 1.0f;


            [SerializeField] private bool m_isHealthLock = false;
            [SerializeField] private bool m_isHealthRegen = true;
            [SerializeField] private float m_health = 100.0f;
            [SerializeField] private float m_healthMin = 0.0f;
            [SerializeField] private float m_healthMax = 100.0f;
            [SerializeField] private float m_healthRegen = 1.0f;
            

            private float m_timeFromExhaustion;
            private bool m_isExhausted = false;
            private bool m_isHealthLow = false;
            private float m_recoveryAfterDead;

            public float RecoveryAfterExhaustion
            {
                get { return this.m_recoveryAfterExhaustion; }
                set { this.m_recoveryAfterExhaustion = value; }
            }
            public float PercentToExhaustion
            {
                get { return m_percentToExhaustion; }
                set { m_percentToExhaustion = value; }
            }
            public float PercentToLowHealth
            {
                get { return this.m_percentToLowHealth; }
                set { this.m_percentToLowHealth = value; }
            }
            public float TimeFromExhaustion
            {
                get { return this.m_timeFromExhaustion; }
                set { this.m_timeFromExhaustion = value; }
            }
            public float StaminaMultiplier
            {
                get { return this.m_staminaMultiplier; }
                set { this.m_staminaMultiplier = value; }
            }
            public float HealthMultiplier
            {
                get { return this.m_healthMultiplier; }
                set { this.m_healthMultiplier = value; }
            }
            public bool IsPersonActive
            {
                get { return this.m_isPersonActive; }
                set { this.m_isPersonActive = value; }
            }
            public float MovementSpeed
            {
                get { return this.m_movementSpeed; }
                set { this.m_movementSpeed = value; }
            }
            public float RotationSpeed
            {
                get { return this.m_rotationSpeed; }
                set { this.m_rotationSpeed = value; }
            }
            public bool IsStaminaRegen
            {
                get { return this.m_isStaminaRegen; }
                set { this.m_isStaminaRegen = value; }
            }
            public bool IsHealthRegen
            {
                get { return this.m_isHealthRegen; }
                set { this.m_isHealthRegen = value; }
            }
            public float StaminaRegen
            {
                get { return this.m_staminaRegen; }
                set { this.m_staminaRegen = value; }
            }
            public bool IsStaminaLock
            {
                get { return this.m_isStaminaLock; }
                set { this.m_isStaminaLock = value; }
            }
            public bool IsHealthLock
            {
                get { return this.m_isHealthLock; }
                set { this.m_isHealthLock = value; }
            }
            public float HealthRegen
            {
                get { return this.m_healthRegen; }
                set { this.m_healthRegen = value; }
            }
            public bool IsExhausted
            {
                get { return this.m_isExhausted; }
                set { this.m_isExhausted = value; }
            }
            public bool IsHealthLow
            {
                get { return this.m_isHealthLow; }
                set { this.m_isHealthLow = value; }
            }
            public float StaminaMin
            {
                get { return this.m_staminaMin; }
                set { this.m_staminaMin = value; }
            }
            public float StaminaMax
            {
                get { return this.m_staminaMax; }
                set { this.m_staminaMax = value; }
            }
            public float HealthMin
            {
                get { return this.m_healthMin; }
                set { this.m_healthMin = value; }
            }
            public float HealthMax
            {
                get { return this.m_healthMax; }
                set { this.m_healthMax = value; }
            }
            public float Stamina
            {
                get { return this.m_stamina; }
                set { this.m_stamina = value; }
            }
            public float Health
            {
                get { return this.m_health; }
                set { this.m_health = value; }
            }

        }


        [SerializeField] private Slider m_staminaSlider;
        [SerializeField] private Slider m_healthSlider;

        [SerializeField] private PersonCharacteristics m_data;
        [SerializeField] private UnknownWorld.Path.Data.PathPoint m_point;        
        
        protected UnknownWorld.Area.Target.TracingAreaContainer m_areaContainer;
        protected event Action<float> m_staminaUIUpdater = delegate { };
        protected event Action<float> m_healthUIUpdater = delegate { };
        protected event Action<float> m_exhaustion = delegate { };
        protected event Action m_normalHealth = delegate { };
        protected UnknownWorld.Sound.SoundContainer m_sound;
        protected event Action m_lowHealth = delegate { };        
        private static uint m_idCounter = 0;
        private bool m_isActive;
        private bool m_isDeath;
        private uint m_id;

        public UnknownWorld.Area.Target.TracingAreaContainer AreaContainer
        {
            get
            {
                return this.m_areaContainer;
            }
        }
        public UnknownWorld.Path.Data.PathPoint FollowingPoint
        {
            get { return this.m_point; }
        }
        public event Action<float> StaminaUIUpdate
        {
            add { this.m_staminaUIUpdater += value; }
            remove { this.m_staminaUIUpdater -= value; }
        }
        public event Action<float> HealthUIUpdate
        {
            add { this.m_healthUIUpdater += value; }
            remove { this.m_healthUIUpdater -= value; }
        }
        public PersonCharacteristics Data
        {
            get { return this.m_data; }
        }
        public event Action<float> Exhaustion
        {
            add { this.m_exhaustion += value; }
            remove { this.m_exhaustion -= value; }
        }
        public event Action NormalHealth
        {
            add { this.m_normalHealth += value; }
            remove { this.m_normalHealth -= value; }
        }
        public event Action LowHealth
        {
            add { this.m_lowHealth += value; }
            remove { this.m_lowHealth -= value; }
        }
        public Slider StaminaSlider
        {
            get { return this.m_staminaSlider; }
        }
        public Slider HealthSlider
        {
            get { return this.m_healthSlider; }
        }
        public float RotationSpeed
        {
            get { return this.m_data.RotationSpeed; }
        }
        public float MovementSpeed
        {
            get { return this.m_data.MovementSpeed; }
        }
        public bool IsStaminaLock
        {
            get { return this.m_data.IsStaminaLock; }
            set { this.m_data.IsStaminaLock = value; }
        }
        public bool IsHealthLock
        {
            get { return this.m_data.IsHealthLock; }
            set { this.m_data.IsHealthLock = value; }
        }
        public float StaminaMax
        {
            get { return this.m_data.StaminaMax; }
        }
        public float StaminaMin
        {
            get { return this.m_data.StaminaMin; }
        }
        public float HealthMin
        {
            get { return this.m_data.HealthMin; }
        }
        public float HealthMax
        {
            get { return this.m_data.HealthMax; }
        }
        public bool IsActive
        {
            get { return this.m_isActive; }
            set { SetIsActive(value); }
        }
        public float Stamina
        {
            get { return this.m_data.Stamina; }
        }
        public float Health
        {
            get { return this.m_data.Health; }
        }
        public bool IsDeath
        {
            get { return this.m_isDeath; }
        }
        public uint Id
        {
            get { return this.m_id; }
        }
        

        protected virtual void Awake()
        {
            m_areaContainer = GetComponent<UnknownWorld.Area.Target.TracingAreaContainer>();
            m_sound = GetComponent<UnknownWorld.Sound.SoundContainer>();
            IsActive = m_data.IsPersonActive;
            m_id = m_idCounter++;
            m_isDeath = false;
        }

        protected virtual void Start()
        {
            if (!m_areaContainer) return;

            m_areaContainer.SetHealthLink((damage) => {
                Damage(damage);
            });

            if (!m_sound) return;

            m_areaContainer.SetSoundLink((listener) => {
                return m_sound.GetAudibility(listener);
            });
        }

        protected virtual void Update()
        {
#if UNITY_EDITOR
            IsActive = m_data.IsPersonActive;
#endif
            if ((m_isDeath) || 
                (!m_isActive)) return;

            if (m_data.IsExhausted)
            {
                if (m_data.TimeFromExhaustion >= m_data.RecoveryAfterExhaustion)
                    m_data.IsExhausted = false;
                else
                    m_data.TimeFromExhaustion += Time.deltaTime;
            }

            if ((m_data.IsHealthLow) &&
                (m_data.Health > HealthMin + ((HealthMax - HealthMin) * m_data.PercentToLowHealth * 0.01)))
            {
                m_data.IsHealthLow = false;
                m_normalHealth();
            }

            StaminaRegen();
            HealthRegen();
        }


        private void HealthRegen()
        {
            if ((m_isDeath) ||
                (!m_data.IsHealthRegen) ||
                (m_data.Health >= m_data.HealthMax)) return;

            if ((!m_data.IsHealthLow) &&
                (m_data.Health <= HealthMin + ((HealthMax - HealthMin) * m_data.PercentToLowHealth * 0.01)))
            {
                m_data.IsHealthLow = true;
                m_lowHealth();
            }            

            m_data.Health += m_data.HealthRegen * Time.deltaTime;

            if (m_data.Health > m_data.HealthMax)
                m_data.Health = m_data.HealthMax;
            m_healthUIUpdater(m_data.Health);
        }

        private void StaminaRegen()
        {
            if ((m_isDeath) ||
                (!m_data.IsStaminaRegen) ||
                (m_data.Stamina >= m_data.StaminaMax)) return;

            if ((!m_data.IsExhausted) &&
                (m_data.Stamina <= StaminaMin + ((StaminaMax - StaminaMin) * m_data.PercentToExhaustion * 0.01)))
            {
                m_data.IsExhausted = true;

                m_exhaustion(m_data.RecoveryAfterExhaustion);
                m_data.TimeFromExhaustion = 0;                
            }


            m_data.Stamina += m_data.StaminaRegen * Time.deltaTime;

            if (m_data.Stamina > m_data.StaminaMax)
                m_data.Stamina = m_data.StaminaMax;
            m_staminaUIUpdater(m_data.Stamina);
        }

        protected virtual void Death()
        {
            if (m_data.IsHealthLock) return;

            m_data.Health = m_data.HealthMin;
            m_healthUIUpdater(m_data.Health);

            m_data.IsExhausted = false;

            m_data.IsPersonActive = false;
            IsActive = false;

            m_isDeath = true;
        }

        protected virtual void SetIsActive(bool isActive)
        {
            if (isActive == m_isActive) return;

            m_isActive = isActive;
        }


        public void Damage(float damage)
        {
            if ((m_isDeath) ||
                (m_data.IsHealthLock))
                return;

            m_data.Health -= (damage * m_data.HealthMultiplier);

            if (m_data.Health < m_data.HealthMin) Death();
        }

        public bool DoHealthAction(float healthConsumption)
        {
            if (m_data.IsHealthLock) return true;
            if (m_data.Health - (healthConsumption * m_data.HealthMultiplier) < HealthMin)
                return false;

            m_data.Health -= (healthConsumption * m_data.HealthMultiplier);

            return true;
        }

        public bool IsHealthAction(float healthConsumption)
        {
            if (m_data.IsHealthLock) return true;
            if (m_data.Health - (healthConsumption * m_data.HealthMultiplier) < HealthMin)
                return false;
            return true;
        }

        public bool DoStaminaAction(float staminaCosumption)
        {
            if (m_data.IsStaminaLock) return true;
            if (m_data.Stamina - (staminaCosumption * m_data.StaminaMultiplier) < StaminaMin)
                return false;
            if ((m_data.IsExhausted) &&
                (m_data.TimeFromExhaustion < m_data.RecoveryAfterExhaustion))
                return false;

            m_data.Stamina -= (staminaCosumption * m_data.StaminaMultiplier);
            return true;
        }

        public bool IsStaminaAction(float staminaCosumption)
        {
            if (m_data.IsStaminaLock) return true;
            if (m_data.Stamina - (staminaCosumption * m_data.StaminaMultiplier) < StaminaMin)
                return false;
            if ((m_data.IsExhausted) &&
                (m_data.TimeFromExhaustion < m_data.RecoveryAfterExhaustion))
                return false;

            return true;
        }

    }
}
