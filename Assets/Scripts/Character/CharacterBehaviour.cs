using UnityEngine;

namespace UnknownWorld.Behaviour
{
    [System.Serializable]
    public class CharacterBehaviour : PersonBehaviour
    {
        private UnknownWorld.Manager.ObserverManager m_observerManager;

        protected override void Awake()
        {
            base.Awake();

            m_observerManager = FindObjectOfType<UnknownWorld.Manager.ObserverManager>();
            StaminaUIUpdate += (stamina) => {
                StaminaSlider.value = 
                UnknownWorld.Area.Data.VectorOperations.Map(stamina, StaminaMin, StaminaMax, StaminaSlider.minValue, StaminaSlider.maxValue);
            };
            HealthUIUpdate += (health) => {
                HealthSlider.value = 
                UnknownWorld.Area.Data.VectorOperations.Map(health, HealthMin, HealthMax, HealthSlider.minValue, HealthSlider.maxValue);
            };
        }

        protected override void Update()
        {
            base.Update();            
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
