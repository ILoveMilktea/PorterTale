using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeadWindow : MonoBehaviour
{
    public Material fightFog;
    public Text gameover;
    public Button backToStart;


    public IEnumerator DeadHandler()
    {
        gameover.color = Color.clear;
        gameover.gameObject.SetActive(true);
        float fogAmount = fightFog.GetFloat("_Slide");

        float timer = 0;
        while (timer < 1.0f)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            gameover.color = Color.Lerp(Color.clear, Color.white, timer);

            fogAmount = Mathf.Lerp(0.2f, 0.5f, timer);
            fightFog.SetFloat("_Slide", fogAmount);
        }
        fightFog.SetFloat("_Slide", 0.5f);

        backToStart.gameObject.SetActive(true);
        backToStart.onClick.AddListener(BackToStartScene);
    }

    private void BackToStartScene()
    {
        GameManager.Instance.LoadNextScene(Constants.FightSceneName, Constants.StartSceneName);
    }
}
