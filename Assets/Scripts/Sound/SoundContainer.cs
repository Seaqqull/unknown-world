using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnknownWorld.Sound
{
    public class SoundContainer : MonoBehaviour
    {
        [SerializeField] private string m_name;

        [SerializeField] private bool m_isAccommodate = true;
        [SerializeField] private bool m_isActive = true;
        [SerializeField] private bool m_isNested = true;

        [SerializeField] private List<UnknownWorld.Sound.Data.AudioSource> m_audios;           


        private List<UnknownWorld.Sound.SoundContainer> m_containers;

        public List<UnknownWorld.Sound.SoundContainer> Containers
        {
            get
            {
                return (this.m_containers) ??
                    (this.m_containers = new List<UnknownWorld.Sound.SoundContainer>());
            }
            set
            {
                this.m_containers = value;
            }
        }
        public List<UnknownWorld.Sound.Data.AudioSource> Audios
        {
            get
            {
                return (this.m_audios) ??
                    (this.m_audios = new List<UnknownWorld.Sound.Data.AudioSource>());
            }
        }
        public bool IsAccommodate
        {
            get { return this.m_isAccommodate; }
        }
        public bool IsActive
        {
            get { return this.m_isActive; }
            set { this.m_isActive = value; }
        }
        public bool IsNested
        {
            get { return this.m_isNested; }            
        }        


        private void Awake()
        {
            if (!IsAccommodate) return;

            Containers = GetComponentsInChildren<UnknownWorld.Sound.SoundContainer>().
                OfType<UnknownWorld.Sound.SoundContainer>().ToList();

            for (int i = 0; i < m_containers.Count; i++)
            {
                if ((!m_containers[i].IsNested) ||
                    (m_containers[i].gameObject.transform.parent != transform)) // exclude not direct childs
                {
                    m_containers.RemoveAt(i--);
                }
            }           
        }


        private string Play(int index)
        {
            if ((index < 0) ||
                (index >= m_audios.Count))
                return string.Empty;

            string audioKey = m_audios[index].InitializeAudioSource(gameObject);

            if (audioKey == string.Empty)
                return string.Empty;

            m_audios[index].Play(audioKey);

            if (!m_audios[index].Loop)
                RunLater(
                    () => m_audios[index].DestroyAudioSource(audioKey),
                    m_audios[index].PlayDelay + ((m_audios[index].PlayTime == 0.0f)? m_audios[index].AudioLength : m_audios[index].PlayTime)
                );

            return audioKey;
        }

        private bool Stop(int index, string audioKey)
        {
            if ((index < 0) ||
                (index >= m_audios.Count) ||
                (!m_audios[index].ContainAudio(audioKey)))
                return false;

            RunLater(() => m_audios[index].DestroyAudioSource(audioKey),
                m_audios[index].AudioLength - m_audios[index].Records[audioKey].time);

            return true;
        }
        

        private string PlayInstant(int index)
        {
            if ((index < 0) ||
                (index >= m_audios.Count))
                return string.Empty;

            string audioKey = m_audios[index].InitializeAudioSource(gameObject);

            if (audioKey == string.Empty)
                return string.Empty;

            m_audios[index].PlayInstant(audioKey);

            if (!m_audios[index].Loop)
                RunLater(
                    () => m_audios[index].DestroyAudioSource(audioKey),
                    ((m_audios[index].PlayTime == 0.0f)? m_audios[index].AudioLength : m_audios[index].PlayTime)
                );

            return audioKey;
        }

        private bool StopInstant(int index, string audioKey)
        {
            if ((index < 0) ||
                (index >= m_audios.Count) ||
                (!m_audios[index].ContainAudio(audioKey)))
                return false;
            
            m_audios[index].DestroyAudioSource(audioKey);

            return true;
        }


        private string PlayDelayed(int index, float delay)
        {
            if ((index < 0) ||
                (index >= m_audios.Count))
                return string.Empty;

            string audioKey = m_audios[index].InitializeAudioSource(gameObject);

            if (audioKey == string.Empty)
                return string.Empty;

            m_audios[index].PlayDelayed(audioKey, delay);

            if (!m_audios[index].Loop)
                RunLater(
                    () => m_audios[index].DestroyAudioSource(audioKey),
                    delay + ((m_audios[index].PlayTime == 0.0f) ? m_audios[index].AudioLength : m_audios[index].PlayTime)
                );

            return audioKey;
        }

        private bool StopDelayed(int index, float delay, string audioKey)
        {
            if ((index < 0) ||
                (index >= m_audios.Count) ||
                (!m_audios[index].ContainAudio(audioKey)))
                return false;

            RunLater(() => m_audios[index].DestroyAudioSource(audioKey), delay);

            return true;
        }


        private string Play(int index, float playTime)
        {
            if ((index < 0) ||
                (index >= m_audios.Count))
                return string.Empty;

            string audioKey = m_audios[index].InitializeAudioSource(gameObject);

            if (audioKey == string.Empty)
                return string.Empty;

            m_audios[index].Play(audioKey);

            RunLater(
                () => m_audios[index].DestroyAudioSource(audioKey),
                m_audios[index].PlayDelay + ((playTime < m_audios[index].AudioLength) ? playTime : m_audios[index].AudioLength)
            );

            return audioKey;
        }

        private string PlayInstant(int index, float playTime)
        {
            if ((index < 0) ||
                (index >= m_audios.Count))
                return string.Empty;

            string audioKey = m_audios[index].InitializeAudioSource(gameObject);

            if (audioKey == string.Empty)
                return string.Empty;

            m_audios[index].PlayInstant(audioKey);

            RunLater(
                () => m_audios[index].DestroyAudioSource(audioKey),
                ((playTime < m_audios[index].AudioLength) ? playTime : m_audios[index].AudioLength)
            );

            return audioKey;
        }

        private string PlayDelayed(int index, float delay, float playTime)
        {
            if ((index < 0) ||
                (index >= m_audios.Count))
                return string.Empty;

            string audioKey = m_audios[index].InitializeAudioSource(gameObject);

            if (audioKey == string.Empty)
                return string.Empty;

            m_audios[index].PlayDelayed(audioKey, delay);

            RunLater(
                () => m_audios[index].DestroyAudioSource(audioKey),
                delay + ((playTime < m_audios[index].AudioLength) ? playTime : m_audios[index].AudioLength)
            );

            return audioKey;
        }

        // delay call methods
        private void RunLater(System.Action method, float waitSeconds)
        {
            if (waitSeconds < 0 || method == null)
            {
                return;
            }
            StartCoroutine(RunLaterCoroutine(method, waitSeconds));
        }

        private IEnumerator RunLaterCoroutine(System.Action method, float waitSeconds)
        {
            yield return new WaitForSeconds(waitSeconds);
            method();
        }


        public string Play(string audioName, bool searchInNested = true, bool onlyDirect = true)
        {
            for (int i = 0; i < m_audios.Count; i++)
            {
                if (m_audios[i].Name == audioName)
                {
                    return Play(i);
                }
            }

            if (!searchInNested) return string.Empty;

            for (int i = 0; i < m_containers.Count; i++)
            {
                string audioKey = m_containers[i].Play(audioName, !onlyDirect, false);

                if (audioKey != string.Empty)
                    return audioKey;
            }

            return string.Empty;
        }

        public bool Stop(string audioName, string audioKey, bool searchInNested = true, bool onlyDirect = true)
        {
            for (int i = 0; i < m_audios.Count; i++)
            {
                if (m_audios[i].Name == audioName)
                {
                    return Stop(i, audioKey);
                }
            }

            if (!searchInNested) return false;

            for (int i = 0; i < m_containers.Count; i++)
            {
                if (m_containers[i].Stop(audioName, audioKey, !onlyDirect, false))
                {
                    return true;
                }
            }

            return false;
        }


        public string PlayInstant(string audioName, bool searchInNested = true, bool onlyDirect = true)
        {
            for (int i = 0; i < m_audios.Count; i++)
            {
                if (m_audios[i].Name == audioName)
                {
                    return PlayInstant(i);
                }
            }

            if (!searchInNested) return string.Empty;

            for (int i = 0; i < m_containers.Count; i++)
            {
                string audioKey = m_containers[i].PlayInstant(audioName, !onlyDirect, false);

                if (audioKey != string.Empty)
                    return audioKey;
            }

            return string.Empty;
        }

        public bool StopInstant(string audioName, string audioKey, bool searchInNested = true, bool onlyDirect = true)
        {
            for (int i = 0; i < m_audios.Count; i++)
            {
                if (m_audios[i].Name == audioName)
                {
                    return StopInstant(i, audioKey);
                }
            }

            if (!searchInNested) return false;

            for (int i = 0; i < m_containers.Count; i++)
            {
                if (m_containers[i].StopInstant(audioName, audioKey, !onlyDirect, false))
                {
                    return true;
                }
            }

            return false;
        }


        public string PlayDelayed(string audioName, float delay, bool searchInNested = true, bool onlyDirect = true)
        {
            for (int i = 0; i < m_audios.Count; i++)
            {
                if (m_audios[i].Name == audioName)
                {
                    return PlayDelayed(i, delay);
                }
            }

            if (!searchInNested) return string.Empty;

            for (int i = 0; i < m_containers.Count; i++)
            {
                string audioKey = m_containers[i].PlayDelayed(audioName, delay, !onlyDirect, false);
                
                if (audioKey != string.Empty)
                    return audioKey;
            }

            return string.Empty;
        }

        public bool StopDelayed(string audioName, float delay, string audioKey, bool searchInNested = true, bool onlyDirect = true)
        {
            for (int i = 0; i < m_audios.Count; i++)
            {
                if (m_audios[i].Name == audioName)
                {
                    return StopDelayed(i, delay, audioKey);
                }
            }

            if (!searchInNested) return false;

            for (int i = 0; i < m_containers.Count; i++)
            {
                if (m_containers[i].StopDelayed(audioName, delay, audioKey, !onlyDirect, false))
                {
                    return true;
                }
            }

            return false;
        }


        public string Play(string audioName, float playTime, bool searchInNested = true, bool onlyDirect = true)
        {
            for (int i = 0; i < m_audios.Count; i++)
            {
                if (m_audios[i].Name == audioName)
                {
                    return Play(i, playTime);
                }
            }

            if (!searchInNested) return string.Empty;

            for (int i = 0; i < m_containers.Count; i++)
            {
                string audioKey = m_containers[i].Play(audioName, playTime, !onlyDirect, false);

                if (audioKey != string.Empty)
                    return audioKey;
            }

            return string.Empty;
        }

        public string PlayInstant(string audioName, float playTime, bool searchInNested = true, bool onlyDirect = true)
        {
            for (int i = 0; i < m_audios.Count; i++)
            {
                if (m_audios[i].Name == audioName)
                {
                    return PlayInstant(i, playTime);
                }
            }

            if (!searchInNested) return string.Empty;

            for (int i = 0; i < m_containers.Count; i++)
            {
                string audioKey = m_containers[i].PlayInstant(audioName, playTime, !onlyDirect, false);

                if (audioKey != string.Empty)
                    return audioKey;
            }

            return string.Empty;
        }

        public string PlayDelayed(string audioName, float delay, float playTime, bool searchInNested = true, bool onlyDirect = true)
        {
            for (int i = 0; i < m_audios.Count; i++)
            {
                if (m_audios[i].Name == audioName)
                {
                    return PlayDelayed(i, delay, playTime);
                }
            }

            if (!searchInNested) return string.Empty;

            for (int i = 0; i < m_containers.Count; i++)
            {
                string audioKey = m_containers[i].PlayDelayed(audioName, delay, playTime, !onlyDirect, false);

                if (audioKey != string.Empty)
                    return audioKey;
            }

            return string.Empty;
        }


        public float GetAudibility(Vector3 source, Vector3 listener, bool searchInNested = true, bool onlyDirect = true)
        {
            float noise = 0.0f, noiseT;

            for (int i = 0; i < m_audios.Count; i++)
            {
                noiseT = m_audios[i].GetAudibility(source, listener);
                if (noiseT > noise) noise = noiseT;
            }

            if (!searchInNested) return noise;

            for (int i = 0; i < m_containers.Count; i++)
            {
                noiseT = m_containers[i].GetAudibility(source, listener, !onlyDirect, false);
                if (noiseT > noise) noise = noiseT;
            }

            return noise;
        }

        public float GetAudibility(Vector3 listener, bool attentionToChildPosition = true, bool searchInNested = true, bool onlyDirect = true)
        {
            float noise = 0.0f, noiseT;

            for (int i = 0; i < m_audios.Count; i++)
            {
                noiseT = m_audios[i].GetAudibility(transform.position, listener);
                if (noiseT > noise) noise = noiseT;
            }

            if (!searchInNested) return noise;

            for (int i = 0; i < m_containers.Count; i++)
            {
                if (attentionToChildPosition)
                    noiseT = m_containers[i].GetAudibility(listener, attentionToChildPosition, !onlyDirect, false);
                else
                    noiseT = m_containers[i].GetAudibility(transform.position, listener, !onlyDirect, false);                

                if (noiseT > noise) noise = noiseT;
            }

            return noise;
        }

    }
}
