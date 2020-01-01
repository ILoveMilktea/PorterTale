using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayInfo
{
    public string CurDungeon { get; private set; }
    public float Playtime { get; private set; }
    public int Stage { get; private set; }
    public int Parts { get; private set; }
    public bool AlreadyAct { get; private set; }

    public PlayInfo()
    {
        CurDungeon = "";
        Playtime = 0;
        Stage = 1;
        Parts = 0;
        AlreadyAct = false;
    }

    public void ResetDungeonPlay()
    {
        CurDungeon = "";
        Playtime = 0;
        Stage = 1;
        AlreadyAct = false;
    }
    public void SetCurDungeon(string value) { CurDungeon = value; }
    public void SetPlaytime(float value) { Playtime = value; }
    public void SetStage(int value) { Stage = value; }
    public void SetParts(int value) { Parts = value; }
    public void SetAlreadyAct(bool value) { AlreadyAct = value; }

}

public class PlayerStatusInfo
{
    public int MaxHp { get; private set; }
    public int RemainHp { get; private set; }
    public int Atk { get; private set; }
    public int BuffHp { get; private set; }
    public int BuffAtk { get; private set; }

    public PlayerStatusInfo()
    {
        MaxHp = 100;
        RemainHp = 100;
        Atk = 5;
        BuffHp = 0;
        BuffAtk = 0;
    }

    public void ResetDungeonPlayStatus()
    {
        RemainHp = MaxHp;
        BuffHp = 0;
        BuffAtk = 0;
    }
    public void SetMaxHp(int value) { MaxHp = value; }
    public void SetRemainHp(int value) { RemainHp = value; }
    public void SetAtk(int value) { Atk = value; }
    public void SetBuffHp(int value) { BuffHp = value; }
    public void SetBuffAtk(int value) { BuffAtk = value; }
}

public class WeaponInfo
{
    public string Name { get; private set; }
    public Dictionary<string, WeaponSkill> SkillTree { get; private set; }

    public WeaponInfo()
    {
        Name = "";
        SkillTree = new Dictionary<string, WeaponSkill>();
    }

    public void SetName(string value) { Name = value.ToString(); }
    public void SetSkillTree(Dictionary<string, WeaponSkill> value) { SkillTree = value; }

}

public class WeaponSkill
{
    //public int ParentKey { get; private set; }
    public bool IsActivated { get; private set; }
    public int UsedParts { get; private set; }

    public WeaponSkill()
    {
        //ParentKey = 0;
        IsActivated = false;
        UsedParts = 0;
    }

    //public void SetParentKey(int value) { ParentKey = value; }
    public void SetIsActivated(bool value) { IsActivated = value; }
    public void SetUsedParts(int value) { UsedParts = value; }
}

public class QuestInfo
{  
    //public int ParentKey { get; private set; }
    public Dictionary<string,string> questList { get; private set; }   

    public QuestInfo()
    {
        questList = new Dictionary<string, string>();        
        questList.Add("GriffonQuest", "quest_available");        
    }
   
    public void SetQuestStateList(Dictionary<string, string> questStateList) { this.questList = questStateList; }
    public void SetQuestState(string questName, string questProgressState)
    {
        foreach(var quest in questList)
        {
            if(quest.Key==questName)
            {
                questList[questName] = questProgressState;
                break;
            }
        }
    }
}