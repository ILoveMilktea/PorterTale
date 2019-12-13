using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusWindow : MonoBehaviour
{
    public Text dungeonName;
    public Text hp;
    public Slider hpSlider;
    public Text atk;
    public Text parts;
    public Text time;

    public Text prevStage;
    public Text nextStage;

    // Start is called before the first frame update
    void Start()
    {
        SetWindow();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha8))
        {
            SetWindow();
        }
    }

    private void SetWindow()
    {
        dungeonName.text = DataManager.Instance.GetPlayInfo.CurDungeon + " Dungeon";
        int remainHp = DataManager.Instance.GetPlayerStatus.RemainHp;
        int maxHp = DataManager.Instance.GetPlayerStatus.MaxHp;
        int buffHp = DataManager.Instance.GetPlayerStatus.BuffHp;
        hp.text = remainHp.ToString() + "/" + maxHp.ToString() + "(+" + buffHp.ToString() + ")";
        hpSlider.maxValue = maxHp + buffHp;
        hpSlider.value = remainHp;
        atk.text = DataManager.Instance.GetPlayerStatus.Atk.ToString() +
                "(+" + DataManager.Instance.GetPlayerStatus.BuffAtk.ToString() + ")";
        parts.text = DataManager.Instance.GetPlayInfo.Parts.ToString();
        time.text = DataManager.Instance.GetPlayInfo.Playtime.ToString("00:00");
        
        prevStage.text = "Stage " + DataManager.Instance.GetPlayInfo.Stage.ToString();
        nextStage.text = "Stage " + (DataManager.Instance.GetPlayInfo.Stage + 1).ToString();


        //w1.text = "w1 : " + DataManager.Instance.GetWeapons;

    }

    public void RedrawWindow()
    {
        SetWindow();
    }
}
