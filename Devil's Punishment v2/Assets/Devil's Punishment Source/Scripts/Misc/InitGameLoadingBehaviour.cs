using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Misc
{
    public class InitGameLoadingBehaviour : MonoBehaviour
    {
        public Slider loadingSlider;
        public TextMeshProUGUI sceneNameText;

        private void Start()
        {
//            if(Application.platform == RuntimePlatform.WindowsEditor | Run)
            ChooseGameToLoad();
        }

        public void ChooseGameToLoad()
        {
            //sceneNameText.text += "";
            StartCoroutine(LoadGame());
        }

        private IEnumerator LoadGame()
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(2);

            while (!operation.isDone)
            {
                loadingSlider.value = operation.progress / 0.9f;
                yield return null;
            }
        }
    }
    
}
