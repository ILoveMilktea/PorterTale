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

    private string[] skillKeys = new string[] { "0", "1_1", "2_1", "2_2", "2_3", "3_1" };

    public DataCenter()
    {
        CreateCenter();
    }
    private void CreateCenter()
    {
        playInfo = new PlayInfo();
        playerStatusInfo = new PlayerStatusInfo();

        weapons = new Dictionary<WeaponType, WeaponInfo>();
        
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
    }

    public void SetWeapons(Dictionary<WeaponType, WeaponInfo> value)
    {
        weapons.Clear();
        weapons = value;
    }
}