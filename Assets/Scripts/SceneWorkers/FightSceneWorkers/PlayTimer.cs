using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayTimer : MonoBehaviour
{
    public Text debugTimer; // display playtime for debug

    private float playtime;
    private bool isFight;
    private IEnumerator timer;

    private void Start()
    {
        playtime = DataManager.Instance.GetPlayInfo.Playtime;
        isFight = false;
        timer = Timer();
    }

    public IEnumerator Timer()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();

            if (isFight)
            {
                playtime += Time.deltaTime * 100;
            }

            debugTimer.text = playtime.ToString("00:00");
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
        StartCoroutine(timer);
    }
    public void StopTimer()
    {
        StopCoroutine(timer);
    }

}
