using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class QuestDialogueInfo
{
    public string m_dialogue_classification { get; private set; }
    public string m_speaker { get; private set; }
    public string m_sentence { get; private set; }
    public string m_destination { get; private set; }
    public string m_portrait_position { get; private set; }

    public void Setdialogue_classification(string dialogue_classification) { m_dialogue_classification = dialogue_classification; }
    public void Setspeaker(string speaker) { m_speaker = speaker; }
    public void Setsentence(string sentence) { m_sentence = sentence; }
    public void Setdestination(string destination) { m_destination = destination; }
    public void Setportrait_position(string portrait_position) { m_portrait_position = portrait_position; }
}

public class QuestDialogueTable : MonoSingleton<QuestDialogueTable>
{
    private Dictionary<string, Dictionary<int, QuestDialogueInfo>> Tables = new Dictionary<string, Dictionary<int, QuestDialogueInfo>>();

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
        string[] resourceNames = new string[] {"GriffonQuestDialogue"};
        foreach(var name in resourceNames)
        {
            TextAsset textAsset = Resources.Load("Tables/QuestDialogue/" + name) as TextAsset;
            MemoryStream memoryStream = new MemoryStream(textAsset.bytes);
            BinaryReader binaryReader = new BinaryReader(memoryStream);
            Dictionary<int, QuestDialogueInfo> table = new Dictionary<int, QuestDialogueInfo>();

            int tupleCount = binaryReader.ReadInt32();

            for( int i = 0; i < tupleCount; i++)
            {
                QuestDialogueInfo info = new QuestDialogueInfo();
                int key = binaryReader.ReadInt32();
                info.Setdialogue_classification(binaryReader.ReadString());
                info.Setspeaker(binaryReader.ReadString());
                info.Setsentence(binaryReader.ReadString());
                info.Setdestination(binaryReader.ReadString());
                info.Setportrait_position(binaryReader.ReadString());

                table.Add(key, info);
            }
            Tables.Add(name, table);
        }
    }

    public Dictionary<int, QuestDialogueInfo> GetTable(string sheetName)
    {
        return Tables[sheetName];
    }

    public QuestDialogueInfo GetTuple(string sheetName, int key)
    {
        QuestDialogueInfo value;

        if (Tables[sheetName].TryGetValue(key, out value))
            return value;

        return null;
    }

}

