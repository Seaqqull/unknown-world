using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Linq;
using System;

public class SettingsMenu : MonoBehaviour {

    [SerializeField] private AudioMixer m_audioMixer;
    [SerializeField] private Dropdown m_graphicsDropdown;

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
    }

    public void SetVolume(float volume)
    {
        m_audioMixer.SetFloat("Volume", volume);
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
