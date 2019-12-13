using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FenceGenerator : MonoBehaviour
{
    private Transform fencePrefab;
    private List<Transform> fencePrefabList;
    private string fencePrefabName;

    private int prevEntrancePos;
    private int nextEntrancePos;

    public void PrepareFence(Vector2 mapSize, int stage)
    {
        fencePrefabName = Const_ObjectPoolName.Fence_Griffon_Base;
        fencePrefabList = new List<Transform>();

        List<GameObject> fencePrefabListFromObjectPool = ObjectPoolManager.Instance.ReadAllFromUnusedList(fencePrefabName);
        Vector3 fenceLocalScale = fencePrefabListFromObjectPool[0].transform.localScale;

        SetEntrance("Stage" + stage.ToString(), mapSize.x);

        int count = 0;

        for (int y = 0; y < mapSize.y; ++y)
        {
            for (int x = 0; x < mapSize.x; ++x)
            {
                if (y >= 1 && y < mapSize.y - 1)
                {
                    if (x >= 1 && x < mapSize.x - 1)
                    {
                        continue;
                    }
                }


                if (y == 0 && x == prevEntrancePos)
                {
                    Vector3 entrancePosition = new Vector3(x * fenceLocalScale.x + 0.5f, -1f, y * fenceLocalScale.z);
                    string prefabName = "";
                    prefabName = StageEntranceTable.Instance.GetTuple("Stage" + stage.ToString(), "prev").m_prefabName;
                    
                    Transform tr = ObjectPoolManager.Instance.Get(prefabName).transform;
                    tr.position = entrancePosition;
                    //prefabTmp.parent = transform;
                    fencePrefabList.Add(tr);

                    x++;
                }
                else if(y == mapSize.y - 1 && x == nextEntrancePos)
                {
                    Vector3 entrancePosition = new Vector3(x * fenceLocalScale.x + 0.5f, -1f, y * fenceLocalScale.z);
                    string prefabName = "";
                    prefabName = StageEntranceTable.Instance.GetTuple("Stage" + stage.ToString(), "next").m_prefabName;

                    Transform tr = ObjectPoolManager.Instance.Get(prefabName).transform;
                    tr.position = entrancePosition;
                    //prefabTmp.parent = transform;
                    fencePrefabList.Add(tr);

                    x++;
                }
                else
                {
                    //Vector3 fencePosition = new Vector3((-mapSize.x / 2 + 0.5f + x)*fenceLocalScale.x, 0, (mapSize.y / 2 - 0.5f - y)*fenceLocalScale.z);
                    Vector3 fencePosition = new Vector3(x * fenceLocalScale.x, 0, y * fenceLocalScale.z);

                    Transform prefabTmp = fencePrefabListFromObjectPool[count].transform;
                    prefabTmp.position = fencePosition;
                    //prefabTmp.parent = transform;
                    fencePrefabList.Add(prefabTmp);
                    count++;
                }

            }
        }
    }

    private void SetEntrance(string stage, float mapWidth)
    {
        Dictionary<string, StageEntranceInfo> table = StageEntranceTable.Instance.GetTable(stage);


        prevEntrancePos = SetEntrancePos(table["prev"].m_entrancePos, mapWidth);
        nextEntrancePos = SetEntrancePos(table["next"].m_entrancePos, mapWidth);
    }

    private int SetEntrancePos(string pos, float mapWidth)
    {
        float entrancePos = 0f;

        switch(pos)
        {
            case "left":
                entrancePos = mapWidth * 0.25f;
                break;
            case "middle":
                entrancePos = mapWidth * 0.5f;
                break;
            case "right":
                entrancePos = mapWidth * 0.75f;
                break;
            case "middle_hide":
                entrancePos = mapWidth * 0.5f;
                break;
            case "none":
                entrancePos = -1;
                break;
        }

        return (int)(entrancePos - 0.5f);
    }

    public void GenerateFence(Vector2 mapSize)
    {
        for(int i=0; i<fencePrefabList.Count; ++i)
        {
            fencePrefabList[i].gameObject.SetActive(true);
        }
    }

}
