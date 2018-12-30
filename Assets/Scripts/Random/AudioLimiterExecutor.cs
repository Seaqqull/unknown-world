using System.Collections.Generic;
using UnityEngine;


namespace UnknownWorld.Random
{
    [System.Serializable]
    public class AudioLimiterExecutor : TimeLimiterExecutor
    {
        [SerializeField] private UnknownWorld.Sound.SoundContainer m_container;
        [SerializeField] private bool m_timeLimitFromAudio = false;
        [SerializeField] private List<string> m_audios;

        private System.Random m_random;
        private float m_initialTimeLimit;

        public UnknownWorld.Sound.SoundContainer Container
        {
            get { return this.m_container; }
            set { this.m_container = value; }
        }
        public List<string> Audios
        {
            get
            {
                return this.m_audios ?? (this.m_audios = new List<string>());
            }
            set { this.m_audios = value; }
        }
        public new float TimeLimit
        {
            get { return base.TimeLimit; }
            set
            {
                base.TimeLimit = value;
                m_initialTimeLimit = value;
            }
        }


        protected override void Awake()
        {
            base.Awake();

            m_random = new System.Random((int)Id + 
                (int)UnknownWorld.Utility.Methods.Hasher.CurrentTimeMillis());

            m_initialTimeLimit = base.TimeLimit;
        }


        protected override bool DoExecute()
        {
            int audioNumber = m_random.Next(0, m_audios.Count);

            if (m_timeLimitFromAudio)
                base.TimeLimit = m_container.Audios.Find(audio => (audio.Name == m_audios[audioNumber])).AudioLength;
            else
                base.TimeLimit = m_initialTimeLimit;

            return (m_container.Play(m_audios[audioNumber]) != string.Empty);
        }
    }
}
