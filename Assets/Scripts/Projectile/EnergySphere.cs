using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//EnerySphere(에너지구) 클래스
public class EnergySphere : Projectile
{
    public ProjectileEffect projectileEffect;

    //상태
    public enum EnerySphereState
    { MOVING,
      STOP,
      HIT,
      GRAVITY,
      DESTROY,
    };
    public EnerySphereState state;

    //Special모드일때 구 지속시간
    public float lastingTime = 10.0f;
    
    private bool isArrived=false;
    //필살기 모드인지   
    private bool isChildrenDestroyed = false;

    //자식 붙어있는거 관리
    public GameObject[] childAttached;

    private void Awake()
    {
        state = EnerySphereState.MOVING;
        isSpecialMode = true;
    }

    private void OnEnable()
    {
        //ObjectPooling하기위해 활성화된 경우
        if (isOnObjectPooling == true)
        {
            isOnObjectPooling = false;
        }
        //실제 플레이안에서 활성화된 경우
        else
        {
            projectileEffect.FireEffect(transform.position);
            //gameObject.transform.GetChild(2).gameObject.SetActive(true);
            StartCoroutine(CheckState());
        }
    }

    private void OnDisable()
    {
        ResetValue();
    }

    IEnumerator CheckState()
    {
        FightSceneController.Instance.AddBulletToList(gameObject);
        while (true)
        {
            if (state == EnerySphereState.MOVING)
            {
                if (isHit == true) //무엇가에 맞았다면
                {
                    state = EnerySphereState.HIT;
                }
                else //맞지않았다면
                {
                    if (CheckMoveDistance() == true) //사정거리만큼 움직였다면
                    {
                        state = EnerySphereState.STOP;
                    }
                    else //아직 사정거리만큼 안 움직였다면
                    {
                        float moveDistance = Move();
                        transform.Translate(Vector3.forward * moveDistance);
                    }
                }
            }
            else if (state == EnerySphereState.HIT)
            {
                projectileEffect.HitEffect(transform.position);
                if (isSpecialMode == false)
                {                    
                    //맞았을때 할거 처리하고
                    state = EnerySphereState.DESTROY;
                }
                else
                {
                    ////DamageFloor 활성화
                    //gameObject.transform.GetChild(0).gameObject.SetActive(true);
                    ////AttractFloor 활성화                                
                    //gameObject.transform.GetChild(1).gameObject.SetActive(true);
                    for (int i = 0; i < childAttached.Length; ++i)
                    {
                        gameObject.transform.GetChild(i).gameObject.SetActive(true);
                    }
                    //gameObject.transform.GetChild(2).gameObject.SetActive(false);
                    state = EnerySphereState.GRAVITY;
                    StartCoroutine(DestroyTimer());
                }
            }
            else if (state == EnerySphereState.STOP)
            {
                if (isSpecialMode == false)
                {                    
                    //도착했을때 할거 처리하고
                    state = EnerySphereState.DESTROY;
                }
                else
                {
                    ////DamageFloor 활성화
                    //gameObject.transform.GetChild(0).gameObject.SetActive(true);
                    ////AttractFloor 활성화                                
                    //gameObject.transform.GetChild(1).gameObject.SetActive(true);

                    for (int i = 0; i < childAttached.Length; ++i)
                    {
                        //GameObject child = Instantiate(childAttached[i], transform);
                        //child.transform.parent = this.gameObject.transform;
                        gameObject.transform.GetChild(i).gameObject.SetActive(true);
                    }
                    state = EnerySphereState.GRAVITY;
                    StartCoroutine(DestroyTimer());
                }

            }
            else if (state == EnerySphereState.DESTROY)
            {
                if (isSpecialMode == false)
                {
                    FightSceneController.Instance.RemoveBulletFromList(gameObject);
                    ObjectPoolManager.Instance.Free(gameObject);
                }
                else
                {
                    if (isChildrenDestroyed == false)
                    {

                        for (int i = 0; i < gameObject.transform.childCount; ++i)
                        {
                            // 총알까지 끄시면 곤란해요~
                            if (gameObject.transform.GetChild(i).gameObject.activeSelf == true &&
                                i != gameObject.transform.childCount - 1)
                            {
                                //Debug.Log("자식 destroy" + gameObject.transform.GetChild(i).gameObject);                            
                                gameObject.transform.GetChild(i).gameObject.SetActive(false);
                            }
                        }
                        isChildrenDestroyed = true;

                    }
                    else
                    {
                        FightSceneController.Instance.RemoveBulletFromList(gameObject);
                        ObjectPoolManager.Instance.Free(gameObject);
                    }

                    
                }

            }
            yield return new WaitForEndOfFrame();
        }
        
    }  

    private void OnDestroy()
    {        
        //StopAllCoroutines();        
    }  

    //ObjectPool에 Free하기 전에 변수 값들 초기화 작업
    override protected void ResetValue()
    {
        base.ResetValue();       
        state = EnerySphereState.MOVING;
        isArrived = false;
        isChildrenDestroyed = false;
    }  

    //protected override bool CheckCollision(float moveDistance)
    //{        
    //    //Ray 생성
    //    Ray ray = new Ray(transform.position, transform.forward);
    //    RaycastHit hit;

    //    //Ray 발사
    //    if (Physics.Raycast(ray, out hit, moveDistance, collisionMask, QueryTriggerInteraction.Collide))
    //    {
    //        //Debug.Log("enemy");
            
    //        if(isSpecialMode==false)
    //        {
    //            OnHitObject(hit);
    //            return true;
    //        }                      
    //    }
    //    return false;
    //}    

    //일정 거리까지 가서 멈추기
    override protected bool CheckMoveDistance()
    { 
        if (distanceTotal == maxRange)
        {            
            return true;
        }
        return false;
    }

    IEnumerator DestroyTimer()
    {
        yield return new WaitForSeconds(lastingTime);
        state = EnerySphereState.DESTROY;
    }

    private void OnTriggerEnter(Collider collider)
    {
        for (int i = 0; i < collisionDamageObjectTagList.Length; ++i)
        {
            if (collider.CompareTag(collisionDamageObjectTagList[i]))
            {
                OnHitObject(collider);
                if (!isPenetratingActive) //관통모드가 아니라면
                {
                    isHit = true;
                }
            }
        }

        if (collider.CompareTag("Fence"))
        {
           
            isHit = true;
        }
    }

}
