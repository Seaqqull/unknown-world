using UnityEngine;


namespace UnknownWorld.Random
{
    [System.Serializable]
    public abstract class TimeLimiterExecutor : RandomnessExecutor
    {
        // minimal time limit setted by child class
        [SerializeField] [Range(0.0f, ushort.MaxValue)] private float m_timeLimit;

        private float m_timeSinceExecution;

        public float TimeLimit
        {
            get { return this.m_timeLimit; }
            set { this.m_timeLimit = value; }
        }


        protected override void Update()
        {
            base.Update();

            if (!m_isBusy) return;

            m_timeSinceExecution += Time.deltaTime;

            if (m_timeSinceExecution >= m_timeLimit)
            {
                DoBusy(false);
            }
        }


        protected override void DoBusy(bool isBusy)
        {
            m_isBusy = isBusy;

            if (isBusy)
                m_timeSinceExecution = 0.0f;
        }
    }
}
