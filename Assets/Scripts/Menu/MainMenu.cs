using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

namespace UnknownWorld.UI
{
    [System.Serializable]
    public class MainMenu : MonoBehaviour
    {

        public Slider loadingSlider;


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


        public void ExitGame()
        {
            Application.Quit();
        }

        public void LoadLevel(int sceneIndex)
        {
            StartCoroutine(LoadAsynchronously(sceneIndex));
        }

    }
}
