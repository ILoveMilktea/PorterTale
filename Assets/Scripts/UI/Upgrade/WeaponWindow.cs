using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillTexts
{
    public string name;
    public string description;

    public SkillTexts(string p_name, string p_description)
    {
        name = p_name;
        description = p_description;
    }
}

public class WeaponWindow : MonoBehaviour
{
    public List<Button> weapons;
    [HideInInspector]
    public Button selectedWeapon;

    public Image weaponImage;
    public List<SkillNode> skillButtons;
    public Dictionary<string, SkillNode> skillNodes;
    [HideInInspector]
    public Button selectedSkill;
    private Dictionary<string, SkillTexts> skillDict;

    public Text parts;
    public GameObject skillTextWindow;
    public Text skillName;
    public Text skillDescription;

    public Button exit;
    public Button upgrade;
    public Text upgradeButtonText;

    private string curWeaponName;
    private string curSkillKey;

    private void Awake()
    {
        skillDict = new Dictionary<string, SkillTexts>();

        foreach (var weapon in weapons)
        {
            weapon.onClick.AddListener(OnClickWeaponButton);
        }

        skillNodes = new Dictionary<string, SkillNode>();
        foreach(var button in skillButtons)
        {
            skillNodes.Add(button.gameObject.name, button);
        }

        exit.onClick.AddListener(CloseWindow);
        SetParts();
        SetOnAwake();
    }

    private void Start()
    {
        SetSkillTree();
    }

    private void SetOnAwake()
    {
        selectedWeapon = weapons[0];
        selectedWeapon.interactable = false;
        curWeaponName = selectedWeapon.image.name;

        ResetSkillDescription();
        upgrade.gameObject.SetActive(false);
    }

    public void SetParts()
    {
        int value = DataManager.Instance.GetPlayInfo.Parts;
        parts.text = value.ToString();
    }

    public void OnClickWeaponButton()
    {
        selectedWeapon.image.color = selectedWeapon.colors.normalColor;
        selectedWeapon.interactable = true;
        selectedWeapon = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        selectedWeapon.image.color = selectedWeapon.colors.selectedColor;
        selectedWeapon.interactable = false;

        // replace background image
        weaponImage.sprite = Resources.Load<Sprite>("Image/Weapon/" + selectedWeapon.image.name);
        curWeaponName = selectedWeapon.image.name;
        SetSkillTree();
        ResetSkillDescription();
    }

    public void ResetSkillWindow()
    {
        ResetSkillDescription();
        upgrade.gameObject.SetActive(false);
    }

    private void ResetSkillDescription()
    {
        WeaponSkillInfo weaponSkillInfo = WeaponSkillTable.Instance.GetTuple(selectedWeapon.image.name, "0");
        skillName.text = weaponSkillInfo.m_skillName;
        skillDescription.text = weaponSkillInfo.m_description;
    }

    public void SetSkillTree()
    {
        skillDict.Clear();

        // replace all nodes
        foreach (var node in skillNodes)
        {
            WeaponSkillInfo skill = WeaponSkillTable.Instance.GetTuple(curWeaponName, node.Key);
            // skill image
            node.Value.button.image.sprite = Resources.Load<Sprite>("Image/Skill/" + skill.m_spriteName);
            // skill text
            SetSkillTexts(node.Key, skill.m_skillName, skill.m_description);
            // skill path
            if (node.Key != "0")
            {
                SetSkillNode(skill.m_prevPath, node.Value);
            }

            node.Value.button.onClick.AddListener(SetSkillDescription);
        }
    }

    private void SetSkillTexts(string key, string name, string description)
    {
        SkillTexts skillTexts = new SkillTexts(name, description);
        skillDict.Add(key, skillTexts);
    }

    private void SetSkillNode(string prevNodeKey, SkillNode node)
    {
        WeaponType weaponType = (WeaponType)Enum.Parse(typeof(WeaponType), weaponImage.sprite.name);
        string curNodeKey = node.gameObject.name;

        if (DataManager.Instance.GetWeapons[weaponType].SkillTree[prevNodeKey].IsActivated)
        {
            if(DataManager.Instance.GetWeapons[weaponType].SkillTree[curNodeKey].IsActivated)
            {
                node.LightOn();
            }
            else
            {
                node.LightOff();
            }
        }
        else
        {
            node.LightOff();
        }
    }
    
    public void SetSkillDescription()
    {
        //skillTextWindow.SetActive(true);

        selectedSkill = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();

        skillName.text = skillDict[selectedSkill.gameObject.name].name;
        skillDescription.text = skillDict[selectedSkill.gameObject.name].description;

        curSkillKey = selectedSkill.gameObject.name;
        SetUpgradeButton();
    }

    public void SetUpgradeButton()
    {
        WeaponType weaponType = (WeaponType)Enum.Parse(typeof(WeaponType), curWeaponName);

        if (DataManager.Instance.GetWeapons[weaponType].SkillTree[curSkillKey].IsActivated)
        {
            upgrade.gameObject.SetActive(false);
        }
        else
        {
            string prevSkillKey = WeaponSkillTable.Instance.GetTuple(curWeaponName, curSkillKey).m_prevPath;
            if(DataManager.Instance.GetWeapons[weaponType].SkillTree[prevSkillKey].IsActivated)
            {
                upgrade.gameObject.SetActive(true);
                upgradeButtonText.text = WeaponSkillTable.Instance.GetTuple(curWeaponName, curSkillKey).m_needParts.ToString();
            }
            else
            {
                upgrade.gameObject.SetActive(false);
            }
        }

        upgrade.onClick.RemoveAllListeners();
        upgrade.onClick.AddListener(UpgradeWeapon);
    }

    private void UpgradeWeapon()
    {
        int need = int.Parse(upgradeButtonText.text);
        int have = int.Parse(parts.text);

        if(have >= need)
        {
            WeaponType weaponType = (WeaponType)Enum.Parse(typeof(WeaponType), curWeaponName);

            DataManager.Instance.AddParts(-need);
            DataManager.Instance.GetWeapons[weaponType].SkillTree[curSkillKey].SetIsActivated(true);
            DataManager.Instance.Save();
            SetParts();
            skillNodes[curSkillKey].LightOn();
        }
        else
        {
            //nooooooo
        }
    }
    
    private void CloseWindow()
    {
        UpgradeSceneController.Instance.CloseSelectWeaponWindow();
    }
}
