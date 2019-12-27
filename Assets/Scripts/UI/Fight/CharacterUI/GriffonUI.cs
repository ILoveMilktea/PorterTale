using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GriffonUI : CharacterUI
{
    public override void ResizeUI()
    {
        Vector3 targetPos = target.transform.position;
        Vector3 targetRightHandPos = target.transform.position + new Vector3(target.transform.lossyScale.x, 0, 0); ;

        targetPos = Camera.main.WorldToScreenPoint(targetPos);
        targetRightHandPos = Camera.main.WorldToScreenPoint(targetRightHandPos);

        float characterWidth = (targetRightHandPos.x - targetPos.x);

        // resize Hp
        RectTransform hpRT = characterHp.GetComponent<RectTransform>();
        float HpWidth = hpRT.sizeDelta.x;
        float sizeRatio = characterWidth / HpWidth;
        hpRT.sizeDelta = new Vector2(hpRT.sizeDelta.x * sizeRatio * 0.5f, hpRT.sizeDelta.y);
    }
    protected override void ChaseTarget()
    {
        //Vector3 UIWorldPos = target.transform.position + new Vector3(0, target.transform.lossyScale.y * 0.5f, 0);
        Vector3 UIWorldPos = target.transform.position + new Vector3(0, target.transform.lossyScale.y * 0.3f, 0);
        Vector3 UIScreenPos = Camera.main.WorldToScreenPoint(UIWorldPos);

        rectTransform.anchoredPosition = new Vector2(UIScreenPos.x / canvas.scaleFactor, UIScreenPos.y / canvas.scaleFactor);
    }

    protected override void DisplayDamage(int value)
    {
        Vector3 headPos = Camera.main.WorldToScreenPoint(target.transform.position + new Vector3(0, target.transform.lossyScale.y, 0));
        Vector3 midPos = Camera.main.WorldToScreenPoint(target.transform.position);

        GameObject floatingText = UIPoolManager.Instance.Get("FloatingText");
        RectTransform rt = floatingText.GetComponent<RectTransform>();
        rt.anchoredPosition = rt.anchoredPosition - new Vector2(0f, (headPos.y - midPos.y) * 2);

        floatingText.SetActive(true);
        floatingText.GetComponent<FloatingText>().FloatDamage(value, target);
    }
}
