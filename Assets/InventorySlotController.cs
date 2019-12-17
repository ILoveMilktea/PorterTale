using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlotController : MonoBehaviour
{
    public Item item;

    public void Use() 
    { 
        if(item)
        {
            Debug.Log(item.itemName);
        }
    }

}
