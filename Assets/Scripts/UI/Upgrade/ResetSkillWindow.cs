using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ResetSkillWindow : MonoBehaviour, IBeginDragHandler
{
    public WeaponWindow weaponWindow;

    private void Awake()
    {
        weaponWindow = FindObjectOfType<WeaponWindow>();
    }

    public void OnBeginDrag(PointerEventData data)
    {
        weaponWindow.ResetSkillWindow();
    }

}
