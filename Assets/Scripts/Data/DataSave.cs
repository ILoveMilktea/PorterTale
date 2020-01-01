using System;
using System.Reflection;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataSave
{
    public void SaveUserData(string directoryPath)
    {
        SavePlayInfo(directoryPath + Const_Path.playInfoPath);
        SavePlayerStatusInfo(directoryPath + Const_Path.playerStatusInfoPath);
        SaveWeaponInfo(directoryPath + Const_Path.WeaponInfoPath);
        //øµ¡ÿ√ﬂ∞°
        SaveQuestInfo(directoryPath + Const_Path.QuestInfoPath);
    }

    private void SavePlayInfo(string dataPath)
    {
        MemoryStream memoryStream = new MemoryStream();
        BinaryWriter binaryWriter = new BinaryWriter(memoryStream);

        PlayInfo info = DataManager.Instance.GetPlayInfo;
        PropertyInfo[] properties = info.GetType().GetProperties();

        foreach (PropertyInfo item in properties)
        {
            binaryWriter = FindTypeAndWrite(binaryWriter, item.PropertyType, item.GetValue(info));
        }

        // ?ùÏÑ±??Tuple ListÎ•?Í∞?Name.bytes ?åÏùºÎ°??Ä??
        SaveToStorage(dataPath, memoryStream.ToArray());

        binaryWriter.Close();
        memoryStream.Close();
    }
    private void SavePlayerStatusInfo(string dataPath)
    {
        MemoryStream memoryStream = new MemoryStream();
        BinaryWriter binaryWriter = new BinaryWriter(memoryStream);

        PlayerStatusInfo info = DataManager.Instance.GetPlayerStatus;
        PropertyInfo[] properties = info.GetType().GetProperties();

        foreach (PropertyInfo item in properties)
        {
            binaryWriter = FindTypeAndWrite(binaryWriter, item.PropertyType, item.GetValue(info));
        }

        // ?ùÏÑ±??Tuple ListÎ•?Í∞?Name.bytes ?åÏùºÎ°??Ä??
        SaveToStorage(dataPath, memoryStream.ToArray());

        binaryWriter.Close();
        memoryStream.Close();
    }
    private void SaveWeaponInfo(string dataPath)
    {
        MemoryStream memoryStream = new MemoryStream();
        BinaryWriter binaryWriter = new BinaryWriter(memoryStream);

        Dictionary<WeaponType, WeaponInfo> weapons = DataManager.Instance.GetWeapons;
        binaryWriter.Write(weapons.Count);

        foreach(var weapon in weapons)
        {
            binaryWriter.Write(weapon.Key.ToString());
            binaryWriter = SaveWeapon(binaryWriter, weapon.Value);
        }

        // ?ùÏÑ±??Tuple ListÎ•?Í∞?Name.bytes ?åÏùºÎ°??Ä??
        SaveToStorage(dataPath, memoryStream.ToArray());

        binaryWriter.Close();
        memoryStream.Close();
    }
    private BinaryWriter SaveWeapon(BinaryWriter note, WeaponInfo weapon)
    {
        PropertyInfo[] properties = weapon.GetType().GetProperties();

        foreach (PropertyInfo item in properties)
        {
            if (item.PropertyType == typeof(Dictionary<string, WeaponSkill>))
            {
                Dictionary<string, WeaponSkill> skillList = (Dictionary<string, WeaponSkill>)item.GetValue(weapon);
                note.Write(skillList.Count);

                foreach(var skill in skillList)
                {
                    note.Write(skill.Key);
                    SaveSkillList(note, skill.Value);
                }
            }
            else
            {
                note = FindTypeAndWrite(note, item.PropertyType, item.GetValue(weapon));
            }

        }
        return note;
    }
    private BinaryWriter SaveSkillList(BinaryWriter note, WeaponSkill skillList)
    {
        PropertyInfo[] properties = skillList.GetType().GetProperties();

        foreach (PropertyInfo item in properties)
        {
            FindTypeAndWrite(note, item.PropertyType, item.GetValue(skillList));
        }

        return note;
    }
    
    private void SaveQuestInfo(string dataPath)
    {
        //MemoryStream memoryStream = new MemoryStream();
        //BinaryWriter binaryWriter = new BinaryWriter(memoryStream);

        //QuestInfo info = DataManager.Instance.GetQuestInfo;
        //PropertyInfo[] properties = info.GetType().GetProperties();

        //foreach (PropertyInfo item in properties)
        //{
        //    binaryWriter = FindTypeAndWrite(binaryWriter, item.PropertyType, item.GetValue(info));
        //}

        //// ?ùÏÑ±??Tuple ListÎ•?Í∞?Name.bytes ?åÏùºÎ°??Ä??
        //SaveToStorage(dataPath, memoryStream.ToArray());

        //binaryWriter.Close();
        //memoryStream.Close();

        MemoryStream memoryStream = new MemoryStream();
        BinaryWriter binaryWriter = new BinaryWriter(memoryStream);

        QuestInfo questInfos = DataManager.Instance.GetQuestInfo;
        binaryWriter.Write(questInfos.questList.Count);

        foreach (var quest in questInfos.questList)
        {
            binaryWriter.Write(quest.Key.ToString());
            binaryWriter.Write(quest.Value.ToString());
        }

        // ?ùÏÑ±??Tuple ListÎ•?Í∞?Name.bytes ?åÏùºÎ°??Ä??
        SaveToStorage(dataPath, memoryStream.ToArray());

        binaryWriter.Close();
        memoryStream.Close();
    }

    private BinaryWriter FindTypeAndWrite(BinaryWriter note, Type propertyType, object item)
    {
        if (propertyType == typeof(int))
        {
            note.Write((int)item);
        }
        else if (propertyType == typeof(float))
        {
            note.Write((float)item);
        }
        else if (propertyType == typeof(bool))
        {
            note.Write((bool)item);
        }
        else if (propertyType == typeof(string))
        {
            note.Write((string)item);
        }
        else if (propertyType == typeof(byte))
        {
            note.Write((byte)item);
        }
        else
        {
            Debug.Log("Cannot find type");
        }

        return note;
    }


    private void SaveToStorage(string path, byte[] binaryFile)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        FileStream fileStream = new FileStream(path, FileMode.Create);
        fileStream.Write(binaryFile, 0, binaryFile.Length);
        fileStream.Close();
    }
}
