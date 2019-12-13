using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSkillInfo
{
    public string m_prevPath { get; private set; }
    public int m_needParts { get; private set; }
    public string m_skillName { get; private set; }
    public string m_description { get; private set; }
    public float m_value { get; private set; }
    public string m_spriteName { get; private set; }

    public void SetprevPath(string prevPath) { m_prevPath = prevPath; }
    public void SetneedParts(int needParts) { m_needParts = needParts; }
    public void SetskillName(string skillName) { m_skillName = skillName; }
    public void Setdescription(string description) { m_description = description; }
    public void Setvalue(float value) { m_value = value; }
    public void SetspriteName(string spriteName) { m_spriteName = spriteName; }
}

public class WeaponSkillTable : MonoSingleton<WeaponSkillTable>
{
    private Dictionary<string, Dictionary<string, WeaponSkillInfo>> Tables = new Dictionary<string, Dictionary<string, WeaponSkillInfo>>();

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
        string[] resourceNames = new string[] {"EnergySphereGun", "LinearGun", "ShotGun"};
        foreach(var name in resourceNames)
        {
            TextAsset textAsset = Resources.Load("Tables/WeaponSkill/" + name) as TextAsset;
            MemoryStream memoryStream = new MemoryStream(textAsset.bytes);
            BinaryReader binaryReader = new BinaryReader(memoryStream);
            Dictionary<string, WeaponSkillInfo> table = new Dictionary<string, WeaponSkillInfo>();

            int tupleCount = binaryReader.ReadInt32();

            for( int i = 0; i < tupleCount; i++)
            {
                WeaponSkillInfo info = new WeaponSkillInfo();
                string key = binaryReader.ReadString();
                info.SetprevPath(binaryReader.ReadString());
                info.SetneedParts(binaryReader.ReadInt32());
                info.SetskillName(binaryReader.ReadString());
                info.Setdescription(binaryReader.ReadString());
                info.Setvalue(binaryReader.ReadSingle());
                info.SetspriteName(binaryReader.ReadString());

                table.Add(key, info);
            }
            Tables.Add(name, table);
        }
    }

    public Dictionary<string, WeaponSkillInfo> GetTable(string sheetName)
    {
        return Tables[sheetName];
    }

    public WeaponSkillInfo GetTuple(string sheetName, string key)
    {
        WeaponSkillInfo value;

        if (Tables[sheetName].TryGetValue(key, out value))
            return value;

        return null;
    }

}

