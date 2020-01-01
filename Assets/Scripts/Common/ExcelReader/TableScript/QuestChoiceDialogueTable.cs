using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class QuestChoiceDialogueInfo
{
    public string m_choice { get; private set; }
    public string m_speaker { get; private set; }
    public string m_sentence { get; private set; }
    public string m_destination { get; private set; }

    public void Setchoice(string choice) { m_choice = choice; }
    public void Setspeaker(string speaker) { m_speaker = speaker; }
    public void Setsentence(string sentence) { m_sentence = sentence; }
    public void Setdestination(string destination) { m_destination = destination; }
}

public class QuestChoiceDialogueTable : MonoSingleton<QuestChoiceDialogueTable>
{
    private Dictionary<string, Dictionary<int, QuestChoiceDialogueInfo>> Tables = new Dictionary<string, Dictionary<int, QuestChoiceDialogueInfo>>();

    protected override void Init() 
    {
        DontDestroyOnLoad(gameObject);
    }


    private void Start() 
    {
        ReadBinaryTable();
    }
    private void ReadBinaryTable()
    {
        string[] resourceNames = new string[] {"GriffonQuestChoiceDialogue"};
        foreach(var name in resourceNames)
        {
            TextAsset textAsset = Resources.Load("Tables/QuestChoiceDialogue/" + name) as TextAsset;
            MemoryStream memoryStream = new MemoryStream(textAsset.bytes);
            BinaryReader binaryReader = new BinaryReader(memoryStream);
            Dictionary<int, QuestChoiceDialogueInfo> table = new Dictionary<int, QuestChoiceDialogueInfo>();

            int tupleCount = binaryReader.ReadInt32();

            for( int i = 0; i < tupleCount; i++)
            {
                QuestChoiceDialogueInfo info = new QuestChoiceDialogueInfo();
                int key = binaryReader.ReadInt32();
                info.Setchoice(binaryReader.ReadString());
                info.Setspeaker(binaryReader.ReadString());
                info.Setsentence(binaryReader.ReadString());
                info.Setdestination(binaryReader.ReadString());

                table.Add(key, info);
            }
            Tables.Add(name, table);
        }
    }

    public Dictionary<int, QuestChoiceDialogueInfo> GetTable(string sheetName)
    {
        return Tables[sheetName];
    }

    public QuestChoiceDialogueInfo GetTuple(string sheetName, int key)
    {
        QuestChoiceDialogueInfo value;

        if (Tables[sheetName].TryGetValue(key, out value))
            return value;

        return null;
    }

}

