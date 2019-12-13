using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//1단계 보스 몬스터 스크립트
[RequireComponent(typeof(EnemyAttack))]
public class RotateSphereEnemy : Enemy
{
    private enum RotateSphereEnemyState
    {
        IDLE,
        SEARCHING,
        CHASING,
        ATTACKING_ROTATING,
        DEAD
    };
    
    //공격할 때 회전 시간 간격
    public float rotateTime;
    //공격할 때 회전 간격(몇 도 마다 공격할지)
    public Vector3 rotateAngleBetweenAttack;

    //상태
    private RotateSphereEnemyState rotateSphereEnemyState;    

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
            SetHealth();
            isDead = false;
            rotateSphereEnemyState = RotateSphereEnemyState.IDLE;
            StartCoroutine(CheckState());
        }
    }

    protected override void Move()
    {
        
    }

    IEnumerator CheckState()
    {
        while(true)
        {
            if (rotateSphereEnemyState == RotateSphereEnemyState.IDLE)
            {
                
                rotateSphereEnemyState = RotateSphereEnemyState.CHASING;
            }
            else if (rotateSphereEnemyState == RotateSphereEnemyState.SEARCHING)
            {

            }
            else if (rotateSphereEnemyState == RotateSphereEnemyState.CHASING)
            {
                StartCoroutine(RotatingAttack());
                rotateSphereEnemyState = RotateSphereEnemyState.ATTACKING_ROTATING;
            }
            else if (rotateSphereEnemyState == RotateSphereEnemyState.ATTACKING_ROTATING)
            {
               
            }
            else if (rotateSphereEnemyState == RotateSphereEnemyState.DEAD)
            {
                rotateSphereEnemyState = RotateSphereEnemyState.IDLE;
                break;
            }
            yield return new WaitForSeconds(1f);
        }
        
    }

    IEnumerator RotatingAttack()
    {
        Debug.Log("하이루");
        rotateSphereEnemyState = RotateSphereEnemyState.ATTACKING_ROTATING;

        while(true)
        {            
            enemyAttack.LinearShooting(muzzle);

            var fromAngle = transform.rotation;
            var toAngle = Quaternion.Euler(transform.eulerAngles + rotateAngleBetweenAttack);
            for (float t = 0f; t < 1; t += Time.deltaTime / rotateTime)
            {

                //Debug.Log("Attack");
                //이거 하면 회전각이 정확한 대신 끊기면서 쏘는 느낌, 안하면 회전각이 부정확한 대신 부드럽게 쏘는 느낌
                //fromAngle = transform.rotation;
                transform.rotation = Quaternion.Lerp(fromAngle, toAngle, t);
                yield return null;
            }           

            //Debug.Log("projectile position: " + newProjectileObject.transform.position);          
        }  
    }    

}
