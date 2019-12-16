using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//움직이면서 쏘는 적
[RequireComponent(typeof(EnemyAttack))]
public class LinearShootingEnemy : Enemy
{   
    private enum LinearShootingEnemyState
    {
        IDLE,
        SEARCHING,
        CHASING,
        ATTACKING_SHOOTING,
        DEAD
    };

    //Enemy 상태
    private LinearShootingEnemyState linearShootingEnemyState;

    //공격할 범위
    public float attackDistance = 5.0f;
    //추적할 범위
    public float chasingDistance = 8.0f;
    //공격간 간격
    public float timeBetweenAttack=2.0f;
    //발사공격 코루틴
    private Coroutine shootingCoroutine;
    //가속력
    public float acceleration = 4f;
    //가속력 (멈출때)
    public float deacceleration = 60f;    

    public NavMeshAgent navMeshAgent;

    //맵 사이즈 캐싱
    private Vector2 mapSize;
    //search관련 여부
    private bool isSearchRestart = false;
    //Searching할 때 목표지점
    private Vector3 searchingDestination;
    //Search할 때 너무 구석으로 안가기 위한 offset
    public float searchingOffset;
    //Search 발동할때 움직일 범위(현재 위치 기준으로 얼마나 Search하러갈지)
    public int searchRangeMin;
    public int searchRangeMax;
    //Search할때 범위
    private float searchingRangeMaxX;
    private float searchingRangeMinX;
    private float searchingRangeMaxZ;
    private float searchingRangeMinZ;

    //게임시작할때 몇초동안 기다렸다 Enemy상태 갱신하기
    private float waitTimeForStart = 2.0f;

    // Trajectory, 공격 궤적
    private TrajectoryLine trajectoryLine;

    //[Animator관련 변수]
    //Animator 변수
    private Animator animator;

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
            navMeshAgent = GetComponent<NavMeshAgent>();
            animator = this.gameObject.GetComponent<Animator>();

            SetHealth();
            isDead = false;
            linearShootingEnemyState = LinearShootingEnemyState.IDLE;

            //
            navMeshAgent.speed = moveSpeed;
            navMeshAgent.acceleration = 2f;
            navMeshAgent.stoppingDistance = attackDistance;

            //영준수정- 매니저에서 Map Size 받아오는걸로 변경
            mapSize = new Vector2(20, 20);
            //처음에 Search활성화 해주기위한 것
            searchingDestination = transform.position;
            //searching관련 값 미리 계산
            searchingRangeMaxX = mapSize.x * 0.5f - searchingOffset;
            searchingRangeMinX = mapSize.x * -0.5f + searchingOffset;
            searchingRangeMaxZ = mapSize.y * 0.5f - searchingOffset;
            searchingRangeMinZ = mapSize.y * -0.5f + searchingOffset;


            animator.SetBool("IsDead", false);

            StartCoroutine(EnemyAction());
            StartCoroutine(CheckEnemyState());
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        ResetValue();
    }

    protected override void Move()
    {
        //Debug.Log("mvoe중");
        //transform.position = Vector3.MoveTowards(transform.position, target.transform.position, moveSpeed * Time.deltaTime);
        if(navMeshAgent.isOnNavMesh)
        {
           
            navMeshAgent.SetDestination(target.transform.position);            
            
        }
       
    }   

    private void Searching()
    {       
        Vector3 toSearchPosition = transform.position;
        toSearchPosition.y = 0;
        float randomX = Random.Range(searchRangeMin, searchRangeMax);
        float randomZ = Random.Range(searchRangeMin, searchRangeMax);

        if (toSearchPosition.x + randomX > searchingRangeMaxX || toSearchPosition.x + randomX < searchingRangeMinX)
        {
            randomX = 0;
        }
        toSearchPosition.x += randomX;
        if (toSearchPosition.z + randomZ > searchingRangeMaxZ || toSearchPosition.z + randomZ < searchingRangeMinZ)
        {
            randomZ = 0;
        }
        toSearchPosition.z += randomZ;

        searchingDestination = toSearchPosition;
        navMeshAgent.SetDestination(searchingDestination);

    }

    IEnumerator CheckEnemyState()
    {
        //맵 완전히 켜질때까지 기다리기
        yield return new WaitForSeconds(waitTimeForStart);

        while (!isDead && FightSceneController.Instance.GetCurrentFightState() != FightState.Dead)
        {
            //Debug.Log("상태" + linearShootingEnemyState);

            float distance = Vector3.Distance(target.transform.position, transform.position);

            if(distance<=attackDistance)
            {
                navMeshAgent.acceleration = deacceleration;
                navMeshAgent.isStopped = true;
                //Debug.Log("적과의거리" + Vector3.Distance(transform.position, target.transform.position));
                //transform.position = transform.position;
                linearShootingEnemyState = LinearShootingEnemyState.ATTACKING_SHOOTING;
            }
            else if(distance<=chasingDistance)
            {                
                navMeshAgent.stoppingDistance = attackDistance;
                navMeshAgent.acceleration = acceleration;
                navMeshAgent.isStopped = false;
                linearShootingEnemyState = LinearShootingEnemyState.CHASING;
            }
            else
            {
                searchingDestination = transform.position;
                navMeshAgent.isStopped = false;                
                linearShootingEnemyState = LinearShootingEnemyState.SEARCHING;
                navMeshAgent.stoppingDistance = 0;
            }
            yield return new WaitForEndOfFrame();
        }
        
    }
    

    IEnumerator EnemyAction()
    {
        //맵 완전히 켜질때까지 기다리기
        yield return new WaitForSeconds(waitTimeForStart);

        while (!isDead && FightSceneController.Instance.GetCurrentFightState() != FightState.Dead)
        {
            

            if (linearShootingEnemyState == LinearShootingEnemyState.IDLE)
            {
                
            }
            else if (linearShootingEnemyState == LinearShootingEnemyState.SEARCHING)
            {
                SetAllAnimationFalse();
                animator.SetBool("isWalking", true);

                Vector3 curPosition = transform.position;
                curPosition.y = 0;
                Vector3 searchPosition = searchingDestination;
                searchPosition.y = 0;
                //Debug.Log("searching할까" + curPosition + "/" + searchPosition + "/" + navMeshAgent.isStopped);

                if (curPosition == searchPosition)
                {
                    Searching();
                }

            }
            else if (linearShootingEnemyState == LinearShootingEnemyState.CHASING)
            {
                SetAllAnimationFalse();
                animator.SetBool("isRunning", true);

                if (shootingCoroutine!=null)
                {
                    StopCoroutine(shootingCoroutine);
                    shootingCoroutine = null;

                    //if(trajectoryLine != null)
                    //{
                    //    trajectoryLine.RemoveLine();
                    //    trajectoryLine = null;
                    //}
                }
                Move();
                LockOnTarget();
                //Debug.Log("상태" + linearShootingEnemyState);
            }
            else if (linearShootingEnemyState == LinearShootingEnemyState.ATTACKING_SHOOTING)
            {
                SetAllAnimationFalse();
                animator.SetBool("isShaking", true);

                LockOnTarget();
                if(shootingCoroutine == null)
                {
                    shootingCoroutine =StartCoroutine(Shooting());

                    // 궤적 추가
                    //trajectoryLine = ObjectPoolManager.Instance.Get(Const_ObjectPoolName.TrajectoryLine).GetComponent<TrajectoryLine>();
                    //trajectoryLine.gameObject.SetActive(true);
                    //StartCoroutine(trajectoryLine.DrawTrajectoryWhileInterrupt(gameObject, target.gameObject));
                }
                //Debug.Log("상태" + linearShootingEnemyState);
            }
            else if (linearShootingEnemyState == LinearShootingEnemyState.DEAD)
            {

                isDead = true;
                SetAllAnimationFalse();
                animator.SetBool("isDead", true);
                //죽는 애니메이션 3초동안 유지하고 없어짐
                yield return new WaitForSeconds(3.0f);

                ObjectPoolManager.Instance.Free(gameObject);
                break;
            }
            yield return new WaitForEndOfFrame();
        }

        SetAllAnimationFalse();
    }

    IEnumerator Shooting()
    {
        //Debug.Log("shooting코루틴");
        while (!isDead)
        {
            enemyAttack.LinearShooting(muzzle);
            yield return new WaitForSeconds(timeBetweenAttack);
        }
    }

    private void ResetValue()
    {
        isSearchRestart = false;
    }

    //모든 Animator 변수 false하기
    private void SetAllAnimationFalse()
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isShaking", false);
        animator.SetBool("isRunning", false);
        animator.SetBool("isDead", false);        
    }

}
