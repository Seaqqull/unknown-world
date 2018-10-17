using UnityEngine;

namespace UnknownWorld.Behaviour
{
    [System.Serializable]
    public abstract class PersonBehaviour : MonoBehaviour
    {
        [SerializeField] protected float m_movementSpeed;
        [SerializeField] protected float m_rotationSpeed;
        [SerializeField] protected float m_stamina;
        [SerializeField] protected float m_health;        

        protected UnknownWorld.Area.Target.TracingAreaContainer m_areaContainer;
        protected static uint m_idCounter = 0;
        protected uint m_id;

        public UnknownWorld.Area.Target.TracingAreaContainer AreaContainer
        {
            get
            {
                return this.m_areaContainer;
            }
        }
        public float RotationSpeed
        {
            get { return this.m_rotationSpeed; }
        }
        public float MovementSpeed
        {
            get { return this.m_movementSpeed; }
        }
        public float Stamina
        {
            get
            {
                return this.m_stamina;
            }
        }
        public float Health
        {
            get
            {
                return this.m_health;
            }
        }
        public uint Id
        {
            get
            {
                return this.m_id;
            }
        }


        protected virtual void Awake()
        {
            m_areaContainer = GetComponent<UnknownWorld.Area.Target.TracingAreaContainer>();

            m_id = m_idCounter++;
        }

    }
}
