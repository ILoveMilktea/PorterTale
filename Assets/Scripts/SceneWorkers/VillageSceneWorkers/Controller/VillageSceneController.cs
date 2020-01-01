using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//VillageScene내의 상호작용하는 건축물들
static class VillageStructure
{
    public const string QuestHouse = "QuestHouse";
    public const string WorldMapEntrance = "WorldMapEntrance";
}

public class VillageSceneController : MonoSingleton<VillageSceneController>
{    
    public Transform playerTransform;
    private Vector3 prevPlayerPosition;

    //QuestHouse
    public Button questHouseEnterButton;
    public GameObject questHouseNameText;
    //QuestHouse위치
    public Transform questHouseTransform;

    //WorldMapEntrance
    public Button worldMapEntranceButton;
    public GameObject worldMapEntranceText;
   

    private bool isFirstVillageVisit = true;

    // Start is called before the first frame update
    void Start()
    {
        questHouseEnterButton.onClick.AddListener(OnClickQuestHouseEnterButton);
        worldMapEntranceButton.onClick.AddListener(OnClickWorldMapEntranceEnterButton);
        
    }

    private void OnEnable()
    {
        if(isFirstVillageVisit==true)
        {
            isFirstVillageVisit = false;
        }
        else
        {
            playerTransform.position = prevPlayerPosition;
        }
        
    }   

    private void OnClickQuestHouseEnterButton()
    {
        prevPlayerPosition = new Vector3(questHouseTransform.position.x,playerTransform.position.y,playerTransform.position.z);
        GameManager.Instance.LoadNextScene(Constants.VillageSceneName, Constants.QuestHouseSceneName);
    }

    private void OnClickWorldMapEntranceEnterButton()
    {
        prevPlayerPosition = new Vector3(questHouseTransform.position.x, playerTransform.position.y, playerTransform.position.z);
        GameManager.Instance.LoadNextScene(Constants.VillageSceneName, Constants.WorldMapSceneName);
    }

    public void ActivateQuestHouseButton()
    {
        questHouseNameText.SetActive(false);
        questHouseEnterButton.gameObject.SetActive(true);
    }

    public void DeactivateQuestHouseButton()
    {
        questHouseEnterButton.gameObject.SetActive(false);
        questHouseNameText.SetActive(true);
    }

    public void ActivateWorldMapEntranceButton()
    {       
        worldMapEntranceText.SetActive(false);
        worldMapEntranceButton.gameObject.SetActive(true);
    }

    public void DeactivateWorldMapEntranceButton()
    {       
        worldMapEntranceButton.gameObject.SetActive(false);
        worldMapEntranceText.SetActive(true);
    }


}
