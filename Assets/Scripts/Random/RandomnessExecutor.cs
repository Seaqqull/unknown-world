using UnityEngine;


namespace UnknownWorld.Random
{
    [System.Serializable]
    public abstract class RandomnessExecutor : MonoBehaviour
    {
        [SerializeField] [Range(0.0f, ushort.MaxValue)] private float m_pollFrequency = 1.0f;
        [SerializeField] [Range(0.0f, 1.0f)] private float m_possibility = 0.2f;

        private static uint m_idCounter = 0;

        private bool m_isPollAllowed = true;
        protected bool m_isBusy = false;
        private System.Random m_random;
        private float m_timeSincePoll;
        private uint m_id;

        public float PollFrequency
        {
            get { return this.m_pollFrequency; }
            set
            {
                if (m_isPollAllowed)
                    this.m_pollFrequency = value;
            }
        }
        public bool IsBusy
        {
            get { return this.m_isBusy; }
        }
        public uint Id
        {
            get { return this.m_id; }
        }


        protected virtual void Awake()
        {            
            m_id = m_idCounter++;

            m_random = new System.Random((int)m_id + 
                (int)UnknownWorld.Utility.Methods.Hasher.CurrentTimeMillis());
        }

        protected virtual void Update()
        {
            if ((m_isBusy) ||
                (m_isPollAllowed)) return;

            m_timeSincePoll += Time.deltaTime;

            if (m_timeSincePoll >= m_pollFrequency)
                m_isPollAllowed = true;
        }


        public virtual void Execute()
        {
            if (m_isBusy) return;

            if (DoExecute())
                DoBusy(true);
        }

        public virtual bool ExecuteIfPossible()
        {
            if ((m_isBusy) ||
                (!m_isPollAllowed))
                return false;

            m_isPollAllowed = false;
            m_timeSincePoll = 0;

            if (m_random.NextDouble() > m_possibility)
                return false;

            Execute();
            return true;
        }


        protected abstract void DoBusy(bool isBusy);
        protected abstract bool DoExecute();

    }
}
