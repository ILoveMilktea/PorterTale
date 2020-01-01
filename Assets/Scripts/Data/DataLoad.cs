using System;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataLoad
{
    public void LoadUserData(string directoryPath)
    {
        LoadPlayInfo(directoryPath + Const_Path.playInfoPath);
        LoadPlayerStatusInfo(directoryPath + Const_Path.playerStatusInfoPath);
        LoadWeaponInfo(directoryPath + Const_Path.WeaponInfoPath);
        LoadQuestInfo(directoryPath + Const_Path.QuestInfoPath);
    }


    private void LoadPlayInfo(string dataPath)
    {
        byte[] saveFile = File.ReadAllBytes(dataPath);
        MemoryStream memoryStream = new MemoryStream(saveFile);
        BinaryReader binaryReader = new BinaryReader(memoryStream);

        PlayInfo info = new PlayInfo();

        info = (PlayInfo)SerializeReadData(binaryReader, info);
        DataManager.Instance.SetPlayInfo(info);

        binaryReader.Close();
        memoryStream.Close();
    }
    private void LoadPlayerStatusInfo(string dataPath)
    {
        byte[] saveFile = File.ReadAllBytes(dataPath);
        MemoryStream memoryStream = new MemoryStream(saveFile);
        BinaryReader binaryReader = new BinaryReader(memoryStream);

        PlayerStatusInfo info = new PlayerStatusInfo();

        info = (PlayerStatusInfo)SerializeReadData(binaryReader, info);
        DataManager.Instance.SetPlayerStatusInfo(info);

        binaryReader.Close();
        memoryStream.Close();
    }
    private void LoadWeaponInfo(string dataPath)
    {
        byte[] saveFile = File.ReadAllBytes(dataPath);
        MemoryStream memoryStream = new MemoryStream(saveFile);
        BinaryReader binaryReader = new BinaryReader(memoryStream);

        Dictionary<WeaponType, WeaponInfo> weapons = new Dictionary<WeaponType, WeaponInfo>();
        int count = binaryReader.ReadInt32();

        for(int i = 0; i < count; i++)
        {
            WeaponType type = (WeaponType)Enum.Parse(typeof(WeaponType), binaryReader.ReadString());
            WeaponInfo info = new WeaponInfo();
            info = LoadWeapon(binaryReader, info);

            weapons.Add(type, info);
        }
        DataManager.Instance.SetWeapons(weapons);

        binaryReader.Close();
        memoryStream.Close();
    }
    private WeaponInfo LoadWeapon(BinaryReader note, WeaponInfo info)
    {
        PropertyInfo[] properties = info.GetType().GetProperties();

        foreach (PropertyInfo item in properties)
        {
            if (item.PropertyType == typeof(Dictionary<string, WeaponSkill>))
            {
                Dictionary<string, WeaponSkill> skillList = new Dictionary<string, WeaponSkill>();
                int count = note.ReadInt32();

                for (int i = 0; i < count; i++)
                {
                    string key = note.ReadString();
                    WeaponSkill skill = new WeaponSkill();
                    skill = (WeaponSkill)SerializeReadData(note, skill);
                    skillList.Add(key, skill);
                }

                info.SetSkillTree(skillList);
            }
            else
            {
                MethodInfo SetMethod = info.GetType().GetMethod("Set" + item.Name, new Type[] { item.PropertyType });

                SetMethod.Invoke(info, FindTypeAndRead(note, item.PropertyType));
            }
        }

        return info;
    }

    private void LoadQuestInfo(string dataPath)
    {
        //byte[] saveFile = File.ReadAllBytes(dataPath);
        //MemoryStream memoryStream = new MemoryStream(saveFile);
        //BinaryReader binaryReader = new BinaryReader(memoryStream);

        //QuestInfo info = new QuestInfo();
        //Debug.Log("데이터부름" + info.questList["GriffonQuest"]);
        //info = (QuestInfo)SerializeReadData(binaryReader, info);
        //DataManager.Instance.SetQuestInfo(info);

        //binaryReader.Close();
        //memoryStream.Close();

        byte[] saveFile = File.ReadAllBytes(dataPath);
        MemoryStream memoryStream = new MemoryStream(saveFile);
        BinaryReader binaryReader = new BinaryReader(memoryStream);

        Dictionary<string, string> questList = new Dictionary<string, string>();
        int count = binaryReader.ReadInt32();

        for (int i = 0; i < count; i++)
        {
            string key = binaryReader.ReadString();
            string value = binaryReader.ReadString();            

            questList.Add(key, value);
        }

        QuestInfo questInfo = new QuestInfo();
        questInfo.SetQuestStateList(questList);
        Debug.Log("데이터부름" + questInfo.questList["GriffonQuest"]);
        DataManager.Instance.SetQuestInfo(questInfo);

        binaryReader.Close();
        memoryStream.Close();
    }


    private object SerializeReadData(BinaryReader note, object info)
    {
        PropertyInfo[] properties = info.GetType().GetProperties();

        foreach (PropertyInfo item in properties)
        {
            MethodInfo SetMethod = info.GetType().GetMethod("Set" + item.Name, new Type[] { item.PropertyType });

            SetMethod.Invoke(info, FindTypeAndRead(note, item.PropertyType));
        }

        return info;
    }

    private object[] FindTypeAndRead(BinaryReader note, Type propertyType)
    {
        if (propertyType == typeof(int))
        {
            return new object[] { note.ReadInt32() };
        }
        else if (propertyType == typeof(float))
        {
            return new object[] { note.ReadSingle() };
        }
        else if (propertyType == typeof(bool))
        {
            return new object[] { note.ReadBoolean() };
        }
        else if (propertyType == typeof(string))
        {
            return new object[] { note.ReadString() };
        }
        else if (propertyType == typeof(byte))
        {
            return new object[] { note.ReadByte() };
        }
        else
        {
            Debug.Log("Cannot find type");
            return null;
        }
    }
    
}
