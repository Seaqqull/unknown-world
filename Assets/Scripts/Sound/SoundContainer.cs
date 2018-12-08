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


        private bool Play(int index)
        {
            if ((index < 0) ||
                (index >= m_audios.Count) ||
                (m_audios[index].IsAttached))
                return false;

            if (!m_audios[index].InitializeAudioSource(gameObject))
                return false;

            m_audios[index].Play();

            if (!m_audios[index].Loop)
                RunLater(() => m_audios[index].DestroyAudioSource(), m_audios[index].AudioLength + m_audios[index].PlayDelay);

            return true;
        }

        // on clip end - use on loop clips
        private bool Stop(int index)
        {
            if ((index < 0) ||
                (index >= m_audios.Count) ||
                (!m_audios[index].IsAttached))
                return false;

            RunLater(() => m_audios[index].DestroyAudioSource(),
                m_audios[index].AudioLength - m_audios[index].AudioTime);

            return true;
        }

        private bool PlayInstant(int index)
        {
            if ((index < 0) ||
                (index >= m_audios.Count) ||
                (m_audios[index].IsAttached))
                return false;

            if (!m_audios[index].InitializeAudioSource(gameObject))
                return false;

            m_audios[index].PlayInstant();

            if (!m_audios[index].Loop)
                RunLater(() => m_audios[index].DestroyAudioSource(), m_audios[index].AudioLength);

            return true;
        }

        private bool StopInstant(int index)
        {
            if ((index < 0) ||
                (index >= m_audios.Count) ||
                (!m_audios[index].IsAttached))
                return false;
            
            m_audios[index].DestroyAudioSource();

            return true;
        }

        private bool PlayDelayed(int index, float delay)
        {
            if ((index < 0) ||
                (index >= m_audios.Count) ||
                (m_audios[index].IsAttached))
                return false;

            if (!m_audios[index].InitializeAudioSource(gameObject))
                return false;

            m_audios[index].PlayDelayed(delay);

            if (!m_audios[index].Loop)
                RunLater(() => m_audios[index].DestroyAudioSource(), m_audios[index].AudioLength + delay);

            return true;
        }

        private bool StopDelayed(int index, float delay)
        {
            if ((index < 0) ||
                (index >= m_audios.Count) ||
                (!m_audios[index].IsAttached))
                return false;

            RunLater(() => m_audios[index].DestroyAudioSource(), delay);

            return true;
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


        public bool Play(string audioName, bool searchInNested = true, bool onlyDirect = true)
        {
            for (int i = 0; i < m_audios.Count; i++)
            {
                if (m_audios[i].Name == audioName)
                {
                    return Play(i);
                }
            }

            if (!searchInNested) return false;

            for (int i = 0; i < m_containers.Count; i++)
            {
                if (m_containers[i].Play(audioName, !onlyDirect, false))
                {
                    return true;
                }
            }

            return false;
        }

        public bool Stop(string audioName, bool searchInNested = true, bool onlyDirect = true)
        {
            for (int i = 0; i < m_audios.Count; i++)
            {
                if (m_audios[i].Name == audioName)
                {
                    return Stop(i);
                }
            }

            if (!searchInNested) return false;

            for (int i = 0; i < m_containers.Count; i++)
            {
                if (m_containers[i].Stop(audioName, !onlyDirect, false))
                {
                    return true;
                }
            }

            return false;
        }

        public bool PlayInstant(string audioName, bool searchInNested = true, bool onlyDirect = true)
        {
            for (int i = 0; i < m_audios.Count; i++)
            {
                if (m_audios[i].Name == audioName)
                {
                    return PlayInstant(i);
                }
            }

            if (!searchInNested) return false;

            for (int i = 0; i < m_containers.Count; i++)
            {
                if (m_containers[i].PlayInstant(audioName, !onlyDirect, false))
                {
                    return true;
                }
            }

            return false;
        }

        public bool StopInstant(string audioName, bool searchInNested = true, bool onlyDirect = true)
        {
            for (int i = 0; i < m_audios.Count; i++)
            {
                if (m_audios[i].Name == audioName)
                {
                    return StopInstant(i);
                }
            }

            if (!searchInNested) return false;

            for (int i = 0; i < m_containers.Count; i++)
            {
                if (m_containers[i].StopInstant(audioName, !onlyDirect, false))
                {
                    return true;
                }
            }

            return false;
        }

        public bool PlayDelayed(string audioName, float delay, bool searchInNested = true, bool onlyDirect = true)
        {
            for (int i = 0; i < m_audios.Count; i++)
            {
                if (m_audios[i].Name == audioName)
                {
                    return PlayDelayed(i, delay);
                }
            }

            if (!searchInNested) return false;

            for (int i = 0; i < m_containers.Count; i++)
            {
                if (m_containers[i].PlayDelayed(audioName, delay, !onlyDirect, false))
                {
                    return true;
                }
            }

            return false;
        }

        public bool StopDelayed(string audioName, float delay, bool searchInNested = true, bool onlyDirect = true)
        {
            for (int i = 0; i < m_audios.Count; i++)
            {
                if (m_audios[i].Name == audioName)
                {
                    return StopDelayed(i, delay);
                }
            }

            if (!searchInNested) return false;

            for (int i = 0; i < m_containers.Count; i++)
            {
                if (m_containers[i].StopDelayed(audioName, delay, !onlyDirect, false))
                {
                    return true;
                }
            }

            return false;
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
