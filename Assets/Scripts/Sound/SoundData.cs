using UnityEngine;
using UnityEngine.Audio;

namespace UnknownWorld.Sound.Data
{
    [System.Serializable]
    public class AudioSource
    {
        [System.Serializable]
        public class SoundSettingDetection
        {            
            [SerializeField] [Range(0.0f, ushort.MaxValue)] public float m_audibility = 100.0f;

            [SerializeField] [Range(0.0f, ushort.MaxValue)] public float m_maxDistance = 100.0f;            
            [SerializeField] [Range(0.0f, ushort.MaxValue)] public float m_minDistance;

            [SerializeField] public bool m_cutOnMin = false;
            [SerializeField] public bool m_cutOnMax = true;

            [SerializeField] public AnimationCurve m_curveAudibility = AnimationCurve.Linear(0, 1, 1, 0);
        }

        [System.Serializable]
        public class SoundSetting3D
        {            
            [SerializeField] [Range(0.0f, ushort.MaxValue)] public float m_maxDistance = 100.0f;
            [SerializeField] [Range(0.0f, ushort.MaxValue)] public float m_minDistance;

            [SerializeField] [Range(0.0f, 5.0f)] public float m_dopplerLevel = 1.0f;
            [SerializeField] [Range(0.0f, 360.0f)] public float m_spread;

            [SerializeField] public AnimationCurve m_curveVolume = AnimationCurve.Linear(0, 1, 1, 0);
        }


        [SerializeField] private string m_name;

        [SerializeField] private AudioMixerGroup m_output;
        [SerializeField] private AudioClip m_audioClip;

        [SerializeField] private bool m_loop;
        [SerializeField] private bool m_mute;

        [SerializeField] [Range(0.0f, ushort.MaxValue)] private float m_playDelay;
        [SerializeField] [Range(0.0f, 1.1f)] private float m_reverbZoneMix = 1.0f;        
        [SerializeField] [Range(-3.0f, 3.0f)] private float m_pitch = 1.0f;
        [SerializeField] [Range(0.0f, 1.0f)] private float m_volume = 1.0f;
        [SerializeField] [Range(0.0f, 1.0f)] private float m_spatialBlend;

        [SerializeField] SoundSettingDetection m_settingDetection;        
        [SerializeField] SoundSetting3D m_setting3D;
        

        private UnityEngine.AudioSource m_source;

        public UnityEngine.AudioSource Source
        {
            get { return this.m_source; }
        }
        public float OutherRadiusDetection
        {
            get { return this.m_settingDetection.m_maxDistance; }
        }
        public AnimationCurve CurveVolume
        {
            get { return this.m_setting3D.m_curveVolume; }
        }        
        public float InnerRadiusDetection
        {
            get { return this.m_settingDetection.m_minDistance; }
        }
        public float OutherRadius3D
        {
            get { return this.m_setting3D.m_maxDistance; }
        }
        public float InnerRadius3D
        {
            get { return this.m_setting3D.m_minDistance; }
        }
        public float AudioLength
        {
            get { return (this.m_audioClip)? this.m_audioClip.length : 0.0f; }
        }
        public float AudioTime
        {
            get { return (this.m_source) ? this.m_source.time : 0.0f; }
        }
        public float PlayDelay
        {
            get { return this.m_playDelay; }
            set { this.m_playDelay = value; }
        }
        public bool IsAttached
        {
            get { return (m_source != null); }
        }
        public string Name
        {
            get { return this.m_name; }
            set { this.m_name = value; }
        }
        public bool Loop
        {
            get { return this.m_loop; }
            set { this.m_loop = value; }
        }


        public void Play()
        {
            if (!m_source) return;

            if (m_playDelay == 0.0f)
                m_source.Play();
            else
                m_source.PlayDelayed(m_playDelay);
        }

        public void PlayInstant()
        {
            if (m_source)
                m_source.Play();         
        }

        public void DestroyAudioSource()
        {
            if (!m_source) return;

            m_source.Stop();
            UnityEngine.Object.Destroy(m_source);

            m_source = null;
        }
      
        public void PlayDelayed(float delay)
        {
            if (m_source)
                m_source.PlayDelayed(delay);
        }

        public bool InitializeAudioSource(GameObject gameObject)
        {
            if ((!m_audioClip) ||
                (!m_output)) return false;

            m_source = gameObject.AddComponent<UnityEngine.AudioSource>();

            m_source.clip = m_audioClip;
            m_source.outputAudioMixerGroup = m_output;
            m_source.mute = m_mute;
            m_source.playOnAwake = false;
            m_source.loop = m_loop;
            m_source.volume = m_volume;
            m_source.pitch = m_pitch;
            m_source.spatialBlend = m_spatialBlend;
            m_source.reverbZoneMix = m_reverbZoneMix;

            m_source.dopplerLevel = m_setting3D.m_dopplerLevel;
            m_source.spread = m_setting3D.m_spread;
            m_source.minDistance = m_setting3D.m_minDistance;
            m_source.maxDistance = m_setting3D.m_maxDistance;

            m_source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, m_setting3D.m_curveVolume);
            m_source.rolloffMode = AudioRolloffMode.Custom;

            return true;
        }

        public float GetAudibility(Vector3 source, Vector3 listener)
        {
            if (!m_source) return 0.0f;

            float distance = Vector3.Distance(source, listener);

            if (((m_settingDetection.m_cutOnMin) && (distance < m_settingDetection.m_minDistance)) ||
               ((m_settingDetection.m_cutOnMax) && (distance > m_settingDetection.m_maxDistance)))
                return 0.0f;            

            float relativeAudibility = UnknownWorld.Utility.Methods.VectorOperations.
                Map(distance, (m_settingDetection.m_minDistance < distance) ? m_settingDetection.m_minDistance : distance, 
                    (m_settingDetection.m_maxDistance > distance) ? m_settingDetection.m_maxDistance : distance, 0, 1);

            return m_settingDetection.m_audibility * m_settingDetection.m_curveAudibility.Evaluate(relativeAudibility);
        }
    }
}
