using System;
using System.Reflection;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class refl
{
    public string f1;
    public int p1 { get; set; }
    public int p2 { get; private set; }

    public refl()
    {
        f1 = "a";
        p1 = 1;
        p2 = 2;
    }
}

public class ReflectionLab : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        refl test = new refl();
        Type t = test.GetType();
        PropertyInfo[] properties = t.GetProperties();
        foreach (PropertyInfo item in properties)
        {
            Debug.Log(item.Name);
            Debug.Log(item.PropertyType);
            Debug.Log(item.GetValue(test));
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}