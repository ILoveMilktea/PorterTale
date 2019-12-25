using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Videotest : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Button playbutton;
    public Button stopbutton;

    private void Start()
    {
        playbutton.onClick.AddListener(playvideo);
        stopbutton.onClick.AddListener(Stopvideo);
    }

    public void playvideo()
    {
        if(videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
        }
        else
        {
            videoPlayer.Play();
        }
    }

    public void Stopvideo()
    {
        videoPlayer.Stop();
    }
}
