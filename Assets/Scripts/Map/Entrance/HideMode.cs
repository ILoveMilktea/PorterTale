using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideMode : MonoBehaviour
{
    public GameObject block1;
    public GameObject block2;

    protected bool hideMode = false;

    private void Start()
    {
        block1.SetActive(true);
        block2.SetActive(true);

        StartCoroutine(WaitFightEnd());    
    }
    
    private IEnumerator WaitFightEnd()
    {
        hideMode = false;

        while(FightSceneController.Instance.GetCurrentFightState() != FightState.Clear_Stage && FightSceneController.Instance.GetCurrentFightState() != FightState.Clear_Dungeon)
        {
            yield return new WaitForEndOfFrame();
        }


        // 효과음?
        block1.SetActive(false);
        block2.SetActive(false);
    }
}
