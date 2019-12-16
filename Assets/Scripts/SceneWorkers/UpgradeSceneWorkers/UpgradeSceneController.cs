using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class ConstDecriptions
{
    public const string FightButton = "다음 스테이지로 진행하시겠습니까?";

    public const string Select_RestButton = "휴식을 취해 체력을 비축합니다.\n<size=50>남은 행동 시간을 소모합니다.</size>"; // "Fight"
    public const string Select_EatButton = "잡은 동물을 구워 먹고 체력을 회복합니다.\n<size=50>남은 행동 시간을 소모합니다.</size>";
    public const string Select_Training = "생명의 기운을 흡수해 신체를 강화합니다.\n<size=50>남은 행동 시간을 소모합니다.</size>";
    public const string Select_Search = "탐색?";
    public const string Select_Retry = "재전투?";
}

public class UpgradeSceneController : MonoSingleton<UpgradeSceneController>
{
    // Inactive on awake
    public PlayerStatusWindow statusWindow;
    public SelectAct selectAct;
    public WeaponWindow weaponWindow;
    public PopupYN popupYN;
    public PopupResult popupResult;

    public Button actButton;
    public Button upgradeButton;
    public Button fightButton;

    protected override void Init()
    {
        actButton.onClick.AddListener(OnClickActButton);
        upgradeButton.onClick.AddListener(OnClickUpgradeButton);
        fightButton.onClick.AddListener(OnClickFightButton);

        AwakeAllUIScript();
    }

    void Start()
    {
        StartUpgradeScene();
    }

    public void StartUpgradeScene()
    { 
        // 시작시 할 명령들?
        if (DataManager.Instance.GetPlayInfo.AlreadyAct)
        {
            actButton.interactable = false;
        }

        DataManager.Instance.Save();
    }

    private void AwakeAllUIScript()
    {
        selectAct.gameObject.SetActive(true);
        weaponWindow.gameObject.SetActive(true);
        popupYN.gameObject.SetActive(true);
        popupResult.gameObject.SetActive(true);

        selectAct.gameObject.SetActive(false);
        weaponWindow.gameObject.SetActive(false);
        popupYN.gameObject.SetActive(false);
        popupResult.gameObject.SetActive(false);
    }
    //private void UIGrouping()
    //{
    //    // 전투 중 active한 UI들
    //    fightGroup.SetMember(joystickAttack.gameObject);
    //    fightGroup.SetMember(joystickMove.gameObject);

    //    // 일시정지 중 active한 UI들
    //    pauseGroup.SetMember(pauseImage.gameObject);


    //    SetStateChangeCallback(FightState.Pause, fightGroup.InactiveAllMembers);
    //    SetStateChangeCallback(FightState.Fight, fightGroup.ActiveAllMembers);

    //    SetStateChangeCallback(FightState.Pause, pauseGroup.ActiveAllMembers);
    //    SetStateChangeCallback(FightState.Fight, pauseGroup.InactiveAllMembers);
    //}

    public void RedrawStatusWindow()
    {
        statusWindow.RedrawWindow();
    }
    
    public void OpenPopupYN(string description, Action yesFunc, Action noFunc)
    {
        StartCoroutine(UIEffect.ExpandFrom90(popupYN.gameObject));
        //popupYN.gameObject.SetActive(true);
        popupYN.SetDescription(description);
        popupYN.SetCallback(yesFunc, noFunc);

        //ActiveExitButton(popupYN.gameObject);
    }

    public void OpenPopupResult(Sprite sprite, string description, Action okFunc)
    {
        ClosePopupYN();
        CloseSelectActWindow();
        DataManager.Instance.SetAlreadyAct(true);
        DataManager.Instance.Save();

        popupResult.image.sprite = sprite;
        popupResult.SetDescription(description);
        //popupResult.gameObject.SetActive(true);
        StartCoroutine(UIEffect.ExpandFrom90(popupResult.gameObject));
        popupResult.SetCallback(okFunc);
    }

    public void OnClickActButton()
    {
        //selectAct.gameObject.SetActive(true);
        StartCoroutine(UIEffect.ExpandFrom90(selectAct.gameObject));
    }

    public void OnClickUpgradeButton()
    {
        //weaponWindow.gameObject.SetActive(true);
        StartCoroutine(UIEffect.ExpandFrom90(weaponWindow.gameObject));
    }

    public void OnClickFightButton()
    {
        //OpenPopupYN(ConstDecriptions.FightButton, EndUpgrade, ClosePopupYN);
        EndUpgrade();
    }

    public void CloseSelectActWindow()
    {
        //selectAct.gameObject.SetActive(false);
        StartCoroutine(UIEffect.ContractTo90(selectAct.gameObject));
        actButton.interactable = false;
    }
    public void CloseSelectWeaponWindow()
    {
        //weaponWindow.gameObject.SetActive(false);
        StartCoroutine(UIEffect.ContractTo90(weaponWindow.gameObject));
    }

    public void ClosePopupYN()
    {
        //exitButton.PullUI();
        //popupYN.gameObject.SetActive(false);
        StartCoroutine(UIEffect.ContractTo90(popupYN.gameObject));
    }

    public void ClosePopupResult()
    {
        //popupResult.gameObject.SetActive(false);
        StartCoroutine(UIEffect.ContractTo90(popupResult.gameObject));
    }

    public void EndUpgrade()
    {
        DataManager.Instance.Save();
        //DataManager.Instance.SetAlreadyAct(false);
        DataManager.Instance.SetStage(DataManager.Instance.GetPlayInfo.Stage + 1);

        popupYN.gameObject.SetActive(false);
        GameManager.Instance.LoadNextScene(Constants.UpgradeSceneName, Constants.FightSceneName);
    }

}
