using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FloatingText : MonoBehaviour
{
    private RectTransform rectTransform;
    private Text damageText;

    private float upMoveAmount;
    private float downMoveAmount;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        damageText = GetComponent<Text>();

        upMoveAmount = 20;
        downMoveAmount = 10;
    }


    public IEnumerator DisplayDamage(int damage, RectTransform target)
    {
        //rectTransform.anchoredPosition += new Vector2(rectTransform.sizeDelta.x * 0.5f, rectTransform.sizeDelta.y * 0.5f);
        damageText.text = damage.ToString();
        damageText.enabled = true;

        // Floating Text start floating
        float xRandomFactor = UnityEngine.Random.Range(-target.sizeDelta.x * 0.4f, target.sizeDelta.x * 0.4f);
        float yRandomFactor = UnityEngine.Random.Range(-target.sizeDelta.y * 0.1f, target.sizeDelta.y * 0.5f);

        Vector2 startPosition = rectTransform.anchoredPosition + new Vector2(xRandomFactor, yRandomFactor);
        Vector2 destination = startPosition + new Vector2(0f, upMoveAmount);

        float timer = 0f;
        while (rectTransform.anchoredPosition.y < destination.y)
        {
            //scale
            //rectTransform.localScale = Vector2.Lerp(new Vector2(0f, 0f), new Vector2(1f, 1f), timer);
            //position
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, destination, timer);

            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime * 3;
        }

        // Wait a moment in character's head
        //yield return new WaitForSeconds(0.1f);

        // Sink and destroy floating text
        startPosition = rectTransform.anchoredPosition;
        destination = rectTransform.anchoredPosition + new Vector2(0f, -downMoveAmount);
        timer = 0;

        while (rectTransform.anchoredPosition.y > destination.y)
        {
            //alpha
            damageText.color = Color.Lerp(new Color(1f, 1f, 1f, 1f), new Color(1f, 1f, 1f, 0f), timer);
            //position
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, destination, timer);

            yield return new WaitForEndOfFrame();

            timer += Time.deltaTime * 2;
        }

        //ObjectPoolManager.Instance.Free(gameObject);
        Destroy(gameObject);
    }
}
