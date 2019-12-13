using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCharacterUI : CharacterUI
{
    public Text remainHp;
    
    public override void ResizeUI()
    {
        Vector3 targetPos = target.transform.position;
        Vector3 targetRightHandPos = target.transform.position + new Vector3(target.transform.lossyScale.x, 0, 0); ;

        targetPos = Camera.main.WorldToScreenPoint(targetPos);
        targetRightHandPos = Camera.main.WorldToScreenPoint(targetRightHandPos);

        float characterWidth = (targetRightHandPos.x - targetPos.x);

        // resize name
        //RectTransform nameRT = characterName.rectTransform;
        //float nameWidth = nameRT.sizeDelta.x;
        //float sizeRatio = characterWidth / nameWidth;
        //nameRT.sizeDelta = new Vector2(nameRT.sizeDelta.x * sizeRatio, nameRT.sizeDelta.y);
        // resize Hp
        RectTransform hpRT = characterHp.GetComponent<RectTransform>();
        float HpWidth = hpRT.sizeDelta.x;
        float sizeRatio = characterWidth / HpWidth;
        hpRT.sizeDelta = new Vector2(hpRT.sizeDelta.x * sizeRatio, hpRT.sizeDelta.y);
    }

    public void SetRemainHp(int value)
    {
        characterHp.value = value;
        remainHp.text = value.ToString();
    }

    public override void HpDown(int value)
    {
        base.HpDown(value);
        remainHp.text = characterHp.value.ToString();
    }
}
