using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FloatingText : MonoBehaviour
{
    private RectTransform rectTransform;
    private Text damageText;

    public Canvas canvas;

    private float upMoveAmount;
    private float downMoveAmount;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        damageText = GetComponent<Text>();

        canvas = GetComponentInParent<Canvas>();

        upMoveAmount = 20;
        downMoveAmount = 10;
    }

    public void FloatDamage(int damage, Transform target)
    {
        StartCoroutine(DisplayDamage(damage, target));
    }

    public IEnumerator DisplayDamage(int damage, Transform target)
    {
        damageText.text = damage.ToString();
        damageText.enabled = true;


        Vector3 UIWorldPos = target.transform.position + new Vector3(0, target.transform.lossyScale.y, 0);
        Vector3 UIScreenPos = Camera.main.WorldToScreenPoint(UIWorldPos);

        rectTransform.anchoredPosition = new Vector2(UIScreenPos.x / canvas.scaleFactor, UIScreenPos.y / canvas.scaleFactor);

        // Floating Text start floating
        Vector3 targetPos = target.transform.position;
        Vector3 targetRightHandPos = target.transform.position + new Vector3(target.transform.lossyScale.x, 0, 0); ;

        targetPos = Camera.main.WorldToScreenPoint(targetPos);
        targetRightHandPos = Camera.main.WorldToScreenPoint(targetRightHandPos);

        float characterWidth = (targetRightHandPos.x - targetPos.x);

        float xRandomFactor = UnityEngine.Random.Range(-characterWidth * 0.4f, characterWidth * 0.4f);
        float yRandomFactor = UnityEngine.Random.Range(-characterWidth * 0.2f, characterWidth * 0.2f);

        Vector2 startPosition = rectTransform.anchoredPosition + new Vector2(xRandomFactor, yRandomFactor);
        Vector2 destination = startPosition + new Vector2(0f, upMoveAmount);

        float timer = 0f;
        while (timer <= 1f)
        {
            //scale
            //rectTransform.localScale = Vector2.Lerp(new Vector2(0f, 0f), new Vector2(1f, 1f), timer);
            //position
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, destination, timer);

            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime * 5;
        }

        // Sink and destroy floating text
        startPosition = rectTransform.anchoredPosition;
        destination = rectTransform.anchoredPosition + new Vector2(0f, -downMoveAmount);
        timer = 0;

        while (timer <= 1f)
        {
            //alpha
            damageText.color = Color.Lerp(new Color(1f, 1f, 1f, 1f), new Color(1f, 1f, 1f, 0.5f), timer);
            //position
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, destination, timer);

            yield return new WaitForEndOfFrame();

            timer += Time.deltaTime * 3;
        }

        OffFloatingText();
    }

    public void OffFloatingText()
    {
        UIPoolManager.Instance.Free(gameObject);
    }
}
