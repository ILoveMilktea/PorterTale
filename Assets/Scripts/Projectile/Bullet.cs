using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Bullet(총알) 클래스
public class Bullet : Projectile
{
    public ProjectileEffect projectileEffect;
    //총알 상태
    public enum BulletSphereState
    {   MOVING,
        STOP,
        HIT       
    };
    private BulletSphereState state;

    private void Awake()
    {
        state = BulletSphereState.MOVING;
    }

    private void OnEnable()
    {
        //ObjectPooling하기위해 활성화된 경우
        if(isOnObjectPooling==true)
        {            
            isOnObjectPooling = false;
        }
        //실제 플레이안에서 활성화된 경우
        else
        {
            projectileEffect.FireEffect(transform.position);
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
            if (state == BulletSphereState.MOVING)
            {
                if (isHit == true) //무엇가에 맞았다면
                {
                    state = BulletSphereState.HIT;
                }
                else //맞지않았다면
                {
                    if (CheckMoveDistance() == true) //사정거리만큼 움직였다면
                    {                        
                        state = BulletSphereState.STOP;
                    }
                    else //아직 사정거리만큼 안 움직였다면
                    {
                        float moveDistance = Move();
                        transform.Translate(Vector3.forward * moveDistance);
                    }                    
                }                        
            }
            else if (state == BulletSphereState.STOP)
            {
                //도착했을때 할거 처리하고
                //FightSceneController.Instance.RemoveBulletFromList(gameObject);
                //ObjectPoolManager.Instance.Free(gameObject);
                ObjectPoolManager.Instance.Free(gameObject);
                break;
            }
            else if (state == BulletSphereState.HIT)
            {
                //FightSceneController.Instance.RemoveBulletFromList(gameObject);
                //ObjectPoolManager.Instance.Free(gameObject);
                projectileEffect.HitEffect(transform.position);
                ObjectPoolManager.Instance.Free(gameObject);
                break;
            }

            yield return new WaitForEndOfFrame();
        }
    }


    //ObjectPool에 Free하기 전에 변수 값들 초기화 작업
    override protected void ResetValue()
    {
        base.ResetValue();
        state = BulletSphereState.MOVING;       
        
    }


    private void OnTriggerEnter(Collider collider)
    {
        for(int i=0; i<collisionDamageObjectTagList.Length; ++i)
        {
            if(collider.CompareTag(collisionDamageObjectTagList[i]))
            {
                OnHitObject(collider);
                if(isKnockBackMode)
                {
                    KnockBack(collider);
                }
                if (!isPenetratingActive) //관통모드가 아니라면
                {
                    isHit = true;
                }
            }
        }
        
        if(collider.CompareTag("Fence"))
        {                
            isHit = true;
        }
    }

}
