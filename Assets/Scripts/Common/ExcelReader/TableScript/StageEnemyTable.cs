using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class StageEnemyInfo
{
    public int m_serialNumber { get; private set; }
    public int m_level { get; private set; }
    public float m_posX { get; private set; }
    public float m_posY { get; private set; }

    public void SetserialNumber(int serialNumber) { m_serialNumber = serialNumber; }
    public void Setlevel(int level) { m_level = level; }
    public void SetposX(float posX) { m_posX = posX; }
    public void SetposY(float posY) { m_posY = posY; }
}

public class StageEnemyTable : MonoSingleton<StageEnemyTable>
{
    private Dictionary<string, Dictionary<int, StageEnemyInfo>> Tables = new Dictionary<string, Dictionary<int, StageEnemyInfo>>();

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
            TextAsset textAsset = Resources.Load("Tables/StageEnemy/" + name) as TextAsset;
            MemoryStream memoryStream = new MemoryStream(textAsset.bytes);
            BinaryReader binaryReader = new BinaryReader(memoryStream);
            Dictionary<int, StageEnemyInfo> table = new Dictionary<int, StageEnemyInfo>();

            int tupleCount = binaryReader.ReadInt32();

            for( int i = 0; i < tupleCount; i++)
            {
                StageEnemyInfo info = new StageEnemyInfo();
                int key = binaryReader.ReadInt32();
                info.SetserialNumber(binaryReader.ReadInt32());
                info.Setlevel(binaryReader.ReadInt32());
                info.SetposX(binaryReader.ReadSingle());
                info.SetposY(binaryReader.ReadSingle());

                table.Add(key, info);
            }
            Tables.Add(name, table);
        }
    }

    public Dictionary<int, StageEnemyInfo> GetTable(string sheetName)
    {
        return Tables[sheetName];
    }

    public StageEnemyInfo GetTuple(string sheetName, int key)
    {
        StageEnemyInfo value;

        if (Tables[sheetName].TryGetValue(key, out value))
            return value;

        return null;
    }

}

