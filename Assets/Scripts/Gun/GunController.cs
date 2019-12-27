using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Const_PassiveSkill
{
    public const string AttackDamageUp = "ATKup";
    public const string AttackSpeedUp = "ASup";
    public const string AttackRangeUp = "ARup";
    public const string AttackAngleUp = "AAup";
    public const string CriticalChanceUp = "CCup";
    public const string CriticalRateUp = "CRup";
}

public static class Const_ActiveSkill_1st
{ 
    public const string LinearGun = "U_PanetratingShot";
    public const string ShotGun = "U_DoubleBarrel";
    public const string EnergySphere = "U_GravityField";
    public const string NodeKey = "3_1";
}

public class GunController : MonoBehaviour
{
    public Transform weaponHold;
    public Gun startingGun;
    private Gun equippedGun;
    private string poolFolderName;

    private WeaponType equipedWeaponType;

    void Start()
    {
        if(startingGun!=null)
        {
            equipedWeaponType = (WeaponType)Enum.Parse(typeof(WeaponType), startingGun.name);
            EquipGun();
        }
    }

    public void EquipGun()
    {
        if(equippedGun!=null)
        {
            ObjectPoolManager.Instance.Free(equippedGun.gameObject, poolFolderName);
        }

        GameObject gunToEquip = ObjectPoolManager.Instance.Get(equipedWeaponType.ToString());
        poolFolderName = gunToEquip.transform.parent.name;

        gunToEquip.transform.position = weaponHold.position;
        gunToEquip.transform.rotation = weaponHold.rotation;
        gunToEquip.transform.parent = weaponHold;

        equippedGun = gunToEquip.GetComponent<Gun>();
        ApplyActivatedSkill(equipedWeaponType, equippedGun);
        equippedGun.gameObject.SetActive(true);
    }


    public void OnTirggerHold()
    {
        if(equippedGun!=null)
        {            
            equippedGun.OnTriggerHold();
        }
    }

    //skill 공격 추가
    public void OnSkillTriggerHold()
    {
        if (equippedGun != null)
        {
            equippedGun.OnSkillTriggerHold();
        }
    }
    
    public void OnTriggerRelease()
    {
        if(equippedGun!=null)
        {
            equippedGun.OnTriggerRelease();
        }
    }

    public float CurrentWeaponRange()
    {
        return equippedGun.maxRange;
    }

    public float CurrentWeaponAngle()
    {
        return equippedGun.projectileMaxAngle;
    }

    // swappppp
    public WeaponType SwapWeapon()
    {
        if (equipedWeaponType == WeaponType.ShotGun)
        {
            equipedWeaponType = WeaponType.EnergySphereGun;
        }
        else
        {
            equipedWeaponType++;
        }

        EquipGun();

        return equipedWeaponType;
    }

    // 스킬 켜진거 적용하는곳
    private void ApplyActivatedSkill(WeaponType type, Gun gun)
    {
        Dictionary<string, WeaponSkill> skillTree = DataManager.Instance.GetWeapons[type].SkillTree;

        foreach (var node in skillTree)
        {
            if (node.Value.IsActivated && node.Key != "0")
            {
                WeaponSkillInfo skillInfo = WeaponSkillTable.Instance.GetTuple(type.ToString(), node.Key);

                switch (skillInfo.m_spriteName)
                {
                    case Const_PassiveSkill.AttackDamageUp:
                        gun.damage += (int)skillInfo.m_value;
                        break;
                    case Const_PassiveSkill.AttackSpeedUp:
                        gun.msBetweenShots -= skillInfo.m_value;
                        break;
                    case Const_PassiveSkill.AttackRangeUp:
                        gun.maxRange += skillInfo.m_value;
                        break;
                    case Const_PassiveSkill.AttackAngleUp:
                        gun.directionNumber += (int)skillInfo.m_value;
                        break;
                    case Const_PassiveSkill.CriticalChanceUp:
                        gun.criticalChance += skillInfo.m_value;
                        break;
                    case Const_ActiveSkill_1st.LinearGun:
                    case Const_ActiveSkill_1st.ShotGun:
                    case Const_ActiveSkill_1st.EnergySphere:
                        gun.isSkillEquiped = true;
                        gun.skillKey = Const_ActiveSkill_1st.NodeKey;
                        break;
                }
            }
        }

        if(gun.isSkillEquiped)
        {
            FightUIController.Instance.SkillButtonOn(type, gun.skillKey);
        }
        else
        {
            FightUIController.Instance.SkillButtonOff();
        }
    }


}
