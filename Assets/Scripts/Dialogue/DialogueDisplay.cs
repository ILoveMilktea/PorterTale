using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum DialogueState
{    
    ONGOING,
    END
}

public enum SentenceState
{
    ONGOING,
    CHOICE,
    DONE
}


public class DialogueDisplay : MonoBehaviour
{

    public GameObject speakerLeft;
    public GameObject speakerRight;

    public Text speakerLeftNameText;
    public Text speakerRightNameText;

    public Text speakerLeftDialogueText;
    public Text speakerRightDialogueText;

    //Quest 대화 테이블
    private Dictionary<int, QuestDialogueInfo> questDialogueTable;
    //Quest 대화 선택지 테이블
    private Dictionary<int, QuestChoiceDialogueInfo> questChoiceDialogueTable;
    //Quest 진척 상황
    private string questStateInfo;

    public GameObject choiceButtonPanel;
    public Button choiceButtonTemplate;

    //Skip 버튼
    public Button skipButton;
    //Exit 버튼
    public Button exitButton;

    //Dialogue 상태
    private DialogueState dialogueState;
    //Sentence(대사 문장) 상태
    private SentenceState sentenceState;

    //문장내에서 한글자간 출력 시간 간격
    public float timeBetweenDialogueLetter = 0.01f;
    //첫 대화가 나오기까지 시간 딜레이
    public float waitTimeBeforeStartDialogue = 2.0f;
    //나중에 DataManger에서 가져오는걸로 변경
    public string questName;
    private int dialogueIndexCount = 1;

    private Queue<int> choiceDialogueIndexQueue;

    private string questChoiceString;
    private string questDialogueClassification;

    private Coroutine typeSentenceCoroutine=null;

    
    

    // Start is called before the first frame update
    void OnEnable()
    {
        dialogueState = DialogueState.ONGOING;
        sentenceState = SentenceState.DONE;
        //여기서 엑셀 파일 불러오기
        questDialogueTable = QuestManager.Instance.GetQuestDialogue(questName);
        questChoiceDialogueTable = QuestManager.Instance.GetQuestChoiceDialogue(questName);
        //추가-quest 진행 상태 table갖고오기        
        questStateInfo = QuestManager.Instance.GetQuestStateByName(questName);
        

        choiceDialogueIndexQueue = new Queue<int>();
        SetQuestChoiceStateString();
        SettingIndex();

        skipButton.onClick.AddListener(OnClickSkipButton);
        exitButton.onClick.AddListener(OnClickExitButton);        

        StartCoroutine(DisplayDialogue());
    } 

    IEnumerator DisplayDialogue()
    {
        yield return new WaitForSeconds(waitTimeBeforeStartDialogue);
        DisplayNextSentence();
        StartCoroutine(CheckDisplayNextRequest());
    }

    IEnumerator CheckDisplayNextRequest()
    {
        while (true)
        {
            //마우스 입력으로 변경
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("누름");
                DisplayNextSentence();
            }
            ////핸드폰에 대한 터치 입력으로 변경
            //if (Input.GetTouch(0).phase == TouchPhase.Began)
            //{
            //   DisplayNextSentence();
            //}
            yield return new WaitForSeconds(0.02f);

            
        }
    }

    private void DisplayNextSentence()
    {      
        //전체 대화가 다 끝난 상태
        if (dialogueState == DialogueState.END)
        {
            if (sentenceState == SentenceState.ONGOING)
            {
                sentenceState = SentenceState.DONE;
            }
            else if (sentenceState == SentenceState.DONE)
            {
                EndDialogue();
            }
        }
        else
        {
            //한 대사가 진행중인 상태
            if (sentenceState == SentenceState.ONGOING)
            {
                sentenceState = SentenceState.DONE;
            }            
            //한 대사가 끝난 상태
            else if (sentenceState == SentenceState.DONE)
            {
                if (typeSentenceCoroutine != null)
                {
                    StopCoroutine(typeSentenceCoroutine);
                }


                QuestDialogueInfo questDialogueInfo = questDialogueTable[dialogueIndexCount++];

                if (questDialogueInfo.m_portrait_position == "left")
                {
                    ChangeScreen(speakerRight, speakerLeft, speakerLeftNameText, questDialogueInfo);
                }
                else if (questDialogueInfo.m_portrait_position == "right")
                {
                    ChangeScreen(speakerLeft, speakerRight, speakerLeftNameText, questDialogueInfo);
                }

                //가운데 선택창 켜는거(선택지 대화일 경우)
                if (questDialogueInfo.m_destination.Contains(questChoiceString))
                {                    
                    sentenceState = SentenceState.CHOICE;
                    
                    int buttonIndex = 0;
                    foreach (var questChoiceDialogueInfo in questChoiceDialogueTable.Values)
                    {
                        if (questDialogueInfo.m_destination.Contains(questChoiceDialogueInfo.m_choice))
                        {
                            AddChoiceButton(buttonIndex, questChoiceDialogueInfo.m_sentence, questChoiceDialogueInfo.m_destination);
                            buttonIndex++;
                        }

                    }

                    if(choiceDialogueIndexQueue.Count>0)
                    {
                        choiceDialogueIndexQueue.Dequeue();
                    }
                    
                    choiceButtonPanel.SetActive(true);

                }
                else if (questDialogueInfo.m_destination == "end")
                {
                    dialogueState = DialogueState.END;
                }




            }


        }

    }    

    private void EndDialogue()
    {
        GameManager.Instance.LoadNextScene(Constants.QuestHouseSceneName, Constants.VillageSceneName);
    }

    //대사와 초상화 전환
    private void ChangeScreen(GameObject toActiveFalse, GameObject toActiveTrue, Text nameText, QuestDialogueInfo questDialogueInfo)
    {
        toActiveFalse.SetActive(false);
        toActiveTrue.SetActive(true);
        nameText.text = questDialogueInfo.m_speaker;

        //한 글자씩 대사 출력
        
         typeSentenceCoroutine=StartCoroutine(TypeSentence(questDialogueInfo.m_sentence, questDialogueInfo.m_portrait_position));
        
        
    }

    IEnumerator TypeSentence(string sentence, string position)
    {
        sentenceState = SentenceState.ONGOING;

        Text speakerDialogueText=null;        

        if (position=="left")
        {
            speakerDialogueText= speakerLeftDialogueText;
        }
        else if(position=="right")
        {
            speakerDialogueText = speakerRightDialogueText;
        }
        speakerDialogueText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            if (sentenceState == SentenceState.DONE)
            {
                speakerDialogueText.text = sentence;
                break;
            }
            speakerDialogueText.text += letter;
            yield return new WaitForSeconds(timeBetweenDialogueLetter);
        }

        if(sentenceState!=SentenceState.CHOICE)
        {
            sentenceState = SentenceState.DONE;
        }
        
    }

    private void AddChoiceButton(int index,string choiceDialogue, string destination)
    {
        int buttonSpacing = -44;

        Button button = Instantiate(choiceButtonTemplate);
        button.GetComponentInChildren<Text>().text = choiceDialogue;        
        button.onClick.AddListener(OnClickChoiceButton);
        button.transform.parent=choiceButtonPanel.transform;
        button.transform.localScale = Vector3.one;
        button.transform.localPosition = new Vector3(0, index * buttonSpacing, 0);
        button.name = destination;
        button.gameObject.SetActive(true);
    }

   

    private void SaveQuestData(KeyValuePair<int,QuestDialogueInfo> questDialogue)
    {
        if(questStateInfo=="quest_available")
        {
            if (questDialogue.Value.m_dialogue_classification.Contains("yes"))
            {
                Debug.Log("quest받기");
                QuestManager.Instance.AcceptQuest(questName);
                
                Debug.Log("저장된상태"+QuestManager.Instance.GetQuestStateByName(questName));
            }            
        }
        else if(questStateInfo=="quest_ongoing")
        {
            if (questDialogue.Value.m_dialogue_classification.Contains("no"))
            {

                QuestManager.Instance.QuitQuest(questName);
                
                Debug.Log("저장된상태" + QuestManager.Instance.GetQuestStateByName(questName));
            }
        }       
    }   

    private void SettingIndex()
    {
        foreach (var questDialogue in questDialogueTable)
        {
            //선택 대화 인덱스 Queue에 넣기
            if (questDialogue.Value.m_destination.Contains(questChoiceString))
            {
                choiceDialogueIndexQueue.Enqueue(questDialogue.Key);
            }
            //퀘스트 상태에 따라 대화 어디서부터 시작할지 결정
            if(questDialogue.Value.m_dialogue_classification==questStateInfo)                         
            {
                dialogueIndexCount = questDialogue.Key;
            }
        }
    }   

    private void SetQuestChoiceStateString()
    {
        Debug.Log("퀘스트 상태" + questStateInfo);
        questChoiceString = questStateInfo + "_" + "choice";

    }

    private void OnClickChoiceButton()
    {
        GameObject currentSelectedgameObject = EventSystem.current.currentSelectedGameObject;

        foreach (var questDialogue in questDialogueTable)
        {

            if (questDialogue.Value.m_dialogue_classification == currentSelectedgameObject.name)
            {
                SaveQuestData(questDialogue);
                dialogueIndexCount = questDialogue.Key;
                break;
            }
        }
        sentenceState = SentenceState.DONE;
        DisplayNextSentence();

        choiceButtonPanel.SetActive(false);
    }

    private void OnClickSkipButton()
    {
        if(sentenceState!=SentenceState.CHOICE)
        {
            sentenceState = SentenceState.DONE;
            if (choiceDialogueIndexQueue.Count > 0)
            {
                dialogueIndexCount = choiceDialogueIndexQueue.Dequeue();
                DisplayNextSentence();
            }
        }
          
    }

    private void OnClickExitButton()
    {
        EndDialogue();
    }
   
   
}
