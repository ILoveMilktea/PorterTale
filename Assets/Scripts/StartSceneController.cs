using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartSceneController : MonoSingleton<StartSceneController>
{
    public Button newButton;
    public Button continueButton;

    protected override void Init()
    {
        newButton.onClick.AddListener(OnClickNewButton);
        continueButton.onClick.AddListener(OnClickContinueButton);
    }

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.SceneStart(Constants.StartSceneName);
        if(!DataManager.Instance.CheckSaveData())
        {
            continueButton.interactable = false;
        }
    }

    public void OnClickNewButton()
    {
        DataManager.Instance.RemoveSaveData();
        GameManager.Instance.LoadNextScene(Constants.StartSceneName, Constants.WorldMapSceneName);
    }

    public void OnClickContinueButton()
    {
        DataManager.Instance.Load();
        if(DataManager.Instance.GetPlayInfo.CurDungeon == "")
        {
            GameManager.Instance.LoadNextScene(Constants.StartSceneName, Constants.WorldMapSceneName);
        }
        else
        {
            GameManager.Instance.LoadNextScene(Constants.StartSceneName, Constants.UpgradeSceneName);
        }
        
    }
}
