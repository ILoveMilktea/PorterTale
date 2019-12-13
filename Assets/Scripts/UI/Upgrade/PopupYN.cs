using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupYN : MonoBehaviour
{
    private Action yesCallback;
    private Action noCallback;

    public Text description;
    public Button yesButton;
    public Button noButton;

    private void Awake()
    {
        yesButton.onClick.AddListener(OnClickYesButton);
        noButton.onClick.AddListener(OnClickNoButton);
    }

    public void OnClickYesButton()
    {
        Action actFunction = yesCallback;
        RemoveCallback();
        actFunction();
    }

    public void OnClickNoButton()
    {
        Action actFunction = noCallback;
        noCallback();
        actFunction();
    }
    public void SetCallback(Action yesFunction, Action noFunction)
    {
        yesCallback = yesFunction;
        noCallback = noFunction;
    }

    public void SetDescription(string p_description)
    {
        description.text = p_description;
    }

    // 제거
    public void RemoveCallback()
    {
        yesCallback = null;
        noCallback = null;
    }
}
