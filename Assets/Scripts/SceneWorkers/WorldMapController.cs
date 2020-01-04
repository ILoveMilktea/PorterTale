using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldMapController : MonoSingleton<WorldMapController>
{
    public Button Dungeon_Griffon;
    public Button StartVillage;

    public PopupYN popupYN;

    protected override void Init()
    {
        Dungeon_Griffon.onClick.AddListener(OnClickDungeon_Griffon);
        StartVillage.onClick.AddListener(OnClickStartVillage);

        AwakeAllUIScript();
    }

    private void AwakeAllUIScript()
    {
        popupYN.gameObject.SetActive(true);
        
        popupYN.gameObject.SetActive(false);
    }

    public void OnClickDungeon_Griffon()
    {
        OpenPopupYN("그리폰 던전으로\n들어가시겠습니까?",
            EnterDungeon_Griffon, ClosePopupYN);
    }

    public void OnClickStartVillage()
    {
        OpenPopupYN("시작마을로\n돌아가시겠습니까?",
            EnterStartVillage, ClosePopupYN);
    }

    public void EnterDungeon_Griffon()
    {
        ClosePopupYN();
        DataManager.Instance.SetDungeonName(Const_AreaName.Dungeon_Griffon);
        GameManager.Instance.LoadNextScene(Constants.WorldMapSceneName, Constants.FightSceneName);
    }
    public void EnterStartVillage()
    {
        ClosePopupYN();
        GameManager.Instance.LoadNextScene(Constants.WorldMapSceneName, Constants.VillageSceneName);
    }


    public void OpenPopupYN(string description, Action yesFunc, Action noFunc)
    {
        //popupYN.gameObject.SetActive(true);
        StartCoroutine(UIEffect.ExpandFrom90(popupYN.gameObject));
        popupYN.SetDescription(description);
        popupYN.SetCallback(yesFunc, noFunc);
    }

    public void ClosePopupYN()
    {
        StartCoroutine(UIEffect.ContractTo90(popupYN.gameObject));
    }
}
