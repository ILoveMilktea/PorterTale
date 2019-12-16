using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayTimer : MonoBehaviour
{
    private float playtime;
    private bool isFight;
    private IEnumerator timer;

    public IEnumerator Timer()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();

            if (isFight)
            {
                playtime += Time.deltaTime * 100;
            }

            FightUIController.Instance.SetTimerText(playtime);
        }
    }


    public void StandbyTimer()
    {
        isFight = false;
    }
    public void FreezeTimer()
    {
        isFight = false;
        Time.timeScale = 0f;
    }

    public void ReleaseTimer()
    {
        isFight = true;
        Time.timeScale = 1f;
    }

    public float GetPlaytime()
    {
        return playtime;
    }

    public void ResetPlaytime()
    {
        playtime = 0f;
    }
    public void StartTimer()
    {
        playtime = DataManager.Instance.GetPlayInfo.Playtime;
        isFight = false;
        timer = Timer();
        StartCoroutine(timer);
    }
    public void StopTimer()
    {
        StopCoroutine(timer);
    }

}
