using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//영준수정
public abstract class Enemy : LivingEntity
{
    //공격할 타겟(이 게임에서는 플레이어)
    public GameObject target;
    
    public float moveSpeed = 5;    
    public float turnSpeed = 3.0f;

    //발사 시작 위치
    public Transform muzzle;

    //외부 스크립트
    public EnemyAttack enemyAttack;

    //처음 시작의 ObjectPooling중인지 실제 플레이중인지 판별하기 위한 것
    protected bool isOnObjectPooling = true;

    protected abstract void Move();

    protected void LockOnTarget()
    {
        Vector3 dir = (target.transform.position - transform.position).normalized;
        dir.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
        transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

    public void SetMoveSpeed(float speed)
    {
        this.moveSpeed = speed;
    }

    public void SetTurnSpeed(float speed)
    {
        this.turnSpeed = speed;
    }

}
