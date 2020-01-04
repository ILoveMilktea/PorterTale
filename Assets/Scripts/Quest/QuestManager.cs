using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoSingleton<QuestManager>
{
    ////Quest 대화 테이블
    //private Dictionary<int, QuestDialogueInfo> questDialogueTable;
    ////Quest 대화 선택지 테이블
    //private Dictionary<int, QuestChoiceDialogueInfo> questChoiceDialougeTable;
    ////Quest 리스트 테이블
    //private Dictionary<int, QuestStateInfo> questStateTable;
    protected override void Init()
    {
        DontDestroyOnLoad(gameObject);
    }

    public Dictionary<int,QuestDialogueInfo> GetQuestDialogue(string questName)
    {

        return QuestDialogueTable.Instance.GetTable(questName + "Dialogue");
    }

    public Dictionary<int, QuestChoiceDialogueInfo> GetQuestChoiceDialogue(string questName)
    {
        return QuestChoiceDialogueTable.Instance.GetTable(questName + "ChoiceDialogue");
    }

    public string GetQuestStateByName(string questName)
    {
        return DataManager.Instance.GetQuestInfo.questList[questName];
    }

    public Dictionary<string,string> GetQuestInfoList()
    {
        return DataManager.Instance.GetQuestInfo.questList;
    }

    public void AcceptQuest(string questName)
    {
        DataManager.Instance.GetQuestInfo.SetQuestState(questName, "quest_ongoing");
        DataManager.Instance.Save();
    }

    public void QuitQuest(string questName)
    {
        DataManager.Instance.GetQuestInfo.SetQuestState(questName, "quest_available");
        DataManager.Instance.Save();
    }

    //Quest달성했을 때 이 함수 호출(그럼 Quest 마친 상태로 바뀜)
    public void FinishQuest(string questName)
    {
        DataManager.Instance.GetQuestInfo.SetQuestState(questName, "quest_done");

    }
}
