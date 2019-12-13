using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RushEnemy : Enemy
{
    private enum RushEnemyState
    {
        IDLE,
        SEARCHING,
        CHASING,
        RUSHING_START,
        RUSHING,
        RUSHING_MISS,
        RUSHING_HIT,        
        DEAD
    };

    //Enemy 상태
    private RushEnemyState rushEnemyState;

    public float attackDamage = 50.0f;
    //공격할 범위
    public float attackDistance = 5.0f;
    //추적할 범위
    public float chasingDistance = 8.0f;
    //공격간 간격
    public float timeBetweenAttack = 2.0f;
    //발사공격 코루틴
    private Coroutine rushTimerCoroutine;
    //Searching할 때 가속력
    public float acceleration = 10.0f;   
    //Rushing할 때 스피드
    public float rushSpeed;

    //Rush중인가 아닌가 상태
    private bool isRushAvailable = true;
    private bool isOnRush = false;
    private bool isRushHit = false;

    //Rushing할때 목표지점
    Vector3 rushDestination;    

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

    public NavMeshAgent navMeshAgent;

    //게임시작할때 몇초동안 기다렸다 Enemy상태 갱신하기
    private float waitTimeForStart = 2.0f;

    //Rushing Miss상태일때 Searching상태로 바로 넘어가는 거 방지
    private bool isRushTimerOn=false;

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
            rushEnemyState = RushEnemyState.IDLE;

            navMeshAgent.speed = moveSpeed;
            navMeshAgent.acceleration = acceleration;
            navMeshAgent.stoppingDistance = 0;

            //영준수정- 매니저에서 Map Size 받아오는걸로 변경
            mapSize = new Vector2(20, 20);
            
            //searching관련 값 미리 계산
            searchingRangeMaxX = mapSize.x * 0.5f - searchingOffset;
            searchingRangeMinX = mapSize.x * -0.5f + searchingOffset;
            searchingRangeMaxZ = mapSize.y * 0.5f - searchingOffset;
            searchingRangeMinZ = mapSize.y * -0.5f + searchingOffset;

            animator.SetBool("IsDead", false);

            StartCoroutine(CheckEnemyState());
            StartCoroutine(EnemyAction());            
        }               

    }

    private void OnDisable()
    {
        ResetValue();
    }

    protected override void Move()
    {
        //Debug.Log("mvoe중");
        //transform.position = Vector3.MoveTowards(transform.position, target.transform.position, moveSpeed * Time.deltaTime);
        if (navMeshAgent.isOnNavMesh)
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

        if(toSearchPosition.x+randomX>searchingRangeMaxX || toSearchPosition.x+randomX<searchingRangeMinX)
        {
            randomX = 0;
        }
        toSearchPosition.x += randomX;
        if (toSearchPosition.z + randomZ > searchingRangeMaxZ || toSearchPosition.z + randomZ <searchingRangeMinZ)
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

        //처음에 Search활성화 해주기위한 것
        searchingDestination = transform.position;
        
       
        while (!isDead && FightSceneController.Instance.GetCurrentFightState() != FightState.Dead)
        {
            
            if (health<=0) // isRushHit==true || 뺌
            {
                rushEnemyState = RushEnemyState.DEAD;
            }
            else
            {
                //Debug.Log("상태" + linearShootingEnemyState);
                float distance = Vector3.Distance(target.transform.position, transform.position);

                if (distance <= attackDistance && isRushAvailable == true && isOnRush == false)
                {
                    isSearchRestart = true;
                    navMeshAgent.speed = rushSpeed;
                    //navMeshAgent.acceleration = acc;               

                    //Debug.Log("적과의거리" + Vector3.Distance(transform.position, target.transform.position));    
                    isRushAvailable = false;
                    isOnRush = true;
                    rushEnemyState = RushEnemyState.RUSHING_START;

                }
                else if (distance <= attackDistance && isRushAvailable == false)
                {
                    if (isOnRush == true && isRushTimerOn == false)
                    {
                        rushEnemyState = RushEnemyState.RUSHING;
                        //if (isRushHit == true)
                        //{
                        //    isOnRush = false;
                        //    rushEnemyState = RushEnemyState.RUSHING_HIT;
                        //}
                    }
                    else
                    {
                        if (isRushHit == false)
                        {
                            rushEnemyState = RushEnemyState.RUSHING_MISS;
                        }
                    }
                }                
                else if(isRushTimerOn==false)
                {
                    navMeshAgent.speed = moveSpeed;
                    //searchingDestination = transform.position;
                    navMeshAgent.isStopped = false;
                    rushEnemyState = RushEnemyState.SEARCHING;
                    //navMeshAgent.stoppingDistance = 0;
                }
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
            Debug.Log("상태" + rushEnemyState);
            if (rushEnemyState == RushEnemyState.IDLE)
            {
                SetAllAnimationFalse();
            }
            else if (rushEnemyState == RushEnemyState.SEARCHING)
            {
                SetAllAnimationFalse();
                animator.SetBool("isWalking", true);
                Vector3 curPosition = transform.position;
                curPosition.y = 0;

                Vector3 searchPosition;
                if (isSearchRestart == true)
                {
                    isSearchRestart = false;
                    searchPosition = curPosition;
                }
                else
                {
                    searchPosition = searchingDestination;
                    searchPosition.y = 0;
                }

                
                if (curPosition == searchPosition)
                {                   
                    Searching();
                }
            }
            else if (rushEnemyState == RushEnemyState.CHASING)
            {
                //Chasing은 안쓸거
                //Move();
                //LockOnTarget();

            }
            else if (rushEnemyState == RushEnemyState.RUSHING_START) //Rush시작할때
            {
                SetAllAnimationFalse();
                animator.SetBool("isRunning", true);

                trajectoryLine = ObjectPoolManager.Instance.Get(Const_ObjectPoolName.TrajectoryLine).GetComponent<TrajectoryLine>();
                trajectoryLine.gameObject.SetActive(true);
                trajectoryLine.SetWidth(transform.lossyScale.z);
                StartCoroutine(trajectoryLine.DrawTrajectoryWhileTime(gameObject, target.transform.position, 1.0f));

                LockOnTarget();
                Rush();
            }
            else if (rushEnemyState == RushEnemyState.RUSHING) //Rush중
            {
                Vector3 enemyPosition = transform.position;
                enemyPosition.y = 0;
                Debug.Log("헤매기" + enemyPosition + "/" + rushDestination);
                //만약 적이 Rush목표지점에 도착했다면                
                if (enemyPosition == rushDestination)
                { 
                    //Rush쿨타임 기다리는 동안 타겟(플레이어)방향으로 회전하기          
                    isOnRush = false;                  
                      
                }
                
            }
            else if (rushEnemyState == RushEnemyState.RUSHING_MISS) //Rush했는데 아무것도 안 부딪혔을때
            {
                SetAllAnimationFalse();
                animator.SetBool("isStanding", true);
                LockOnTarget();
                if (rushTimerCoroutine == null)
                {
                    isOnRush = false;
                    rushTimerCoroutine = StartCoroutine(RushTimer());
                }                
            }
            else if(rushEnemyState==RushEnemyState.RUSHING_HIT) //Rush했는데 타겟에 맞았을때
            {
                //LockOnTarget();
                //if (rushTimerCoroutine==null)
                //{                    
                //    rushTimerCoroutine = StartCoroutine(RushTimer());
                //}
                
            }
            else if (rushEnemyState == RushEnemyState.DEAD)
            {
                
                isDead = true;
                //죽기전 애니메이션
                SetAllAnimationFalse();
                animator.SetBool("isDead", true);
                //죽는 애니메이션 3초동안 유지하고 없어짐
                yield return new WaitForSeconds(3.0f);
                
                ObjectPoolManager.Instance.Free(gameObject);   
                //Destroy(gameObject);
            }
            yield return new WaitForEndOfFrame();
        }
        SetAllAnimationFalse();
    }

    public void Rush()
    {        
        rushDestination = target.transform.position;
        rushDestination.y = 0;
        enemyAttack.Rushing(navMeshAgent, rushDestination);        
    }

    public void RushHit()
    {        
        IDamageable damageableObject = target.GetComponent<IDamageable>();
        //영준수정-이거 나중에 인자 없애줘야함
        //damageableObject.TakeHit(2);

        if (damageableObject != null)
        {
            enemyAttack.KnockBack(target);
            FightSceneController.Instance.DamageToCharacter(gameObject, target, attackDamage);
        }

        isRushHit = true;
    }

    IEnumerator RushTimer()
    {
        isRushTimerOn = true;
        yield return new WaitForSeconds(timeBetweenAttack);        
        isRushAvailable = true;
        //isRushHit = false;
        isOnRush = false;
        rushTimerCoroutine = null;
        isRushTimerOn = false;
    }

    private void OnTriggerEnter(Collider other)
    {
       
        if(isOnRush==true)
        {
            isOnRush = false;
            //부딪힌게 Player면
            if (other.CompareTag("Player"))
            {                
                RushHit();
            }
        }        
    }

    private void ResetValue()
    {
        isOnRush = false;
        isRushAvailable = true;        
        isRushHit = false;
        rushTimerCoroutine = null;
    }

    //모든 Animator 변수 false하기
    private void SetAllAnimationFalse()
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isStanding", false);
        animator.SetBool("isRunning", false);
        animator.SetBool("isDead", false);
        animator.SetBool("isAttacking", false);
    }

}
