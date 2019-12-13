using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FightState
{
    Standby,
    Fight,
    Pause,
    Dead,
    Clear_Stage,
    Clear_Dungeon,
    End
}

public class FightStateObserver : MonoBehaviour
{
    private Dictionary<FightState, List<Action>> stateChangeCallback = new Dictionary<FightState, List<Action>>();

    private FightState prevFightState;
    public FightState curFightState { get; private set; }

    void Start()
    {
        prevFightState = FightState.Standby;
        curFightState = FightState.Standby;
    }

    // Update is called once per frame
    void Update()
    {
        // Game state is changed
        if (prevFightState != curFightState)
        {
            if (stateChangeCallback.ContainsKey(curFightState))
            {
                List<Action> callbacks = stateChangeCallback[curFightState];

                foreach (var callback in callbacks)
                {
                    callback();
                }

                prevFightState = curFightState;
            }
        }
    }

    // GameManager에서 전투 상황 변경시 호출됩니다.
    public void SetCurrentFightState(FightState fightState)
    {
        curFightState = fightState;
    }

    // 전투 상황이 변경 될 때에 부를 Callback 함수들을 등록합니다.
    public void SetStateChangeCallback(FightState fightState, Action function)
    {
        if (stateChangeCallback.ContainsKey(fightState))
        {
            stateChangeCallback[fightState].Add(function);
        }
        else
        {
            List<Action> callbackList = new List<Action>();
            callbackList.Add(function);

            stateChangeCallback.Add(fightState, callbackList);
        }
    }

    // 제거
    public void RemoveStateChangeCallback(FightState fightState, Action function)
    {
        if (stateChangeCallback.ContainsKey(fightState))
        {
            if (stateChangeCallback[fightState].Contains(function))
            {
                stateChangeCallback[fightState].Remove(function);
            }
            else
            {
                Debug.Log("no func");
            }
        }
        else
        {
            Debug.Log("no state");
        }
    }
}
