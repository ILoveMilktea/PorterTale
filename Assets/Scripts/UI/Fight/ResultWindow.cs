using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;

static class Const_AreaName
{
    public const string Dungeon_Griffon = "Griffon";
}

public class ResultWindow : MonoBehaviour
{
    private Action callback;


    public Image clearImage;
    public Sprite defaultRankSprite;

    public Image achievementWindow;
    public Image myRank;
    public Text myTime;

    public ClearRank gold;
    public ClearRank silver;
    public ClearRank bronze;

    public Button escapeDungeon;
    public Button returnButton;

    public PopupYN warningPopup;

    private PlayInfo playInfo;
    private DungeonListInfo dungeonInfo;
    private void Awake()
    {
        escapeDungeon.onClick.AddListener(OnClickEscapeButton);
        returnButton.onClick.AddListener(OnClickReturnButton);
    }

    public void OpenResultWindow()
    {
        playInfo = DataManager.Instance.GetPlayInfo;
        dungeonInfo = Tables.Instance.DungeonList.GetTuple(playInfo.CurDungeon);

        gold.SetTimeLimit(dungeonInfo.m_gold_timeLimit.ToString("00:00"));
        gold.SetReward(dungeonInfo.m_gold_reward.ToString());

        silver.SetTimeLimit(dungeonInfo.m_silver_timeLimit.ToString("00:00"));
        silver.SetReward(dungeonInfo.m_silver_reward.ToString());

        bronze.SetTimeLimit(dungeonInfo.m_bronze_timeLimit.ToString("00:00"));
        bronze.SetReward(dungeonInfo.m_bronze_reward.ToString());
        // 나중에 돈도 얻자
        if (playInfo.Playtime <= dungeonInfo.m_gold_timeLimit)
        {
            myRank.sprite = gold.medal.sprite;
            myRank.color = gold.medal.color;
        }
        else if (playInfo.Playtime <= dungeonInfo.m_silver_timeLimit)
        {
            myRank.sprite = silver.medal.sprite;
            myRank.color = silver.medal.color;
        }
        else if (playInfo.Playtime <= dungeonInfo.m_bronze_timeLimit)
        {
            myRank.sprite = bronze.medal.sprite;
            myRank.color = bronze.medal.color;
        }
        else
        {
            myRank.sprite = defaultRankSprite;
        }

        myTime.text = "00:00";


        StartCoroutine(ShowResultUIEffect());
    }
    private IEnumerator ShowResultUIEffect()
    {
        // 1. clear image & achivement window
        StartCoroutine(UIEffect.SlideHorizontalRight(clearImage.rectTransform, 0.5f));
        StartCoroutine(UIEffect.SlideHorizontalLeft(achievementWindow.rectTransform, 0.5f));
        yield return new WaitForSeconds(0.8f);
        // 2. g,s,b
        StartCoroutine(UIEffect.SlideHorizontalLeft(gold.GetComponent<RectTransform>(), 0.5f));
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(UIEffect.SlideHorizontalLeft(silver.GetComponent<RectTransform>(), 0.5f));
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(UIEffect.SlideHorizontalLeft(bronze.GetComponent<RectTransform>(), 0.5f));
        yield return new WaitForSeconds(0.7f);
        // 3. my rank
        myTime.gameObject.SetActive(true);
        StartCoroutine(TimeFlow(myTime, 1.0f));
        yield return new WaitForSeconds(1.0f);
        myRank.gameObject.SetActive(true);
        StartCoroutine(UIEffect.Expand(myRank.gameObject, 0.3f, 0.2f));
        // 4. button on
        escapeDungeon.gameObject.SetActive(true);
        returnButton.gameObject.SetActive(true);
    }
    private IEnumerator TimeFlow(Text displayTime, float effectTime)
    {
        float lerpTime = 0f;

        float timer = 0f;
        while (timer < 1.0f)
        {
            lerpTime = Mathf.Lerp(0f, playInfo.Playtime, timer);
            displayTime.text = lerpTime.ToString("00:00");
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime / effectTime;
        }

        displayTime.text = playInfo.Playtime.ToString("00:00");
    }

    public void OnClickEscapeButton()
    {
        FightSceneController.Instance.EscapeDungeon();
    }
    public void OnClickReturnButton()
    {
        StartCoroutine(UIEffect.ExpandFrom90(warningPopup.gameObject));
        warningPopup.SetDescription("준비한게 여기까지인데..\n<size=50>더 진행하시겠습니까?</size>");
        warningPopup.SetCallback(Warning_Yes, Warning_No);
    }

    public void Warning_Yes()
    {
        StartCoroutine(UIEffect.ContractTo90(warningPopup.gameObject));
        StartCoroutine(UIEffect.ContractTo90(gameObject));
        FightSceneController.Instance.ChangeFightState(FightState.Clear_Stage);
    }
    public void Warning_No()
    {
        StartCoroutine(UIEffect.ContractTo90(warningPopup.gameObject));
    }

}
