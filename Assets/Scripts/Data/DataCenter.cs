using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    EnergySphereGun,
    LinearGun,
    ShotGun
}

public class DataCenter
{
    public PlayInfo playInfo { get; private set; }
    public PlayerStatusInfo playerStatusInfo { get; private set; }
    public Dictionary<WeaponType, WeaponInfo> weapons { get; private set; }
    public InventoryInfo inventoryInfo { get; private set; }
    public QuestInfo questInfo { get; private set; }

    public DataCenter()
    {
        CreateCenter();
    }
    private void CreateCenter()
    {
        playInfo = new PlayInfo();
        playerStatusInfo = new PlayerStatusInfo();

        weapons = new Dictionary<WeaponType, WeaponInfo>();

        //영준 추가
        questInfo = new QuestInfo();

        foreach (WeaponType type in Enum.GetValues(typeof(WeaponType)))
        {
            WeaponInfo info = new WeaponInfo();

            Dictionary<string, WeaponSkill> skillTree = new Dictionary<string, WeaponSkill>();

            foreach (var key in WeaponSkillTable.Instance.GetTable(type.ToString()).Keys)
            {
                WeaponSkill skill = new WeaponSkill();
                if(key == WeaponSkillTable.Instance.GetTable(type.ToString()).Keys.First())
                {
                    skill.SetIsActivated(true);
                }
                skillTree.Add(key, skill);
            }

            info.SetName(type.ToString());
            info.SetSkillTree(skillTree);
            weapons.Add(type, info);
        }

        inventoryInfo = new InventoryInfo();
    }
    public void SetPlayInfo(PlayInfo value)
    {
        playInfo = value;
    }

    public void SetPlayerStatusInfo(PlayerStatusInfo value)
    {
        playerStatusInfo = value;
    }
    public void SetWeapons(Dictionary<WeaponType, WeaponInfo> value)
    {
        weapons.Clear();
        weapons = value;
    }

    public void SetInventoryInfo(InventoryInfo value)
    {
        inventoryInfo = value;
    }
}