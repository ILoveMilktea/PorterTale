using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class UIEffect
{
    public static bool IsEffectEnd = true;
    public static IEnumerator Expand(GameObject target, float fromPercent, float effectTime)
    {
        Vector3 originalScale = target.transform.localScale;
        Vector3 startScale = originalScale * fromPercent;
        target.transform.localScale = startScale;

        if (target.gameObject.activeSelf == false)
        {
            target.gameObject.SetActive(true);
        }

        float timer = 0f;
        while (timer < 1.0f)
        {
            target.transform.localScale = Vector3.Lerp(startScale, originalScale, timer);
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime / effectTime;
        }

        target.transform.localScale = originalScale;
    }
    public static IEnumerator Expand(GameObject target, Action EndCallback)
    {
        Vector3 originalScale = target.transform.localScale;
        target.transform.localScale = Vector3.zero;

        if (target.gameObject.activeSelf == false)
        {
            target.gameObject.SetActive(true);
        }

        float timer = 0f;
        while (timer < 1.0f)
        {
            target.transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, timer);
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime * 2;
        }

        target.transform.localScale = originalScale;
        EndCallback.Invoke();
    }
    public static IEnumerator ExpandFrom90(GameObject target)
    {
        Vector3 originalScale = target.transform.localScale;
        Vector3 startScale = originalScale * 0.9f;
        target.transform.localScale = startScale;

        if (target.gameObject.activeSelf == false)
        {
            target.gameObject.SetActive(true);
        }

        float timer = 0f;
        while (timer < 1.0f)
        {
            target.transform.localScale = Vector3.Lerp(startScale, originalScale, timer);
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime * 10;
        }

        target.transform.localScale = originalScale;
    }
    public static IEnumerator ContractTo90(GameObject target)
    {
        Vector3 originalScale = target.transform.localScale;
        Vector3 EndScale = originalScale * 0.9f;

        if (target.gameObject.activeSelf == false)
        {
            target.gameObject.SetActive(true);
        }

        float timer = 0f;
        while (timer < 1.0f)
        {
            target.transform.localScale = Vector3.Lerp(originalScale, EndScale, timer);
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime * 10;
        }

        target.gameObject.SetActive(false);
        target.transform.localScale = originalScale;
    }
    public static IEnumerator AlphaIn(Image target)
    {
        target.gameObject.SetActive(true);

        Color alpha255 = new Color(target.color.r, target.color.g, target.color.b, 1);
        Color alpha0 = new Color(target.color.r, target.color.g, target.color.b, 0);

        float timer = 0f;
        while (timer < 1.0f)
        {
            target.color = Color.Lerp(alpha0, alpha255, timer);
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }

        target.color = alpha255;

    }

    public static IEnumerator AlphaIn(Image target, Action EndCallback)
    {
        target.gameObject.SetActive(true);

        Color alpha255 = new Color(target.color.r, target.color.g, target.color.b, 1);
        Color alpha0 = new Color(target.color.r, target.color.g, target.color.b, 0);

        float timer = 0f;
        while (timer < 1.0f)
        {
            target.color = Color.Lerp(alpha0, alpha255, timer);
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }

        //target.color = originalColor;
        yield return new WaitForSeconds(0.5f);
        EndCallback.Invoke();
    }

    public static IEnumerator AlphaOut(Image target, Action EndCallback)
    {
        Color alpha255 = new Color(target.color.r, target.color.g, target.color.b, 1);
        Color alpha0 = new Color(target.color.r, target.color.g, target.color.b, 0);

        float timer = 0f;
        while (timer < 1.0f)
        {
            target.color = Color.Lerp(alpha255, alpha0, timer);
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }

        //target.color = originalColor;
        target.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        EndCallback.Invoke();
    }

    public static IEnumerator CutOut(Image target, Action EndCallback)
    {
        target.gameObject.SetActive(true);
        Material material = target.material;

        float startRadius = material.GetFloat("_DefaultRadius");
        float lerpRadius = startRadius;

        float timer = 0f;
        while (lerpRadius > 0)
        {
            lerpRadius = Mathf.Lerp(startRadius, 0, timer);
            material.SetFloat("_Radius", lerpRadius);
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }

        material.SetFloat("_Radius", 0);
        //target.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        EndCallback.Invoke();
    }

    public static IEnumerator CutIn(Image target, Action EndCallback)
    {

        target.gameObject.SetActive(true);

        Material material = target.material;
        material.SetFloat("_Radius", 0);

        float destinationRadius = material.GetFloat("_DefaultRadius");
        float lerpRadius = 0;

        float timer = 0f;
        while (lerpRadius < destinationRadius)
        {
            lerpRadius = Mathf.Lerp(0, destinationRadius, timer);
            material.SetFloat("_Radius", lerpRadius);
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }
        material.SetFloat("_Radius", destinationRadius);
        //target.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        EndCallback.Invoke();
    }

    public static IEnumerator SlideDownIn(Image target, Action EndCallback)
    {
        Vector2 startPos = target.rectTransform.anchoredPosition;
        Vector2 endPos = startPos;
        endPos.y -= target.rectTransform.rect.height;

        float timer = 0f;
        while (timer < 1.0f)
        {
            target.rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, timer);
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }

        target.rectTransform.anchoredPosition = endPos;
        yield return new WaitForSeconds(0.5f);
        EndCallback.Invoke();
    }

    public static IEnumerator SlideDownOut(Image target, Action EndCallback)
    {
        Vector2 startPos = target.rectTransform.anchoredPosition;
        Vector2 endPos = startPos;
        endPos.y -= target.rectTransform.rect.height;

        float timer = 0f;
        while (timer < 1.0f)
        {
            target.rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, timer);
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }

        startPos.y += target.rectTransform.rect.height;
        target.rectTransform.anchoredPosition = startPos;
        yield return new WaitForSeconds(0.5f);
        EndCallback.Invoke();
    }

    public static IEnumerator SlideHorizontalRight(RectTransform target, float effectTime)
    {
        Vector2 startPos = target.anchoredPosition;
        Vector2 endPos = startPos;
        endPos.x += target.rect.width;

        float timer = 0f;
        while (timer < 1.0f)
        {
            target.anchoredPosition = Vector2.Lerp(startPos, endPos, timer);
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime / effectTime;
        }

        target.anchoredPosition = endPos;
    }
    public static IEnumerator SlideHorizontalRight(RectTransform target, Action EndCallback)
    {
        Vector2 startPos = target.anchoredPosition;
        Vector2 endPos = startPos;
        endPos.x += target.rect.width;

        float timer = 0f;
        while (timer < 1.0f)
        {
            target.anchoredPosition = Vector2.Lerp(startPos, endPos, timer);
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }

        target.anchoredPosition = endPos;
        yield return new WaitForSeconds(0.5f);
        EndCallback.Invoke();
    }
    public static IEnumerator SlideHorizontalLeft(RectTransform target, float effectTime)
    {
        Vector2 startPos = target.anchoredPosition;
        Vector2 endPos = startPos;
        endPos.x -= target.rect.width;

        float timer = 0f;
        while (timer < 1.0f)
        {
            target.anchoredPosition = Vector2.Lerp(startPos, endPos, timer);
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime / effectTime;
        }

        target.anchoredPosition = endPos;
    }
    public static IEnumerator SlideHorizontalLeft(RectTransform target, Action EndCallback)
    {
        Vector2 startPos = target.anchoredPosition;
        Vector2 endPos = startPos;
        endPos.x -= target.rect.width;

        float timer = 0f;
        while (timer < 1.0f)
        {
            target.anchoredPosition = Vector2.Lerp(startPos, endPos, timer);
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }

        target.anchoredPosition = endPos;
        yield return new WaitForSeconds(0.5f);
        EndCallback.Invoke();
    }

}
