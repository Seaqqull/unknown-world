using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameMenu : MonoBehaviour {

    public static bool isPaused = false;
    public GameObject pauseMenuUI;
    

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }


    private void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0.0f;
        isPaused = true;
    }

    private void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1.0f;
        isPaused = false;
    }    


    public void Exit()
    {
        Application.Quit();
    }

    public void LoadMainMenu()
    {

    }
}
