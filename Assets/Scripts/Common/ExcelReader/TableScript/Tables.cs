using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class DungeonListInfo
{
    public int m_lastStage { get; private set; }
    public float m_gold_timeLimit { get; private set; }
    public float m_silver_timeLimit { get; private set; }
    public float m_bronze_timeLimit { get; private set; }
    public int m_gold_reward { get; private set; }
    public int m_silver_reward { get; private set; }
    public int m_bronze_reward { get; private set; }

    public void SetlastStage(int lastStage) { m_lastStage = lastStage; }
    public void Setgold_timeLimit(float gold_timeLimit) { m_gold_timeLimit = gold_timeLimit; }
    public void Setsilver_timeLimit(float silver_timeLimit) { m_silver_timeLimit = silver_timeLimit; }
    public void Setbronze_timeLimit(float bronze_timeLimit) { m_bronze_timeLimit = bronze_timeLimit; }
    public void Setgold_reward(int gold_reward) { m_gold_reward = gold_reward; }
    public void Setsilver_reward(int silver_reward) { m_silver_reward = silver_reward; }
    public void Setbronze_reward(int bronze_reward) { m_bronze_reward = bronze_reward; }
}

public class DungeonListTable
{
    public DungeonListTable()
    {
        ReadBinaryTable();
    }

    private Dictionary<string, DungeonListInfo> Table = new Dictionary<string, DungeonListInfo>();

    private void ReadBinaryTable()
    {
        TextAsset textAsset = Resources.Load("Tables/PrefabInfo/DungeonList") as TextAsset;
        MemoryStream memoryStream = new MemoryStream(textAsset.bytes);
        BinaryReader binaryReader = new BinaryReader(memoryStream);

        int tupleCount = binaryReader.ReadInt32();

        for( int i = 0; i < tupleCount; i++)
        {
            DungeonListInfo info = new DungeonListInfo();
            string key = binaryReader.ReadString();
            info.SetlastStage(binaryReader.ReadInt32());
            info.Setgold_timeLimit(binaryReader.ReadSingle());
            info.Setsilver_timeLimit(binaryReader.ReadSingle());
            info.Setbronze_timeLimit(binaryReader.ReadSingle());
            info.Setgold_reward(binaryReader.ReadInt32());
            info.Setsilver_reward(binaryReader.ReadInt32());
            info.Setbronze_reward(binaryReader.ReadInt32());

            Table.Add(key, info);
        }
    }

    public Dictionary<string, DungeonListInfo> GetTable()
    {
        return Table;
    }

    public DungeonListInfo GetTuple(string key)
    {
        DungeonListInfo value;

        if (Table.TryGetValue(key, out value))
            return value;

        return null;
    }

}
public class EnemyStatusInfo
{
    public string m_name { get; private set; }
    public int m_hp { get; private set; }
    public int m_atk { get; private set; }
    public int m_dropParts { get; private set; }

    public void Setname(string name) { m_name = name; }
    public void Sethp(int hp) { m_hp = hp; }
    public void Setatk(int atk) { m_atk = atk; }
    public void SetdropParts(int dropParts) { m_dropParts = dropParts; }
}

public class EnemyStatusTable
{
    public EnemyStatusTable()
    {
        ReadBinaryTable();
    }

    private Dictionary<int, EnemyStatusInfo> Table = new Dictionary<int, EnemyStatusInfo>();

    private void ReadBinaryTable()
    {
        TextAsset textAsset = Resources.Load("Tables/PrefabInfo/EnemyStatus") as TextAsset;
        MemoryStream memoryStream = new MemoryStream(textAsset.bytes);
        BinaryReader binaryReader = new BinaryReader(memoryStream);

        int tupleCount = binaryReader.ReadInt32();

        for( int i = 0; i < tupleCount; i++)
        {
            EnemyStatusInfo info = new EnemyStatusInfo();
            int key = binaryReader.ReadInt32();
            info.Setname(binaryReader.ReadString());
            info.Sethp(binaryReader.ReadInt32());
            info.Setatk(binaryReader.ReadInt32());
            info.SetdropParts(binaryReader.ReadInt32());

            Table.Add(key, info);
        }
    }

    public Dictionary<int, EnemyStatusInfo> GetTable()
    {
        return Table;
    }

    public EnemyStatusInfo GetTuple(int key)
    {
        EnemyStatusInfo value;

        if (Table.TryGetValue(key, out value))
            return value;

        return null;
    }

}
public class GunDefaultStatusInfo
{
    public string m_fireMode { get; private set; }
    public string m_skillMode { get; private set; }
    public float m_msBetweenShots { get; private set; }
    public float m_muzzleVelocity { get; private set; }
    public float m_maxRange { get; private set; }
    public int m_damage { get; private set; }
    public float m_knonkBackForce { get; private set; }
    public int m_directionNumber { get; private set; }
    public float m_projectileMaxAngle { get; private set; }

    public void SetfireMode(string fireMode) { m_fireMode = fireMode; }
    public void SetskillMode(string skillMode) { m_skillMode = skillMode; }
    public void SetmsBetweenShots(float msBetweenShots) { m_msBetweenShots = msBetweenShots; }
    public void SetmuzzleVelocity(float muzzleVelocity) { m_muzzleVelocity = muzzleVelocity; }
    public void SetmaxRange(float maxRange) { m_maxRange = maxRange; }
    public void Setdamage(int damage) { m_damage = damage; }
    public void SetknonkBackForce(float knonkBackForce) { m_knonkBackForce = knonkBackForce; }
    public void SetdirectionNumber(int directionNumber) { m_directionNumber = directionNumber; }
    public void SetprojectileMaxAngle(float projectileMaxAngle) { m_projectileMaxAngle = projectileMaxAngle; }
}

public class GunDefaultStatusTable
{
    public GunDefaultStatusTable()
    {
        ReadBinaryTable();
    }

    private Dictionary<string, GunDefaultStatusInfo> Table = new Dictionary<string, GunDefaultStatusInfo>();

    private void ReadBinaryTable()
    {
        TextAsset textAsset = Resources.Load("Tables/PrefabInfo/GunDefaultStatus") as TextAsset;
        MemoryStream memoryStream = new MemoryStream(textAsset.bytes);
        BinaryReader binaryReader = new BinaryReader(memoryStream);

        int tupleCount = binaryReader.ReadInt32();

        for( int i = 0; i < tupleCount; i++)
        {
            GunDefaultStatusInfo info = new GunDefaultStatusInfo();
            string key = binaryReader.ReadString();
            info.SetfireMode(binaryReader.ReadString());
            info.SetskillMode(binaryReader.ReadString());
            info.SetmsBetweenShots(binaryReader.ReadSingle());
            info.SetmuzzleVelocity(binaryReader.ReadSingle());
            info.SetmaxRange(binaryReader.ReadSingle());
            info.Setdamage(binaryReader.ReadInt32());
            info.SetknonkBackForce(binaryReader.ReadSingle());
            info.SetdirectionNumber(binaryReader.ReadInt32());
            info.SetprojectileMaxAngle(binaryReader.ReadSingle());

            Table.Add(key, info);
        }
    }

    public Dictionary<string, GunDefaultStatusInfo> GetTable()
    {
        return Table;
    }

    public GunDefaultStatusInfo GetTuple(string key)
    {
        GunDefaultStatusInfo value;

        if (Table.TryGetValue(key, out value))
            return value;

        return null;
    }

}
public class WallStatusInfo
{
    public string m_name { get; private set; }

    public void Setname(string name) { m_name = name; }
}

public class WallStatusTable
{
    public WallStatusTable()
    {
        ReadBinaryTable();
    }

    private Dictionary<int, WallStatusInfo> Table = new Dictionary<int, WallStatusInfo>();

    private void ReadBinaryTable()
    {
        TextAsset textAsset = Resources.Load("Tables/PrefabInfo/WallStatus") as TextAsset;
        MemoryStream memoryStream = new MemoryStream(textAsset.bytes);
        BinaryReader binaryReader = new BinaryReader(memoryStream);

        int tupleCount = binaryReader.ReadInt32();

        for( int i = 0; i < tupleCount; i++)
        {
            WallStatusInfo info = new WallStatusInfo();
            int key = binaryReader.ReadInt32();
            info.Setname(binaryReader.ReadString());

            Table.Add(key, info);
        }
    }

    public Dictionary<int, WallStatusInfo> GetTable()
    {
        return Table;
    }

    public WallStatusInfo GetTuple(int key)
    {
        WallStatusInfo value;

        if (Table.TryGetValue(key, out value))
            return value;

        return null;
    }

}

public class Tables : MonoSingleton<Tables>
{
    protected override void Init() 
    {
        DontDestroyOnLoad(gameObject);
    }

    public DungeonListTable DungeonList = null;
    public EnemyStatusTable EnemyStatus = null;
    public GunDefaultStatusTable GunDefaultStatus = null;
    public WallStatusTable WallStatus = null;

    private void Start() 
    {
        DungeonList = new DungeonListTable();
        EnemyStatus = new EnemyStatusTable();
        GunDefaultStatus = new GunDefaultStatusTable();
        WallStatus = new WallStatusTable();
    }
}

