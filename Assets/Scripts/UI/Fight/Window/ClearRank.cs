using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClearRank : MonoBehaviour
{
    public Image medal;
    public Text timeLimit;
    public Text reward;
    public void SetTimeLimit(string value)
    {
        timeLimit.text = value;
    }
    public void SetReward(string value)
    {
        reward.text = value;
    }

}
