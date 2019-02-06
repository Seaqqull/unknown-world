using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour {

    public static bool isPaused = false;
    public GameObject pauseMenuUI;
    public Slider loadingSlider;

    public float delayOnDead = 3.0f;
    public GameObject panelDead;

    private Image imageDead = null;
    

    public bool IsDeadActive
    {
        get { return imageDead != null; }
    }


    private void Update()
    {
        if ((imageDead == null) && 
            (Input.GetKeyDown(KeyCode.Escape)))
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

        if (imageDead != null)
        {
            var tempColor = imageDead.color;
            tempColor.a += ((delayOnDead * Time.deltaTime) / delayOnDead);

            imageDead.color = tempColor;
        }
    }


    private IEnumerator LoadAsynchronously(int sceneIndex)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!loadOperation.isDone)
        {
            loadingSlider.value =
                Mathf.Clamp01(loadOperation.progress / 0.9f);

            yield return null;
        }
    }

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


    public void OnDead()
    {
        panelDead.SetActive(true);
        imageDead = panelDead.GetComponent<Image>();
        
        RunLater(
            () => LoadLevel(0)
            , delayOnDead);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0.0f;
        isPaused = true;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1.0f;
        isPaused = false;
    }

    public void LoadLevel(int sceneIndex)
    {
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }
}
