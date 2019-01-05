using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Linq;
using System;

[System.Serializable]
public class SettingsMenu : MonoBehaviour {

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
    [SerializeField] private AudioMixer m_audioMixer;

    private Resolution[] m_resolutions;


    private void Start()
    {
        m_resolutions = Screen.resolutions;
        m_graphicsDropdown.ClearOptions();

        List<string> options = Array.ConvertAll(m_resolutions, x => x.width + "x" + x.height).ToList();
        m_graphicsDropdown.AddOptions(options);
        m_graphicsDropdown.value = 
            options.FindIndex((res) => res == Screen.currentResolution.width + " x " + Screen.currentResolution.height);
        m_graphicsDropdown.RefreshShownValue();

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
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Screen.SetResolution(m_resolutions[resolutionIndex].width, m_resolutions[resolutionIndex].height, Screen.fullScreen);
    }
}
