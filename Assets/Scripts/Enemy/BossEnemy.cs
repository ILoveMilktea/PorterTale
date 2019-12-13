using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : Enemy
{
    private enum BossEnemyState
    {
        IDLE,
        ATTACKING_LINEARSHOOTING,
        ATTACKING_ROTATINGSHOOTING,
        ATTACKING_SPREADSHOOTING,
        DEAD
    };

    //공격할 때 회전 시간 간격
    public float rotateTime;
    //공격할 때 회전 간격(몇 도 마다 공격할지)
    public Vector3 rotateAngleBetweenAttack;

    //공격간 시간 간격
    //public float timeBetweenAttack;
    //공격할 수 있는지 
    private bool isAttackAvailable = true;
    //공격중인지
    private bool isAttackWait = false;


    //각 공격스킬 당 시간간격
    public float timeBetweenAttack;
    //공격 스킬 코루틴
    private Coroutine attackSkillCoroutine;

    public float timeBetweenLinearShooting = 0.7f;
    public float timeBetweenSpreadShooting = 0.7f;

    private float deadAnimationLastingTime = 2.0f;

    //RotatingAttack 공격 시간
    public float rotatingAttackLastingTime;

    //상태
    private BossEnemyState bossEnemyState;

    //게임시작할때 몇초동안 기다렸다 Enemy상태 갱신하기
    private float waitTimeForStart = 2.0f;

    //[Animator관련 변수]
    //Animator 변수
    private Animator animator;

    private bool isSpreadShootingAttackMode = false;
    private bool isLinearShootingAttackMode = true;
    private bool isRotateShootingAttackMode = false;

    //spreadshooting한번에 몇번 쏘게 할건지
    private int spreadShootingBurstCountMin=3;
    private int spreadShootingBurstCountMax = 6;

    //linearshooting한번에 몇번 쏘게 할건지
    private int linearShootingBurstCountMin = 3;
    private int linearShootingBurstCountMax = 5;


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
            target = FindObjectOfType<Player>().gameObject;
            animator = this.gameObject.GetComponent<Animator>();

            SetHealth();
            isDead = false;
            bossEnemyState = BossEnemyState.IDLE;

            StartCoroutine(CheckEnemyState());
            StartCoroutine(EnemyAction());
        }
    }

    private void OnDisable()
    {
        isAttackAvailable = true;
        isAttackWait = false;
        attackSkillCoroutine = null;
    }

    protected override void Move()
    {

    }

    IEnumerator CheckEnemyState()
    {      
        //맵 완전히 켜질때까지 기다리기
        yield return new WaitForSeconds(waitTimeForStart);

        float halfHealth = startingHealth * 0.5f;
        //시작하고 3초간 가만히 있기
        //yield return new WaitForSeconds(3.0f);
        while (!isDead)
        {
            if (health == 0)
            {
                bossEnemyState = BossEnemyState.DEAD;
            }
            else
            {
                if (isAttackAvailable == false)
                {
                    if (isAttackWait == true)
                    {
                        LockOnTarget();
                    }
                    bossEnemyState = BossEnemyState.IDLE;
                }
                else if (isLinearShootingAttackMode && isAttackAvailable == true)
                {
                    bossEnemyState = BossEnemyState.ATTACKING_LINEARSHOOTING;
                }
                else if (isSpreadShootingAttackMode && isAttackAvailable == true)
                {
                    bossEnemyState = BossEnemyState.ATTACKING_SPREADSHOOTING;
                }
                else if (isRotateShootingAttackMode && isAttackAvailable == true)
                {
                    bossEnemyState = BossEnemyState.ATTACKING_ROTATINGSHOOTING;
                }

            }
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator EnemyAction()
    {
        //맵 완전히 켜질때까지 기다리기
        yield return new WaitForSeconds(waitTimeForStart);

        while (!isDead)
        {

            if (bossEnemyState == BossEnemyState.IDLE)
            {
                SetAllAnimationFalse();
                LockOnTarget();
            }
            else if (bossEnemyState == BossEnemyState.ATTACKING_LINEARSHOOTING)
            {
                LockOnTarget();
                SetAllAnimationFalse();
                animator.SetBool("isAttacking_Skill2", true);
                if (attackSkillCoroutine == null)
                {

                    attackSkillCoroutine = StartCoroutine(LinearShooting());
                }
            }
            else if (bossEnemyState == BossEnemyState.ATTACKING_SPREADSHOOTING)
            {
                LockOnTarget();
                SetAllAnimationFalse();
                animator.SetBool("isAttacking_Skill1", true);
                if (attackSkillCoroutine == null)
                {

                    attackSkillCoroutine = StartCoroutine(SpreadShooting());
                }
            }
            else if (bossEnemyState == BossEnemyState.ATTACKING_ROTATINGSHOOTING)
            {
                SetAllAnimationFalse();
                animator.SetBool("isAttacking_Skill2", true);
                if (attackSkillCoroutine == null)
                {

                    attackSkillCoroutine = StartCoroutine(RotatingAttack());
                }
            }

            else if (bossEnemyState == BossEnemyState.DEAD)
            {
                isDead = true;
                SetAllAnimationFalse();
                animator.SetBool("isDead", true);
                //죽는 애니메이션 2초동안 유지하고 없어짐
                yield return new WaitForSeconds(deadAnimationLastingTime);

                ObjectPoolManager.Instance.Free(gameObject);
            }

            yield return new WaitForEndOfFrame();
        }
    }

    //부채꼴형태로 발사체 공격
    IEnumerator SpreadShooting()
    {
        int count = Random.Range(spreadShootingBurstCountMin, spreadShootingBurstCountMax);
        //SpreadShooting스킬에서 한번 발사간 시간 간격
        
        while (count != 0)
        {
            LockOnTarget();
            count--;
            enemyAttack.SpreadShooting(muzzle);

            yield return new WaitForSeconds(timeBetweenSpreadShooting);
        }

        isSpreadShootingAttackMode = false;
        isRotateShootingAttackMode = true;

        StartCoroutine(AttackWaitTimer());
    }

    //회전하면서 발사체 공격
    IEnumerator RotatingAttack()
    {
        float totalRotateTime = 0f;
        enemyAttack.SetDirectionNumber(1);
        enemyAttack.SetSpeedChangeMode(false);
        while (true)
        {
            //RotatignAttack 공격 지속 시간이 지나면
            if (totalRotateTime >= rotatingAttackLastingTime)
            {
                break;
            }
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
            totalRotateTime += rotateTime;
            //Debug.Log("projectile position: " + newProjectileObject.transform.position);          
        }

        isRotateShootingAttackMode = false;
        isLinearShootingAttackMode = true;

        StartCoroutine(AttackWaitTimer());
    }

    IEnumerator LinearShooting()
    {
        int count = Random.Range(linearShootingBurstCountMin, linearShootingBurstCountMax);
       
        enemyAttack.SetDirectionNumber(3);
        enemyAttack.SetSpeedChangeMode(true);
        while (count != 0)
        {

            count--;
            enemyAttack.LinearShooting(muzzle);

            yield return new WaitForSeconds(timeBetweenLinearShooting);

        }
        isLinearShootingAttackMode = false;
        isSpreadShootingAttackMode = true;

        StartCoroutine(AttackWaitTimer());
    }

    IEnumerator AttackWaitTimer()
    {
        isAttackWait = true;
        isAttackAvailable = false;
        yield return new WaitForSeconds(timeBetweenAttack);
        isAttackWait = false;
        isAttackAvailable = true;
        attackSkillCoroutine = null;
    }

    //모든 Animator 변수 false하기
    private void SetAllAnimationFalse()
    {        
        animator.SetBool("isRunning", false);        
        animator.SetBool("isAttacking_Skill1", false);
        animator.SetBool("isAttacking_Skill2", false);
        //animator.SetBool("isHurt", false);
        animator.SetBool("isDead", false);
    }

}
