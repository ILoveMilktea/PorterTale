using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntranceButtonColliderCheck : MonoBehaviour
{  
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(transform.parent.name==VillageStructure.QuestHouse)
            {
                VillageSceneController.Instance.ActivateQuestHouseButton();
            }
            else if(transform.parent.name==VillageStructure.WorldMapEntrance)
            {
                VillageSceneController.Instance.ActivateWorldMapEntranceButton();
            }
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {            
            if (transform.parent.name == VillageStructure.QuestHouse)
            {
                VillageSceneController.Instance.DeactivateQuestHouseButton();
            }
            else if (transform.parent.name == VillageStructure.WorldMapEntrance)
            {
                VillageSceneController.Instance.DeactivateWorldMapEntranceButton();
            }
        }


    }
}
