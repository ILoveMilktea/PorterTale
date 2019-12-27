using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpritePoolManager : MonoSingleton<SpritePoolManager>
{
    public Sprite[] spriteList;
    private Dictionary<string, Sprite> spriteDict;

    protected override void Init()
    {
        //DontDestroyOnLoad(Instance);
        InitSpritePool();
    }


    public void InitSpritePool()
    {
        spriteDict = new Dictionary<string, Sprite>();
        for (int i = 0; i < spriteList.Length; ++i)
        {
            spriteDict.Add(spriteList[i].name, spriteList[i]);
        }
    }

    public Sprite Get(string name)
    {

        if (!spriteDict.ContainsKey(name))
        {
            Debug.Log("noooo" + name);
            return null;
        }

        return spriteDict[name];
    }

}
