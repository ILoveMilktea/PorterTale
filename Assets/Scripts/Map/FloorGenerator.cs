using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FloorGenerator : MonoBehaviour
{
    public NavMeshSurface navMeshSurface;

    private string floorPrefabName;
        
    public void PrepareFloor(Vector2 mapSize, int stage, out List<List<Transform>> floorPrefabList)
    {
       
        floorPrefabName = Const_ObjectPoolName.Floor_Griffon_Base;

        floorPrefabList = new List<List<Transform>>();
       
        //int currentPrefabIndex = 0;
        //int floorPrefabMaxLength = floorPrefab.Length;
        //int firstPrefabIndex = 0;

        //나중에 Read String Stage이름으로 바꿔야함
        
        List<GameObject> floorPrefabListFromObjectPool = ObjectPoolManager.Instance.ReadAllFromUnusedList(floorPrefabName);

        Vector3 floorLocalScale = floorPrefabListFromObjectPool[0].transform.localScale;

        int count = 0;        

        for (int y = 0; y < mapSize.y; ++y)
        {            
            for (int x = 0; x < mapSize.x; ++x)
            {
               
                floorPrefabList.Add(new List<Transform>());

                //Vector3 floorPosition = new Vector3((-mapSize.x / 2 + 0.5f + x)*floorLocalScale.x, 0, (mapSize.y/2 - 0.5f - y)*floorLocalScale.z);
                Vector3 floorPosition = new Vector3((x + 1.0f) * floorLocalScale.x, -1f, (y + 1.0f) * floorLocalScale.z);

                Transform prefabTmp = floorPrefabListFromObjectPool[count].transform;
                prefabTmp.position = floorPosition;
                //prefabTmp.parent = transform;
                floorPrefabList[y].Add(prefabTmp);                             
                count++;                         
                
                

                //currentPrefabIndex++;
                //if (currentPrefabIndex == floorPrefabMaxLength)
                //{
                //    currentPrefabIndex = 0;
                //}
            }
            //currentPrefabIndex = firstPrefabIndex + 1;
            //firstPrefabIndex++;
            //if(currentPrefabIndex==floorPrefabMaxLength)
            //{
            //    currentPrefabIndex = 0;
            //    firstPrefabIndex = 0;
            //}
        }

       

    }

    public void GenerateFloor(Vector2 mapSize, ref List<List<Transform>> floorPrefabList)
    {
        for (int y = 0; y < mapSize.y; ++y)
        {
            for (int x = 0; x < mapSize.x; ++x)
            {
                floorPrefabList[y][x].gameObject.SetActive(true);
            }
        }

        //Stage에서 맵 활성화할때 NavMesh Bake함
        navMeshSurface.BuildNavMesh();
    }


}
