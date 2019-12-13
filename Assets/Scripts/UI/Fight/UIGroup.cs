using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGroup
{
    // 여러 UI를 한번에 동작시키기 위한 class
    private List<GameObject> members = new List<GameObject>();

    public void SetMember(GameObject member)
    {
        if (!members.Contains(member))
        {
            members.Add(member);
        }
    }

    public void RemoveMember(GameObject member)
    {
        if (members.Contains(member))
        {
            members.Remove(member);
        }
    }

    public void ActiveAllMembers()
    {
        foreach (var member in members)
        {
            member.SetActive(true);
        }
    }

    public void InactiveAllMembers()
    {
        foreach (var member in members)
        {
            member.SetActive(false);
        }
    }
}

