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
            [SerializeField] private float m_staminaMultiplier = 1.0f;
            [SerializeField] private float m_healthMultiplier = 1.0f;
            [SerializeField] private bool m_isPersonActive = true;
            [SerializeField] private float m_staminaRegen = 1.0f;
            [SerializeField] private float m_staminaMax = 100.0f;            
            [SerializeField] private float m_healthRegen = 1.0f;
            [SerializeField] private float m_healthMax = 100.0f;
            [SerializeField] private float m_staminaMin = 0.0f;
            [SerializeField] private float m_healthMin = 0.0f;
            [SerializeField] private float m_stamina = 100.0f;
            [SerializeField] private float m_health = 100.0f;
            [SerializeField] private float m_movementSpeed;
            [SerializeField] private float m_rotationSpeed;
            [SerializeField] private bool m_isStaminaLock;
            [SerializeField] private bool m_isHealthLock;

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


        [SerializeField] private PersonCharacteristics m_data;
        [SerializeField] private Slider m_staminaSlider;
        [SerializeField] private Slider m_healthSlider;        
        
        protected UnknownWorld.Area.Target.TracingAreaContainer m_areaContainer;
        private Action<float> m_staminaUIUpdater = delegate { };
        private Action<float> m_healthUIUpdater = delegate { };
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
        public Action<float> StaminaUIUpdate
        {
            get { return this.m_staminaUIUpdater; }
            set { this.m_staminaUIUpdater = value; }
        }
        public Action<float> HealthUIUpdate
        {
            get { return this.m_healthUIUpdater; }
            set { this.m_healthUIUpdater = value; }
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
            IsActive = m_data.IsPersonActive;
            m_id = m_idCounter++;
            m_isDeath = false;
        }

        protected virtual void Update()
        {
            //if (m_isActive) return; // not editor

            StaminaRegen();
            HealthRegen();

            IsActive = m_data.IsPersonActive; // only for editor
        }

        private void HealthRegen()
        {
            if ((m_isDeath) ||
                (m_data.Health >= m_data.HealthMax)) return;

            m_data.Health += m_data.HealthRegen * Time.deltaTime;

            if (m_data.Health > m_data.HealthMax)
                m_data.Health = m_data.HealthMax;
            m_healthUIUpdater(m_data.Health);
        }

        private void StaminaRegen()
        {
            if ((m_isDeath) ||
                (m_data.Stamina >= m_data.StaminaMax)) return;

            m_data.Stamina += m_data.StaminaRegen * Time.deltaTime;

            if (m_data.Stamina > m_data.StaminaMax)
                m_data.Stamina = m_data.StaminaMax;
            m_staminaUIUpdater(m_data.Stamina);
        }

        protected virtual void SetIsActive(bool isActive)
        {
            if (isActive == m_isActive) return;

            m_isActive = isActive;
        }        

        public bool DoHealthAction(float healthConsumption)
        {
            if (m_data.IsHealthLock) return true;
            if (m_data.Health - (healthConsumption * m_data.HealthMultiplier) < HealthMin)
                return false;

            m_data.Health -= (healthConsumption * m_data.HealthMultiplier);
            return true;
        }

        public bool DoStaminaAction(float staminaCosumption)
        {
            if (m_data.IsStaminaLock) return true;
            if (m_data.Stamina - (staminaCosumption * m_data.StaminaMultiplier) < StaminaMin)
                return false;

            m_data.Stamina -= (staminaCosumption * m_data.StaminaMultiplier);
            return true;
        }

        protected virtual void Death()
        {
            if (m_data.IsHealthLock) return;

            m_isActive = false;
        }

    }
}
