using UnityEngine;


namespace UnknownWorld.Random
{
    [System.Serializable]
    public abstract class RandomnessExecutor : MonoBehaviour
    {
        [SerializeField] [Range(0.0f, ushort.MaxValue)] private float m_pollFrequency = 1.0f;
        [SerializeField] [Range(0.0f, 1.0f)] private float m_possibility = 0.2f;

        private bool m_isPollAllowed = true;
        protected bool m_isBusy = false;
        private System.Random m_random;
        private float m_timeSincePoll;

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


        protected virtual void Awake()
        {
            m_random = new System.Random();
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
