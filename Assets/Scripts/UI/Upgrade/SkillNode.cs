using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillNode : MonoBehaviour
{   
    public Button button;
    public Image prevPathLine;


    private RectTransform lineRectTransform;
    public float lineWidth = 5.0f;

    public RectTransform startPos;
    public RectTransform endPos;

    public GameObject nodeLightParticle;
    public GameObject nodeSelectParticle;
    public GameObject lineLightParticle;

    void Start()
    {
        lineRectTransform = prevPathLine.rectTransform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 startPoint = startPos.anchoredPosition;
        Vector2 endPoint = endPos.anchoredPosition;

        Vector2 differenceVector = endPoint - startPoint;

        lineRectTransform.sizeDelta = new Vector2(differenceVector.magnitude, lineWidth);
        //lineRectTransform.pivot = new Vector2(0, 0.5f);
        lineRectTransform.anchoredPosition = (startPoint - endPoint) * 0.5f;
        float angle = Mathf.Atan2(differenceVector.y, differenceVector.x) * Mathf.Rad2Deg;
        lineRectTransform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void LightOn()
    {
        prevPathLine.color = Color.white;
        button.image.color = Color.white;


        //RectTransform lineLightRect = lineLightParticle.GetComponent<RectTransform>();
        //lineLightRect.sizeDelta = lineRectTransform.sizeDelta;
        //lineLightRect.anchoredPosition = lineRectTransform.anchoredPosition;
        //lineLightRect.rotation = lineRectTransform.rotation;
        //lineLightParticle.SetActive(true);
        //nodeLightParticle.SetActive(true);
    }
    
    public void LightOff()
    {
        prevPathLine.color = new Color(0.25f, 0.25f, 0.25f);
        button.image.color = new Color(0.25f, 0.25f, 0.25f);

        //lineLightParticle.SetActive(false);
        //nodeLightParticle.SetActive(false);
    }

    public void SelectLightOn()
    {
        //nodeSelectParticle.SetActive(true);
    }
}
