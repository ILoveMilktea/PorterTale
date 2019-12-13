using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class StageEntranceInfo
{
    public string m_entrancePos { get; private set; }
    public string m_prefabName { get; private set; }

    public void SetentrancePos(string entrancePos) { m_entrancePos = entrancePos; }
    public void SetprefabName(string prefabName) { m_prefabName = prefabName; }
}

public class StageEntranceTable : MonoSingleton<StageEntranceTable>
{
    private Dictionary<string, Dictionary<string, StageEntranceInfo>> Tables = new Dictionary<string, Dictionary<string, StageEntranceInfo>>();

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
        string[] resourceNames = new string[] {"Stage1", "Stage2", "Stage3", "Stage4", "Stage5", "Stage6"};
        foreach(var name in resourceNames)
        {
            TextAsset textAsset = Resources.Load("Tables/StageEntrance/" + name) as TextAsset;
            MemoryStream memoryStream = new MemoryStream(textAsset.bytes);
            BinaryReader binaryReader = new BinaryReader(memoryStream);
            Dictionary<string, StageEntranceInfo> table = new Dictionary<string, StageEntranceInfo>();

            int tupleCount = binaryReader.ReadInt32();

            for( int i = 0; i < tupleCount; i++)
            {
                StageEntranceInfo info = new StageEntranceInfo();
                string key = binaryReader.ReadString();
                info.SetentrancePos(binaryReader.ReadString());
                info.SetprefabName(binaryReader.ReadString());

                table.Add(key, info);
            }
            Tables.Add(name, table);
        }
    }

    public Dictionary<string, StageEntranceInfo> GetTable(string sheetName)
    {
        return Tables[sheetName];
    }

    public StageEntranceInfo GetTuple(string sheetName, string key)
    {
        StageEntranceInfo value;

        if (Tables[sheetName].TryGetValue(key, out value))
            return value;

        return null;
    }

}

