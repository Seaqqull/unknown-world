using System;
using UnityEngine;

namespace UnknownWorld.Behaviour
{
    [Serializable]
    public class CharacterBehaviour : PersonBehaviour
    {
        [Serializable]
        public class StaminaActionCost
        {
            [SerializeField] [Range(0.0f, ushort.MaxValue)] private float m_jumpCost;
            [SerializeField] [Range(0.0f, ushort.MaxValue)] private float m_runCost;

            public float JumpCost
            {
                get { return this.m_jumpCost; }                
            }
            public float RunCost
            {                
                get { return this.m_runCost; }
            }
        }

        [SerializeField] private StaminaActionCost m_staminaConsumption;

        private UnknownWorld.Manager.ObserverManager m_observerManager;

        public StaminaActionCost StaminaConsumption
        {
            get { return this.m_staminaConsumption; }            
        }


        protected override void Awake()
        {
            base.Awake();

            m_observerManager = FindObjectOfType<UnknownWorld.Manager.ObserverManager>();
            StaminaUIUpdate += (stamina) => {
                StaminaSlider.value =
                UnknownWorld.Utility.Methods.VectorOperations.Map(stamina, StaminaMin, StaminaMax, StaminaSlider.minValue, StaminaSlider.maxValue);
            };
            HealthUIUpdate += (health) => {
                HealthSlider.value =
                UnknownWorld.Utility.Methods.VectorOperations.Map(health, HealthMin, HealthMax, HealthSlider.minValue, HealthSlider.maxValue);
            };
        }

        protected override void Update()
        {
            base.Update();            
        }


        protected override void Death()
        {
            base.Death();
            gameObject.layer = 15;
        }

        protected override void SetIsActive(bool isActive)
        {
            if (IsActive == isActive) return;

            base.SetIsActive(isActive);

            if (!IsActive)
            {
                m_observerManager.ClearCharacterMask(Id);
                m_areaContainer.DisableAllAreas();
            }
            else
            {
                m_areaContainer.EnableAllAreas();
            }
        }

    }
}
