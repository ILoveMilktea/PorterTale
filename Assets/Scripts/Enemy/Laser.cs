using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    private enum LaserState
    {
        MOVING,
        ARRIVED,
        HIT,
        END
    };

    //레이저 상태
    private LaserState laserState;

    //레이저 GroundFireTimer 코루틴
    private Coroutine groundFireTimerCoroutine;

    public LayerMask collisionMask;
    public float damage = 0;

    public bool useLaser = false;
    public LineRenderer lineRenderer;

    //땅에 끌리면서 나가는 laser 속도
    public float groundLaserSpeed = 3.0f;
    //public float groundFireLaserTime=3.0f;
    private float currentGroundFireLaserTime = 3.0f;

    private void Awake()
    {
        laserState = LaserState.MOVING;
    }

    public void Fire(Transform startPosition,GameObject target)
    {
        lineRenderer.SetPosition(0, startPosition.position);
        lineRenderer.SetPosition(1, target.transform.position);
    }

    public void GroundFire(Transform startPosition, GameObject target)
    {
        //Debug.Log("코루틴시작");
        Vector3 newTargetPosition = new Vector3(target.transform.position.x, 0, target.transform.position.z);
        StartCoroutine(CheckLaserState());
        groundFireTimerCoroutine=StartCoroutine(GroundFireTimer(startPosition.position,newTargetPosition));
    }

    IEnumerator GroundFireTimer(Vector3 startPosition, Vector3 targetPosition)
    {
        
        lineRenderer.enabled = true;

        //targetPosition.y = 0;        
        float elapsed = 0;
        Vector3 lerpPosition=new Vector3(-100,-100,-100);


        while (lerpPosition != targetPosition)
        {            
            elapsed += Time.deltaTime * groundLaserSpeed;
            lineRenderer.SetPosition(0, startPosition);
            lerpPosition = Vector3.Lerp(startPosition, targetPosition, elapsed);
            lerpPosition.y = 0;
            lineRenderer.SetPosition(1, lerpPosition);
            CheckCollision(startPosition, lerpPosition);

            if (laserState == LaserState.HIT)
            {        
                
                break;
            }

            yield return new WaitForEndOfFrame();
        }

        if(laserState!=LaserState.HIT)
        {
            laserState = LaserState.ARRIVED;
        }
        
        elapsed = 0;       
    }

    IEnumerator CheckLaserState()
    {
        while(true)
        {
            if(laserState==LaserState.MOVING) //레이저가 움직이는중
            {
                Debug.Log("레이저 움직이는중");
            }
            else if(laserState==LaserState.ARRIVED) //레이저가 안맞고 그냥 도착한 경우
            {
                Debug.Log("레이저 그냥 도착");
                laserState = LaserState.END;
            }
            else if(laserState==LaserState.HIT) //레이저가 타겟에 맞은 경우
            {
                Debug.Log("레이저맞음");
                //레이저 폭발 이펙트?
                laserState = LaserState.END;                
            }
            else if(laserState==LaserState.END)
            {
                Debug.Log("레이저끝");                
                laserState = LaserState.MOVING;
                break;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    //충돌 검사
    protected void CheckCollision(Vector3 startPosition, Vector3 targetPosition)
    {
        Vector3 dir=(targetPosition-startPosition);
        float rayDistance = Vector3.Distance(startPosition,targetPosition);
        //dir.y = 0.1f;
        //dir.y = 0;

        //Ray 생성
        Ray ray = new Ray(startPosition, dir);
        RaycastHit hit;

        //Ray 발사
        if (Physics.Raycast(ray, out hit, rayDistance, collisionMask, QueryTriggerInteraction.Collide))
        {
            Vector3 hitPosition = hit.collider.gameObject.transform.position;
            hitPosition.y = 0;
            lineRenderer.SetPosition(1,hitPosition);
            OnHitObject(hit);           
        }
        Debug.DrawRay(startPosition, dir, Color.red, 2.0f);
        
        
    }

    //물체와 부딪혔을 때 작동하는 함수
    protected void OnHitObject(RaycastHit hit)
    {
        laserState = LaserState.HIT;
        Collider collider = hit.collider;
        IDamageable damageableObject = collider.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.TakeHit(damage);
            
        }
    }
}
