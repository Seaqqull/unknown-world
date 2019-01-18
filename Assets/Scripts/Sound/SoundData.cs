using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

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
        [SerializeField] [Range(0.0f, ushort.MaxValue)] private float m_playTime;
        [SerializeField] [Range(-3.0f, 3.0f)] private float m_pitch = 1.0f;
        [SerializeField] [Range(0.0f, 1.0f)] private float m_volume = 1.0f;
        [SerializeField] [Range(0.0f, 1.0f)] private float m_spatialBlend;

        [SerializeField] SoundSettingDetection m_settingDetection;
        [SerializeField] SoundSetting3D m_setting3D;
        

        private Dictionary<string, UnityEngine.AudioSource> m_records;


        public Dictionary<string, UnityEngine.AudioSource> Records
        {
            get
            {
                return (this.m_records) ??
                    (this.m_records = new Dictionary<string, UnityEngine.AudioSource>());
            }
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
        public float PlayDelay
        {
            get { return this.m_playDelay; }
            set { this.m_playDelay = value; }
        }
        public float PlayTime
        {
            get { return this.m_playTime; }
            set { this.m_playTime = value; }
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


        public bool Play(string audioKey)
        {
            if (!Records.ContainsKey(audioKey))
                return false;

            if (m_playDelay == 0.0f)
                Records[audioKey].Play();
            else
                Records[audioKey].PlayDelayed(m_playDelay);

            return true;
        }

        public bool PlayInstant(string audioKey)
        {
            if (!Records.ContainsKey(audioKey))
                return false;

            Records[audioKey].Play();

            return true;
        }

        public bool ContainAudio(string audioKey)
        {
            return Records.ContainsKey(audioKey);
        }

        public bool DestroyAudioSource(string audioKey)
        {
            if (!Records.ContainsKey(audioKey))
                return false;

            Records[audioKey].Stop();
            UnityEngine.Object.Destroy(Records[audioKey]);

            Records.Remove(audioKey);

            return true;
        }
      
        public bool PlayDelayed(string audioKey, float delay)
        {
            if (!Records.ContainsKey(audioKey))
                return false;
            
            Records[audioKey].PlayDelayed(delay);

            return true;
        }

        public string InitializeAudioSource(GameObject gameObject)
        {
            if ((!m_output) ||
                (!m_audioClip)) return string.Empty;

            string sourceKey = UnknownWorld.Utility.Methods.Hasher.GenerateHash();
            Records.Add(sourceKey, gameObject.AddComponent<UnityEngine.AudioSource>());
            
            Records[sourceKey].clip = m_audioClip;
            Records[sourceKey].outputAudioMixerGroup = m_output;
            Records[sourceKey].mute = m_mute;
            Records[sourceKey].playOnAwake = false;
            Records[sourceKey].loop = m_loop;
            Records[sourceKey].volume = m_volume;
            Records[sourceKey].pitch = m_pitch;
            Records[sourceKey].spatialBlend = m_spatialBlend;
            Records[sourceKey].reverbZoneMix = m_reverbZoneMix;

            Records[sourceKey].dopplerLevel = m_setting3D.m_dopplerLevel;
            Records[sourceKey].spread = m_setting3D.m_spread;
            Records[sourceKey].minDistance = m_setting3D.m_minDistance;
            Records[sourceKey].maxDistance = m_setting3D.m_maxDistance;

            Records[sourceKey].SetCustomCurve(AudioSourceCurveType.CustomRolloff, m_setting3D.m_curveVolume);
            Records[sourceKey].rolloffMode = AudioRolloffMode.Custom;

            return sourceKey;
        }

        public float GetAudibility(Vector3 source, Vector3 listener)
        {
            if (Records.Count == 0) return 0.0f;

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
