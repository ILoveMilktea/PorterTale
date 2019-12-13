using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoSingleton<ParticleManager>
{
    //기본 풀링개수
    public int defaultAmount = 10;
    // particle 
    public GameObject[] particlePrefabList;
    public int[] particlePoolAmount;
    public Dictionary<string, ObjectPool> particlePoolList;

    protected override void Init()
    {
        DontDestroyOnLoad(Instance);

        particlePoolList = InitObjectPool(particlePrefabList, particlePoolAmount);
    }

    public Dictionary<string, ObjectPool> InitObjectPool(GameObject[] prefabList, int[] poolAmount)
    {
        Dictionary<string, ObjectPool> poolList = new Dictionary<string, ObjectPool>();

        for (int i = 0; i < prefabList.Length; ++i)
        {
            //각 ObjectPool에 Prefab을 넣는 작업
            ObjectPool objectPool = new ObjectPool();
            objectPool.source = prefabList[i];
            poolList[prefabList[i].name] = objectPool;

            GameObject folder = new GameObject();
            folder.name = prefabList[i].name;
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

        return poolList;
    }

    public GameObject GetParticle(string name)
    {

        if (!particlePoolList.ContainsKey(name))
        {
            Debug.Log("[ObjectPoolManger] Can't Find ObjectPool!" + name);
            return null;
        }

        ObjectPool pool = particlePoolList[name];

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

    public void FreeParticle(GameObject obj)
    {
        string keyName = obj.transform.parent.name;
        if (!particlePoolList.ContainsKey(keyName))
        {
            Debug.Log("[ObjectPoolManager] Can't find Free ObjectPool!" + name);
        }

        ObjectPool pool = particlePoolList[keyName];
        obj.SetActive(false);
        pool.unusedList.Add(obj);

    }

    public void OnParticle(string particleName, float time, Vector3 position)
    {
        StartCoroutine(OnParticleTimer(particleName, time, position));
    }

    private IEnumerator OnParticleTimer(string particleName, float time, Vector3 position)
    {       
        GameObject particleObject = GetParticle(particleName);
        particleObject.transform.position = position;
        particleObject.SetActive(true);

        yield return new WaitForSeconds(time);
        FreeParticle(particleObject);
    }

    //활성화된 프리팹들 찾아서 비활성화 시키기
    public void SetActvieFalseAllPrefabs()
    {
        // particle
        foreach (var pair in particlePoolList)
        {
            foreach (Transform child in pair.Value.folder.transform)
            {
                if (child.gameObject.activeSelf == true)
                {
                    FreeParticle(child.gameObject);
                }
            }
        }
    }

}
