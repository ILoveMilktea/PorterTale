using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{ 
    //실제 Instantiate할 Prefab
    public GameObject source;
    //Pool의 Object 개수
    public int maxAmount;
    //Hierarchy view에서 구별을 위한 empty object
    public GameObject folder;

    //미사용 GameObject 리스트
    public List<GameObject> unusedList = new List<GameObject>();

}
