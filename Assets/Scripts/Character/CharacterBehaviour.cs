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
        }

        protected override void Update()
        {
            base.Update();
            IsActive = m_isPersonActive; // only for editor
        }

        protected override void SetIsActive(bool isActive)
        {
            if (m_isActive == isActive) return;

            base.SetIsActive(isActive);

            if (!m_isActive)
                m_observerManager.ClearCharacterMask(m_id);
        }
    }
}
