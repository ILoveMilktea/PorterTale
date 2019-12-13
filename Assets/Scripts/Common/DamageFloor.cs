using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFloor : MonoBehaviour
{
    //장판 데미지
    public int damage = 1;
    //장판 지속 시간
    public float lastingTime = 100.0f;
    //장판 크기
    public float floorRange = 5.0f;
    //장판 공격시간간격
    public float timeBetweenAttack = 0.2f;
    //공격할 수 있는지 여부
    private bool isAttackAvailable = true;

    //장판 지속 시간 끝났는지 여부
    private bool isFloorFinished = false;
    //장판 공격 시간
    private IEnumerator attackCoroutine;

    //장판 공격당하는 대상
    public List<GameObject> attackedGameObjectList;

    private void Awake()
    {

        attackedGameObjectList = new List<GameObject>();
    }

    private void OnEnable()
    {
        //장판 크기 설정
        gameObject.transform.localScale = new Vector3(floorRange, 0.1f, floorRange);
        //장판 지속 시간 체크 코루틴 시작
        StartCoroutine(FloorLastingTimer());
        StartCoroutine(AttackTimer());
    }

    private void OnDisable()
    {
        ResetValue();
        //Debug.Log("DamageFloor비활성화");
    }

    IEnumerator CheckState()
    {
        while (true)
        {
            if (isFloorFinished == true)
            {
                //Destroy(gameObject);
            }
            yield return new WaitForEndOfFrame();
        }

    }

    //장판 공격 일단 보류 ( 중력장만 필요한것같아서)
    private void Attack()
    {
        //Debug.Log("장판공격");
        //for(int i=0; i<attackedGameObjectList.Count; ++i)
        //{
        //   //여기 gameObject 인자로 DamageFloor를 추가해주어야함(엑셀에 추가되어야될듯?)
        //    FightSceneController.Instance.DamageToCharacter(gameObject, attackedGameObjectList[i], damage);

        //}        
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Enemy") || other.CompareTag("Player"))
        {
            bool isNew = true;

            for (int i = 0; i < attackedGameObjectList.Count; ++i)
            {
                if (other.gameObject == attackedGameObjectList[i])
                {
                    isNew = false;
                    break;
                }
            }
            if (isNew == true)
            {
                attackedGameObjectList.Add(other.gameObject);
            }

        }

    }

    //private void OnTriggerStay(Collider other)
    //{       
    //    //if (isAttackAvailable==true)
    //    //{
    //    //    if (other.tag == "Enemy")
    //    //    {                
    //    //        attackCoroutine = AttackTimer();
    //    //        StartCoroutine(attackCoroutine);
    //    //        Attack(other);               
    //    //    }
    //    //}        
    //}

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Player"))
        {
            attackedGameObjectList.Remove(other.gameObject);
        }
    }

    IEnumerator AttackTimer()
    {
        while (true)
        {
            Attack();
            yield return new WaitForSeconds(timeBetweenAttack);
        }
    }

    IEnumerator FloorLastingTimer()
    {
        yield return new WaitForSeconds(lastingTime);
        isFloorFinished = true;
    }

    private void ResetValue()
    {
        attackedGameObjectList.Clear();
        isAttackAvailable = true;
        isFloorFinished = false;
    }
}
