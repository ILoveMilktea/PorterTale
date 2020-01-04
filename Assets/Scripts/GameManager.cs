using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

static class Constants
{
    public const string WorldMapSceneName = "WorldMap";
    public const string StartSceneName = "StartScene";
    public const string FightSceneName = "FightScene"; // "Fight"
    public const string UpgradeSceneName = "Upgrade";
    public const string VillageSceneName = "VillageScene";
    public const string QuestHouseSceneName = "QuestHouseScene";

    public const string PlayerTag = "Player";
    public const string EnemyTag = "Enemy";
}

public class GameManager : MonoSingleton<GameManager>
{
    public Image cutEffectImage;
    public Image fadeEffectImage;
    public Image slideEffectImage;

    private bool IsUIEffectEnd;

    protected override void Init()
    {
        DontDestroyOnLoad(Instance);
    }

    public void EndUIEffect()
    {
        IsUIEffectEnd = true;
    }

    public void SceneStart(string sceneName)
    {
        switch (sceneName)
        {
            case Constants.StartSceneName:
                //IsUIEffectEnd = false;
                //StartCoroutine(UIEffect.CutIn(cutEffectImage, UIEffectEndListener));
                break;
            case Constants.WorldMapSceneName:
                IsUIEffectEnd = false;
                StartCoroutine(UIEffect.CutIn(cutEffectImage, UIEffectEndListener));
                break;
            case Constants.FightSceneName:
                IsUIEffectEnd = false;
                if (cutEffectImage.material.GetFloat("_Radius") < 1.0f)
                {
                    SetSlideImagePos(true);
                    StartCoroutine(UIEffect.CutIn(cutEffectImage, UIEffectEndListener));

                }
                else
                {
                    StartCoroutine(UIEffect.SlideDownOut(slideEffectImage, UIEffectEndListener));
                }
                break;
            case Constants.UpgradeSceneName:
                IsUIEffectEnd = false;
                if (cutEffectImage.material.GetFloat("_Radius") < 1.0f)
                {
                    SetSlideImagePos(false);
                    StartCoroutine(UIEffect.CutIn(cutEffectImage, UIEffectEndListener));

                }
                else
                {
                    StartCoroutine(UIEffect.SlideDownOut(slideEffectImage, UIEffectEndListener));
                }
                break;
            case Constants.VillageSceneName:
                IsUIEffectEnd = false;
                StartCoroutine(UIEffect.CutIn(cutEffectImage, UIEffectEndListener));
                break;
            case Constants.QuestHouseSceneName:
                //영준 수정
                //IsUIEffectEnd = false;
                //StartCoroutine(UIEffect.CutOut(cutEffectImage, UIEffectEndListener));
                break;
        }
    }

    public void LoadNextScene(string curSceneName, string nextSceneName)
    {
        Button[] buttons = FindObjectsOfType<Button>();
        foreach (var button in buttons)
        {
            button.interactable = false;
        }

        switch (curSceneName)
        {
            case Constants.StartSceneName:
                IsUIEffectEnd = false;
                StartCoroutine(UIEffect.CutOut(cutEffectImage, UIEffectEndListener));
                break;
            case Constants.WorldMapSceneName:
                IsUIEffectEnd = false;
                StartCoroutine(UIEffect.CutOut(cutEffectImage, UIEffectEndListener));
                break;
            case Constants.FightSceneName:
                IsUIEffectEnd = false;
                SetSlideImagePos(true);
                StartCoroutine(UIEffect.SlideDownIn(slideEffectImage, UIEffectEndListener));
                //StartCoroutine(UIEffect.AlphaIn(fadeEffectImage, UIEffectEndListener));
                break;
            case Constants.UpgradeSceneName:
                IsUIEffectEnd = false;
                SetSlideImagePos(true);
                StartCoroutine(UIEffect.SlideDownIn(slideEffectImage, UIEffectEndListener));
                //StartCoroutine(UIEffect.AlphaIn(fadeEffectImage, UIEffectEndListener));
                break;
            case Constants.VillageSceneName:
                IsUIEffectEnd = false;
                StartCoroutine(UIEffect.CutOut(cutEffectImage, UIEffectEndListener));
                break;
            case Constants.QuestHouseSceneName:
                //영준 수정
                //IsUIEffectEnd = false;
                //StartCoroutine(UIEffect.CutOut(cutEffectImage, UIEffectEndListener));
                break;
        }

        //던전 탈출의 경우
        if(curSceneName == Constants.FightSceneName && nextSceneName == Constants.WorldMapSceneName)
        {
            cutEffectImage.gameObject.SetActive(false);
            cutEffectImage.material.SetFloat("_Radius", 0);
        }
        StartCoroutine(LoadScene(nextSceneName));
    }


    public void UIEffectEndListener()
    {
        IsUIEffectEnd = true;
    }

    public void SetSlideImagePos(string sceneName)
    {
        if (sceneName == Constants.FightSceneName || sceneName == Constants.UpgradeSceneName)
        {
            RectTransform rt = slideEffectImage.rectTransform;
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, 0);
        }
        else
        {
            RectTransform rt = slideEffectImage.rectTransform;
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, rt.rect.height);
        }
    }

    public void SetSlideImagePos(bool up)
    {
        if(up)
        {
            Vector2 endPos = slideEffectImage.rectTransform.anchoredPosition;
            endPos.y = 0;
            endPos.y += slideEffectImage.rectTransform.rect.height;
            slideEffectImage.rectTransform.anchoredPosition = endPos;
        }
        else
        {
            Vector2 endPos = slideEffectImage.rectTransform.anchoredPosition;
            endPos.y = 0;
            endPos.y -= slideEffectImage.rectTransform.rect.height;
            slideEffectImage.rectTransform.anchoredPosition = endPos;
        }
    }

    private IEnumerator LoadScene(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        float timer = 0.0f;
        while (!operation.isDone)
        {
            yield return new WaitForEndOfFrame();

            if(operation.progress >= 0.9f && IsUIEffectEnd && timer > 1f)
            {
                if(sceneName == Constants.UpgradeSceneName || sceneName == Constants.StartSceneName)
                {
                    // objectpool 샷다내림
                    ObjectPoolManager.Instance.SetActvieFalseAllPrefabs();
                }
                SetSlideImagePos(sceneName);
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

        SceneStart(sceneName);
    }

    private void OnDestroy()
    {
        cutEffectImage.material.SetFloat("_Radius", cutEffectImage.material.GetFloat("_DefaultRadius"));
    }
}
