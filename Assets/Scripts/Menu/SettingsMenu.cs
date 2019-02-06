using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Linq;
using System;

namespace UnknownWorld.UI
{
    [System.Serializable]
    public class SettingsMenu : MonoBehaviour
    {

        [System.Serializable]
        public class AudioSetting
        {
            [SerializeField] private string m_exposedParam;
            [SerializeField] private Slider m_slider;

            public string ExposedParamn
            {
                get { return this.m_exposedParam; }
            }
            public Slider Slider
            {
                get { return this.m_slider; }
            }


            public void Initialize()
            {
                m_slider.value = PlayerPrefs.GetFloat(m_exposedParam);
            }
        }


        [SerializeField] private AudioSetting[] m_audioSettings;
        [SerializeField] private Dropdown m_graphicsDropdown;
        [SerializeField] private Toggle m_fullscreen;

        [SerializeField] private AudioMixer m_audioMixer;


        private Resolution[] m_resolutions;


        private void Start()
        {
            m_resolutions = Screen.resolutions.Where((res) => res.refreshRate == 60).ToArray();
            m_graphicsDropdown.ClearOptions();

            List<string> options = Array.ConvertAll(m_resolutions, x => x.width + "x" + x.height).ToList();
            m_graphicsDropdown.AddOptions(options);
            m_graphicsDropdown.value =
                options.FindIndex((res) => res == (Screen.width + "x" + Screen.height));
            m_graphicsDropdown.RefreshShownValue();

            m_fullscreen.isOn = Screen.fullScreen;

            for (int i = 0; i < m_audioSettings.Length; i++)
            {
                m_audioSettings[i].Initialize();
            }
        }


        private float GetExposedValue(string audioName)
        {
            for (int i = 0; i < m_audioSettings.Length; i++)
            {
                if (m_audioSettings[i].ExposedParamn == audioName)
                {
                    return m_audioSettings[i].Slider.value;
                }
            }
            return 0.0f;
        }

        private IEnumerator WaitForScreenChange(bool fullscreen, int widthIncome = 0, int heightIncome = 0)
        {
            int width = (widthIncome == 0) ? Screen.width : widthIncome;
            int height = (heightIncome == 0) ? Screen.height : heightIncome;

            Screen.fullScreen = fullscreen;

            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            Screen.SetResolution(width, height, Screen.fullScreen);
        }


        public void SetMusic(float volume)
        {
            m_audioMixer.SetFloat("Main", GetExposedValue("Main"));
        }

        public void SetVolume(float volume)
        {
            m_audioMixer.SetFloat("Volume", GetExposedValue("Volume"));
        }

        public void SetEffects(float volume)
        {
            m_audioMixer.SetFloat("Effects", GetExposedValue("Effects"));
        }

        public void SetInterface(float volume)
        {
            m_audioMixer.SetFloat("Interface", GetExposedValue("Interface"));
        }

        public void SetGraphicQuality(int index)
        {
            QualitySettings.SetQualityLevel(index);
        }

        public void SetFullscreen(bool isFullscreen)
        {
            StartCoroutine(WaitForScreenChange(isFullscreen, Screen.width, Screen.height));
        }

        public void SetResolution(int resolutionIndex)
        {
            StartCoroutine(WaitForScreenChange(Screen.fullScreen, m_resolutions[resolutionIndex].width, m_resolutions[resolutionIndex].height));
        }
    }
}
