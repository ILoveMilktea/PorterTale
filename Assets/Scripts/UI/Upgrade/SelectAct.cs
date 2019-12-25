using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectAct: MonoBehaviour
{
    public Button rest;
    public Button eat;
    public Button training;
    public Button search;
    public Button retry;

    public Button exit;

    private void Awake()
    {
        rest.onClick.AddListener(OnClickRestButton);
        eat.onClick.AddListener(OnClickEatButton);
        training.onClick.AddListener(OnClickTrainingButton);
        search.onClick.AddListener(OnClickSearchButton);
        retry.onClick.AddListener(OnClickRetryButton);

        exit.onClick.AddListener(OnClickExitButton);
    }

    public void OnClickRestButton()
    {
        UpgradeSceneController.Instance.OpenPopupYN(
            ConstDecriptions.Select_RestButton,
            ActRest,
            UpgradeSceneController.Instance.ClosePopupYN
            );
    }
    public void OnClickEatButton()
    {
        UpgradeSceneController.Instance.OpenPopupYN(
            ConstDecriptions.Select_EatButton,
            ActEat,
            UpgradeSceneController.Instance.ClosePopupYN
            );
    }
    public void OnClickTrainingButton()
    {
        UpgradeSceneController.Instance.OpenPopupYN(
            ConstDecriptions.Select_Training,
            ActTraining,
            UpgradeSceneController.Instance.ClosePopupYN
            );
    }
    public void OnClickSearchButton()
    {
        UpgradeSceneController.Instance.OpenPopupYN(
            ConstDecriptions.Select_Search,
            ActSearch,
            UpgradeSceneController.Instance.ClosePopupYN
            );
    }
    public void OnClickRetryButton()
    {
        UpgradeSceneController.Instance.OpenPopupYN(
            ConstDecriptions.Select_Retry,
            ActRetry,
            UpgradeSceneController.Instance.ClosePopupYN
            );
    }

    public void OnClickExitButton()
    {
        gameObject.SetActive(false);
    }
    private void ActRest()
    {
        // 휴식, 체력증가 소, 체력회복 대
        DataManager.Instance.AddBuffHp(10);
        DataManager.Instance.AddRemainHp(10);

        UpgradeSceneController.Instance.OpenPopupResult(rest.image.sprite, "피로가..회복...zzZ", UpgradeSceneController.Instance.ClosePopupResult);
    }

    private void ActEat()
    {
        // 식사, 체력증가 소, 체력회복 중, atk증가 소
        //DataManager.Instance.AddBuffHp(10);
        DataManager.Instance.AddRemainHp(30);
        
        UpgradeSceneController.Instance.OpenPopupResult(eat.image.sprite, "생기가 돈다!", UpgradeSceneController.Instance.ClosePopupResult);
    }

    private void ActTraining()
    {
        // 단련, 체력증가 중, atk증가 소 || 체력증가 소, atk증가 중
        DataManager.Instance.AddBuffHp(5);
        DataManager.Instance.AddBuffAtk(2);
        
        UpgradeSceneController.Instance.OpenPopupResult(training.image.sprite, "마음이 고양된다.", UpgradeSceneController.Instance.ClosePopupResult);
    }

    private void ActSearch()
    {
        UpgradeSceneController.Instance.OpenPopupResult(search.image.sprite, "search", UpgradeSceneController.Instance.ClosePopupResult);
    }

    private void ActRetry()
    {
        UpgradeSceneController.Instance.OpenPopupResult(retry.image.sprite, "retry", UpgradeSceneController.Instance.ClosePopupResult);
    }
}
