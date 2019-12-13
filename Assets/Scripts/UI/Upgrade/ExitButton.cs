using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitButton : MonoBehaviour
{
    private Stack<GameObject> UIStack;

    public Button exitButton;

    private void Awake()
    {
        UIStack = new Stack<GameObject>();
        exitButton.onClick.AddListener(PullUI);
    }

    public void PushUI(GameObject targetUI)
    {
        UIStack.Push(targetUI);
    }

    public void PullUI()
    {
        if(UIStack.Count > 0)
        {
            GameObject target = UIStack.Pop();
            target.SetActive(false);
        }

        if(UIStack.Count == 0)
        {
            gameObject.SetActive(false);
        }
    }
    
    public void ClearStack()
    {
        while(UIStack.Count > 0)
        {
            PullUI();
        }
    }
}
