using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupResult : MonoBehaviour
{
    private Action okCallback;

    public Image image;
    public Text description;
    public Button okButton;

    private void Awake()
    {
        okButton.onClick.AddListener(OnClickOKButton);
    }

    public void OnClickOKButton()
    {
        Action actFunction = okCallback;
        RemoveCallback();
        actFunction();
    }

    public void SetCallback(Action okFunction)
    {
        okCallback = okFunction;
    }

    public void SetDescription(string p_description)
    {
        description.text = p_description;
    }

    // 제거
    public void RemoveCallback()
    {
        okCallback = null;
    }
}
