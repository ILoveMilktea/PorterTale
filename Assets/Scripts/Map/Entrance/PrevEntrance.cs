﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrevEntrance : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == Constants.PlayerTag)
        {
            FightSceneController.Instance.Retreat();
        }
    }
}
