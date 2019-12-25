using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PrepareGame : MonoBehaviour
{
    public GameObject text;

    void Start()
    {
        StartCoroutine(LoadScene(Constants.StartSceneName));
    }

    private IEnumerator LoadScene(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        float timer = 0.0f;
        while (!operation.isDone)
        {
            yield return new WaitForEndOfFrame();


            if (operation.progress >= 0.9f && timer > 1f)
            {
                text.SetActive(false);
                timer = 0;
                while(timer < 1f)
                {
                    timer += Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }
                operation.allowSceneActivation = true;
            }
            timer += Time.deltaTime;
            //if (oper.progress >= 0.9f)
            //{
            //    progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer);
            //    //Image가 아니라 Slider의 경우 progressBar.value로 간단히 구현이 가능합니다만
            //    // 이미지가 찌그러진 것이 펴지는 것처럼 나오기 때문에 비추천하는 방법입니다.

            //    if (progressBar.fillAmount == 1.0f)
            //        oper.allowSceneActivation = true;
            //}
            //else
            //{
            //    progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, oper.progress, timer);
            //    if (progressBar.fillAmount >= oper.progress)
            //    {
            //        timer = 0f;
            //    }
            //}
        }
    }
}
