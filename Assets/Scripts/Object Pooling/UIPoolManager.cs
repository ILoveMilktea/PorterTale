using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPoolManager : MonoSingleton<UIPoolManager>
{
    //기본 풀링개수
    public int defaultAmount = 10;
    //개별 프리팹
    public GameObject[] poolList;
    //개별 풀링 개수
    public int[] poolAmount;
    private Dictionary<string, ObjectPool> objectPoolList;


    protected override void Init()
    {
        objectPoolList = new Dictionary<string, ObjectPool>();
        //DontDestroyOnLoad(Instance);
        InitObjectPool();
    }
    //private void Awake()
    //{
    //    objectPoolList = new Dictionary<string, ObjectPool>();
    //    DontDestroyOnLoad(Instance);
    //    InitObjectPool();
    //} 
    // --------> MonoSingleton 변경으로 Init함수가 Awake 대신합니다.

    public void InitObjectPool()
    {

        for (int i = 0; i < poolList.Length; ++i)
        {
            //각 ObjectPool에 Prefab을 넣는 작업
            ObjectPool objectPool = new ObjectPool();
            objectPool.source = poolList[i];
            objectPoolList[poolList[i].name] = objectPool;

            //GameObject folder = new GameObject(poolList[i].name, typeof(RectTransform));
            GameObject folder = new GameObject(poolList[i].name);
            folder.transform.parent = this.transform;
            objectPool.folder = folder;

            //Default 풀링 개수
            int amount = defaultAmount;
            //개별 풀링 개수
            if (poolAmount.Length > i && poolAmount[i] != 0)
            {
                amount = poolAmount[i];
            }

            //풀을 생성해서 비활성화 하는 작업
            for (int j = 0; j < amount; ++j)
            {

                GameObject inst = Instantiate(objectPool.source, folder.transform);
                inst.SetActive(false);
                //inst.transform.parent = folder.transform;
                objectPool.unusedList.Add(inst);

                //한번에 풀을 생성할 때 부하줄이기 위해서 코루틴 사용
                //yield return new WaitForEndOfFrame();
            }

            objectPool.maxAmount = amount;

        }
    }

    public GameObject Get(string name)
    {

        if (!objectPoolList.ContainsKey(name))
        {
            Debug.Log("[ObjectPoolManger] Can't Find ObjectPool!" + name);
            return null;
        }

        ObjectPool pool = objectPoolList[name];

        if (pool.unusedList.Count > 0)
        {

            GameObject obj = pool.unusedList[0];
            pool.unusedList.RemoveAt(0);
            //obj.SetActive(true);

            return obj;
        }
        //사용 가능한 Object가 없을 때
        else
        {
            //추가로 생성            
            GameObject obj = Instantiate(pool.source);
            obj.SetActive(false);
            obj.transform.parent = pool.folder.transform;
            return obj;
        }
    }

    public void Free(GameObject obj)
    {

        string keyName = obj.transform.parent.name;
        if (!objectPoolList.ContainsKey(keyName))
        {
            Debug.Log("[ObjectPoolManager] Can't find Free ObjectPool!" + name);
        }

        ObjectPool pool = objectPoolList[keyName];
        obj.SetActive(false);
        pool.unusedList.Add(obj);

    }

    public void Free(GameObject obj, string folderName)
    {
        if (!objectPoolList.ContainsKey(folderName))
        {
            Debug.Log("[ObjectPoolManager] Can't find Free ObjectPool!" + name);
        }

        ObjectPool pool = objectPoolList[folderName];
        obj.transform.parent = pool.folder.transform;
        obj.SetActive(false);
        pool.unusedList.Add(obj);

    }

    //UnusedList에서 빼지않고 그 안의 첫번째 객체에만 접근하기
    public GameObject ReadFirstOneFromUnusedList(string name)
    {
        if (!objectPoolList.ContainsKey(name))
        {
            Debug.Log("[ObjectPoolManger] Can't Find ObjectPool!" + name);
            return null;
        }

        ObjectPool pool = objectPoolList[name];
        if (pool.unusedList.Count > 0)
        {
            GameObject obj = pool.unusedList[0];
            return obj;
        }
        //사용 가능한 Object가 없을 때
        else
        {
            return null;
        }
    }

    //ObjectPool의 UnuseList에 접근하기
    public List<GameObject> ReadAllFromUnusedList(string name)
    {
        if (!objectPoolList.ContainsKey(name))
        {
            Debug.Log("[ObjectPoolManger] Can't Find ObjectPool!" + name);
            return null;
        }

        ObjectPool pool = objectPoolList[name];
        if (pool.unusedList.Count > 0)
        {

            return pool.unusedList;
        }
        //사용 가능한 Object가 없을 때
        else
        {
            return null;
        }
    }

    //활성화된 프리팹들 찾아서 비활성화 시키기
    public void SetActvieFalseAllPrefabs()
    {
        foreach (var pair in objectPoolList)
        {
            foreach (Transform child in pair.Value.folder.transform)
            {
                if (child.gameObject.activeSelf == true)
                {
                    Free(child.gameObject);
                }
            }
        }
    }

}
