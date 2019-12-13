using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttractFloor : MonoBehaviour
{
    //
    private bool isOnObjectPooling = true;

    //장판 지속 시간
    public float lastingTime = 100.0f;
    //장판 크기
    public float floorRange = 20.0f;
    //공격할 수 있는지
    private bool isAttractAvailable = true;
    //장판이 끌어당기는 힘
    public float floorForce = 100.0f;
    //장판 지속 시간 끝났는지 여부
    private bool isFloorFinished = false;

    //장판의 부모Object
    private GameObject parentObject;
    //장판 부모Object의 위치
    private Vector3 endPosition;

    //AddForce영향을 받고있는 모든 GameObject저장
    public List<GameObject> affectedGameObjectList;

    private void OnEnable()
    {
        //ObjectPooling하기위해 활성화된 경우
        //if (isOnObjectPooling == true)
        //{
        //    isOnObjectPooling = false;
        //}
        //실제 플레이안에서 활성화된 경우
        //else
        //{
        //장판 크기 설정
        gameObject.transform.localScale = new Vector3(floorRange, 0.1f, floorRange);
        //장판 지속 시간 체크 코루틴 시작
        //StartCoroutine(FloorLastingTimer());
        //Debug.Log("attract활성화됨");
        ParticleManager.Instance.OnParticle("GravityField", 5.0f, transform.position);
        StartCoroutine(CheckState());

        //}
    }

    private void OnDisable()
    {
        StopColliders();
        ResetValue();
        //StopAllCoroutines();
        //Debug.Log("AttractFloor비활성화");
    }

    IEnumerator CheckState()
    {
        while (true)
        {
            if (transform.parent != null)
            {
                parentObject = transform.parent.gameObject;
                //Debug.Log("활성"+parentObject.activeSelf);
                endPosition = parentObject.transform.position;
            }
            else
            {
                endPosition = transform.position;
            }

            if (isFloorFinished == true)
            {
                //Destroy(gameObject);
            }
            yield return new WaitForEndOfFrame();
        }
    }

    private void StopColliders()
    {
        for (int i = 0; i < affectedGameObjectList.Count; ++i)
        {
            if (affectedGameObjectList[i] != null)
            {
                //Debug.Log("stop할거"+affectedGameObjectList[i]);
                affectedGameObjectList[i].GetComponent<Rigidbody>().velocity = Vector3.zero;

            }
        }

        //if(affectedGameObjectList.Count==0)
        //{

        //}
    }

    //장판 끌어당기기
    private void Attract(Collider other, Vector3 dir)
    {

        if (isFloorFinished == false)
        {
            other.GetComponent<Rigidbody>().velocity = Vector3.zero;
            other.GetComponent<Rigidbody>().AddForce(dir * floorForce, ForceMode.Acceleration);

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isFloorFinished == false)
        {
            if (other.CompareTag("Enemy") || other.CompareTag("Player"))
            {
                affectedGameObjectList.Add(other.gameObject);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (isFloorFinished == false)
        {
            if (other.CompareTag("Enemy") || other.CompareTag("Player"))
            {
                Vector3 currTargetPosition = other.transform.position;
                //끌어들일 방향(normalized해줘야 부드럽게 끌어들여짐-벡터의 방향은 그대로두고 크기만 1.0)
                Vector3 dir = (endPosition - currTargetPosition).normalized;
                //Debug.Log("부모" + endPosition+"/"+"Player"+currTargetPosition);

                //Vector3 dir = (currTargetPosition-endPosition).normalized;
                dir.y = 0;
                //attractCoroutine = AttractTimer();
                Attract(other, dir);
                //StartCoroutine(attractCoroutine);                           
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Player"))
        {
            other.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }

    IEnumerator FloorLastingTimer()
    {
        yield return new WaitForSeconds(lastingTime);
        isFloorFinished = true;
    }

    public void ResetValue()
    {
        isAttractAvailable = true;
        isFloorFinished = false;
    }
}
