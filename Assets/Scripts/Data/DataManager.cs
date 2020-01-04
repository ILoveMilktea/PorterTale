using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

static class Const_Path
{
    public const string playInfoPath = "/PlayInfo.byte";
    public const string playerStatusInfoPath = "/PlayerStatusInfo.byte";
    public const string WeaponInfoPath = "/WeaponInfo.byte";
    public const string QuestInfoPath = "/QuestInfo.byte";
    public const string InventoryInfoPath = "/InvetoryInfo.byte";
}

public partial class DataManager : MonoSingleton<DataManager>
{
    private string dataPath;
    private int maxExp = 100; // 임시 설정
    private float playtime;

    private DataCenter dataCenter;
    private DataSave dataSave;
    private DataLoad dataLoad;

    protected override void Init()
    {
        DontDestroyOnLoad(gameObject);

#if UNITY_IPHONE
        dataPath = Application.dataPath + "/Raw";
#endif

#if UNITY_ANDROID
        //dataPath = "jar:file://" + Application.dataPath + "!/assets";
        dataPath = Application.persistentDataPath + "/SaveData";
#endif

#if UNITY_STANDALONE || UNITY_EDITOR
        dataPath = Application.dataPath + "/SaveData";
#endif


        if (!Directory.Exists(dataPath))
        {
            Directory.CreateDirectory(dataPath);
        }

        //dataPath += "/SaveData.json";
    }

    private void Start()
    {
        dataCenter = new DataCenter();
        dataSave = new DataSave();
        dataLoad = new DataLoad();
    }

    public bool CheckSaveData()
    {
        if (File.Exists(dataPath + Const_Path.playInfoPath))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void RemoveSaveData()
    {
        File.Delete(dataPath + Const_Path.playInfoPath);
        File.Delete(dataPath + Const_Path.playerStatusInfoPath);
        File.Delete(dataPath + Const_Path.WeaponInfoPath);
        //영준 추가
        File.Delete(dataPath + Const_Path.QuestInfoPath);
    }

    public void Save()
    {
        dataSave.SaveUserData(dataPath);

        if (SceneManager.GetActiveScene().name == "Upgrade")
        {
            UpgradeSceneController.Instance.RedrawStatusWindow();
        }
    }

    public void Load()
    {
        dataLoad.LoadUserData(dataPath);
    }

    public void ResetDungeonData()
    {
        GetPlayInfo.ResetDungeonPlay();
        GetPlayerStatus.ResetDungeonPlayStatus();
    }
}

// PlayInfo 부분
public partial class DataManager : MonoSingleton<DataManager>
{
    public PlayInfo GetPlayInfo { get { return dataCenter.playInfo; } }

    public void SetPlayInfo(PlayInfo info)
    {
        //SetDungeonName(info.CurDungeon);
        //SetPlayTime(info.Playtime);
        //SetStage(info.Stage);
        //SetParts(info.Parts);
        //SetAlreadyAct(info.AlreadyAct);
        dataCenter.SetPlayInfo(info);
    }
    public void SetDungeonName(string curDungeon)
    {
        GetPlayInfo.SetCurDungeon(curDungeon);
    }
    public void SetPlayTime(float playtime)
    {
        GetPlayInfo.SetPlaytime(playtime);
    }
    public void AddPlayTime(float playtime)
    {
        SetPlayTime(GetPlayInfo.Playtime + playtime);
    }
    public void SetStage(int stage)
    {
        GetPlayInfo.SetStage(stage);
    }
    public void AddStage(int value)
    {
        SetStage(GetPlayInfo.Stage + value);
    }
    public void SetParts(int parts)
    {
        GetPlayInfo.SetParts(parts);
    }
    public void AddParts(int parts)
    {
        SetParts(GetPlayInfo.Parts + parts);
    }
    public void SetAlreadyAct(bool value)
    {
        GetPlayInfo.SetAlreadyAct(value);
    }
}
// PlayerStatus 부분
public partial class DataManager : MonoSingleton<DataManager>
{
    public PlayerStatusInfo GetPlayerStatus { get { return dataCenter.playerStatusInfo; } }
    public void SetPlayerStatusInfo(PlayerStatusInfo info)
    {
        //SetMaxHp(info.MaxHp);
        //SetBuffHp(info.BuffHp);
        //SetRemainHp(info.RemainHp);
        //SetAtk(info.Atk);
        //SetBuffAtk(info.BuffAtk);
        dataCenter.SetPlayerStatusInfo(info);
    }
    public void SetMaxHp(int value)
    {
        GetPlayerStatus.SetMaxHp(value);
    }
    public void AddMaxHp(int value)
    {
        SetMaxHp(GetPlayerStatus.MaxHp + value);
    }
    public void SetRemainHp(int value)
    {
        if (value > dataCenter.playerStatusInfo.MaxHp + dataCenter.playerStatusInfo.BuffHp)
        {
            GetPlayerStatus.SetRemainHp(GetPlayerStatus.MaxHp + dataCenter.playerStatusInfo.BuffHp);
        }
        else
        {
            GetPlayerStatus.SetRemainHp(value);
        }
    }
    public void AddRemainHp(int value)
    {
        SetRemainHp(GetPlayerStatus.RemainHp + value);
    }
    public void SetAtk(int value)
    {
        GetPlayerStatus.SetAtk(value);
    }
    public void AddAtk(int value)
    {
        SetAtk(GetPlayerStatus.Atk + value);
    }
    public void SetBuffHp(int value)
    {
        GetPlayerStatus.SetBuffHp(value);
    }
    public void AddBuffHp(int value)
    {
        SetBuffHp(GetPlayerStatus.BuffHp + value);
        AddRemainHp(value);
    }
    public void SetBuffAtk(int value)
    {
        GetPlayerStatus.SetBuffAtk(value);
    }
    public void AddBuffAtk(int value)
    {
        SetBuffAtk(GetPlayerStatus.BuffAtk + value);
    }
}
// Weapon 부분
public partial class DataManager : MonoSingleton<DataManager>
{
    public Dictionary<WeaponType, WeaponInfo> GetWeapons { get { return dataCenter.weapons; } }
    public void SetWeapons(Dictionary<WeaponType, WeaponInfo> weapons)
    {
        dataCenter.SetWeapons(weapons);
    }
    public void AddWeapon(WeaponType type, WeaponInfo info)
    {
        GetWeapons.Add(type, info);
    }
    public void SetSkillTree(WeaponType type, Dictionary<string, WeaponSkill> tree)
    {
        GetWeapons[type].SetSkillTree(tree);
    }

}
// inventory 부분
public partial class DataManager : MonoSingleton<DataManager>
{
    public InventoryInfo GetInventoryInfo { get { return dataCenter.inventoryInfo; } }
    public void SetInventoryInfo(InventoryInfo inventory)
    {
        dataCenter.SetInventoryInfo(inventory);
    }
    public void AddComsumableItem(int key)
    {
        dataCenter.inventoryInfo.consumables[key]++;
    }
    public void RemoveComsumableItem(int key)
    {
        dataCenter.inventoryInfo.consumables[key]--;
    }
    public void AddDropItem(int key)
    {
        dataCenter.inventoryInfo.drops[key]++;
    }
    public void RemoveDropItem(int key)
    {
        dataCenter.inventoryInfo.drops[key]--;
    }



}

// Quest 부분
public partial class DataManager : MonoSingleton<DataManager>
{    //영준 추가
    public QuestInfo GetQuestInfo { get { return dataCenter.questInfo; } }
    public void SetQuestInfo(QuestInfo info)
    {
        GetQuestInfo.SetQuestStateList(info.questList);
    }
}

