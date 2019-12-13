using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//발사체 클래스
public abstract class Projectile : MonoBehaviour
{
    //처음 시작의 ObjectPooling중인지 실제 플레이중인지 판별하기 위한 것
    protected bool isOnObjectPooling = true;   

    protected GameObject source;

    //public LayerMask collisionMask;
    //총알 속도
    protected float speed;
    //총알 데미지
    protected float damage;
    //총알 최대 거리
    protected float maxRange=10.0f;
    //총알 관통 여부
    protected bool isPenetratingActive=false;
    //필살기 모드 여부
    protected bool isSpecialMode = false;
    //KnockBack 모드 여부
    protected bool isKnockBackMode = false;
    //KnockBack 힘
    protected float knockBackForce = 5.0f;
    //KnockBack 시간
    protected float knockBackDuration = 0.2f;

    //speed바뀌는 모드
    protected bool isSpeedChangingMode = false;
    //전체거리중 speed가 바뀌는 지점 비율(예시)30퍼센트)
    protected float speedChangingRatio = 0.2f;    
    //바뀔 speed
    protected float speedAfterChange;

    //총알이 무언가에 맞았는지 여부
    protected bool isHit = false;
  

    //지금까지 간 거리 합
    protected float distanceTotal = 0f;

    //충돌해서 데미지 판정할 Object들 tag리스트
    public string[] collisionDamageObjectTagList;

    //KnockBack에서 사용하는 muzzlePosition
    private Vector3 knockBackMuzzlePosition;

    //Set함수
    public void SetSource(GameObject source)
    {
        this.source = source;
    }

    public void SetSpeed(float speed)
    {       
        this.speed = speed;
        this.speedAfterChange = speed +15;
    }

    public void SetMaxRange(float maxRange)
    {
        this.maxRange = maxRange;
    }
    
    public void SetKnockBackMode(bool mode)
    {
        isKnockBackMode = mode;
    }

    public void SetKnockBackMuzzlePosition(Vector3 muzzlePosition)
    {
        this.knockBackMuzzlePosition = muzzlePosition;
    }

    public void SetKnockBackForce(float force)
    {
        knockBackForce = force;
    }

    public void SetDamage(float damage)
    {
        this.damage = damage;
    }

    public void SetSpecialMode(bool mode)
    {
        isSpecialMode = mode;
    }

    public void SetPentratingActive(bool mode)
    {
        this.isPenetratingActive = mode;
    }

    public void SetSpeedChangingRatio(float ratio)
    {
        this.speedChangingRatio = ratio;
    }

    public void SetSpeedAfterChange(float changedSpeed)
    {
        this.speedAfterChange = changedSpeed;
    }

    public void SetRotation(Quaternion angle)
    {
        transform.rotation = angle;
    }

    public void SetSpeedChangeMode(bool mode)
    {
        this.isSpeedChangingMode = mode;
    }

    protected float Move()
    {       
        float moveDistance = speed * Time.deltaTime;

        if (distanceTotal + moveDistance > maxRange)
        {
            moveDistance = maxRange - distanceTotal;
            distanceTotal = maxRange;
        }
        else
        {
            distanceTotal += moveDistance;
        }

        if (isSpeedChangingMode == true && maxRange*speedChangingRatio<distanceTotal)
        {
            speed = speedAfterChange;
            isSpeedChangingMode = false;
        }

        return moveDistance;
    }



    //충돌 검사
    //protected void CheckCollision()
    //{
    //    //Ray 생성
    //    Ray ray = new Ray(transform.position, transform.forward);
    //    RaycastHit hit;

    //    //Ray 발사
    //    if (Physics.Raycast(ray, out hit, moveDistance, collisionMask, QueryTriggerInteraction.Collide))
    //    {
    //        OnHitObject(hit);
    //        return true;
    //    }
    //    return false;



    //}

    ////물체와 부딪혔을 때 작동하는 함수
    //protected void OnHitObject(RaycastHit hit)
    //{
    //    Collider collider = hit.collider;
    //    IDamageable damageableObject=collider.GetComponent<IDamageable>();
    //    if(damageableObject!=null)
    //    {
    //        damageableObject.TakeHit(damage);
    //        GameObject target = hit.transform.gameObject;
    //        //FightSceneController.Instance.DamageToCharacter(source, target);

    //    }

    //}   

    //물체와 부딪혔을 때 작동하는 함수
    protected void OnHitObject(Collider collider)
    {        
        IDamageable damageableObject = collider.GetComponent<IDamageable>();
        if (damageableObject != null)
        {            
            GameObject target = collider.gameObject;
            //damageableObject.TakeHit(damage);
            FightSceneController.Instance.DamageToCharacter(source, target, damage);
        }
    }

    //발사체가 최대 거리까지가 움직였는지 체크
    virtual protected bool CheckMoveDistance()
    {            
        if(distanceTotal==maxRange)
        {           
            distanceTotal = 0;
            return true;
            //ObjectPoolManager.Instance.Free(gameObject);
        }
        return false;
    }

    //넉백 효과
    public void KnockBack(Collider collider)
    {        
        Vector3 dir = (collider.transform.position - knockBackMuzzlePosition).normalized;
        dir.y = 0;

        IDamageable damageableObject = collider.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.TakeKnockBack(dir, knockBackForce, knockBackDuration);
        }
    }

    //ObjectPool에 Free하기 전에 변수 값들 초기화 작업
    virtual protected void ResetValue()
    {
        distanceTotal = 0;
        isKnockBackMode = false;
        isSpecialMode = false;
        isSpeedChangingMode = false;
        isPenetratingActive = false;
        isHit = false;
    }

    
}
