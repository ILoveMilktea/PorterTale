using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    protected GameObject floatingText;
    
    protected Text characterName;
    protected Slider characterHp;

    [SerializeField]
    protected Transform target;
    protected RectTransform rectTransform;

    protected virtual void Awake()
    {
        floatingText = Resources.Load("Prefab/UI/FloatingText") as GameObject;

        characterName = GetComponentInChildren<Text>();
        characterHp = GetComponentInChildren<Slider>();

        rectTransform = GetComponent<RectTransform>();

        characterName.resizeTextForBestFit = true;
    }


    // Update is called once per frame
    // Late 안쓰면 ui가 먼저 가버려서 떨림 현상 생김
    protected void LateUpdate()
    {
        ChaseTarget();
        
    }

    public virtual void ResizeUI()
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
    protected virtual void ChaseTarget()
    {
        Vector3 UIWorldPos = target.transform.position + new Vector3(0, target.transform.lossyScale.y, 0);
        Vector3 UIScreenPos = Camera.main.WorldToScreenPoint(UIWorldPos);

        float fovX = 1920.0f / Screen.width;
        float fovY = 1080.0f / Screen.height;
        rectTransform.anchoredPosition = new Vector2(UIScreenPos.x * fovX, UIScreenPos.y * fovY);
    }

    public virtual void SetStatus(CharacterStatus status)
    {
        SetName(status.name);
        SetMaxHp(status.maxHp);
    }

    public void SetName(string name)
    {
        characterName.text = name;
    }
    public void SetTarget(GameObject targetObj)
    {
        target = targetObj.transform;
    }
    public void SetMaxHp(int maxHp)
    {
        characterHp.maxValue = maxHp;
        characterHp.value = maxHp;
    }
    public int GetRemainHp()
    {
        return (int)characterHp.value;
    }

    public void HpUp(int value)
    {
        characterHp.value += value;
        DisplayDamage(value);
    }

    public virtual void HpDown(int value)
    {
        if(characterHp.value < value)
        {
            characterHp.value = 0;
        }
        else
        {
            characterHp.value -= value;
        }
        
        DisplayDamage(value);
    }

    protected virtual void DisplayDamage(int value)
    {
        Vector3 headPos = Camera.main.WorldToScreenPoint(target.transform.position + new Vector3(0, target.transform.lossyScale.y, 0));
        Vector3 midPos = Camera.main.WorldToScreenPoint(target.transform.position);

        //rectTransform.anchoredPosition = new Vector2(UIScreenPos.x, UIScreenPos.y);

        //GameObject damageText = ObjectPoolManager.Instance.Get("FloatingText");
        //damageText.transform.parent = rectTransform;

        GameObject damageText = Instantiate(floatingText, rectTransform) as GameObject;
        RectTransform rt = damageText.GetComponent<RectTransform>();
        rt.anchoredPosition = rt.anchoredPosition - new Vector2(0f, (headPos.y - midPos.y) * 2);

        StartCoroutine(damageText.GetComponent<FloatingText>().DisplayDamage(value, rectTransform));
    }
}
