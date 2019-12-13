using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorController : MonoBehaviour
{
    private FloorGenerator floorGenerator;
    private List<List<Transform>> floorPrefabList;
    private Vector2 mapSize;

    //Floor 공격 시간    
    public float timeBetweenFloorAttackMove = 0.5f;    
    public float timeBetweenFloorAttack = 2.0f;
    //Floor 공격 material  
    public Material attackMaterial;
    //Floor가 공격 중인지
    private bool isAttack = false;
    private bool isAttackWait = false;

    //이거 나중에 GameManager에서 Player좌표 바꾸는 것으로 바꿔야함
    private GameObject player;

    private void Awake()
    {
        floorGenerator = gameObject.GetComponent<FloorGenerator>();
        
        //수정-GameManager에서 Player정보 받아오기로 교체
        player = GameObject.FindWithTag("Player");        
    }

    private void OnEnable()
    {
        StartCoroutine(CheckAttackAvailableTimer());
    }

    public void PrepareFloor(Vector2 mapSize, int stage)
    {
        this.mapSize = mapSize;
        
        floorGenerator.PrepareFloor(mapSize, stage, out floorPrefabList);
    }

    public void GenerateFloor(Vector2 mapSize)
    {       
        floorGenerator.GenerateFloor(mapSize, ref floorPrefabList);
    }  

    public void Attack() // temp, 공격 막아둠
    {
        //IDamageable damageableObject = player.GetComponent<IDamageable>();
        //수정-서버로 보내는 공격 데미지 함수로 교체
        //damageableObject.TakeHit(0);
    }

    private Vector2 CheckAttackFloor()
    {
        Vector3 playerPosition = player.transform.position;
        Vector2 targetFloorIndex = new Vector2(-1, -1);

        for (int y = 0; y < mapSize.y; ++y)
        {
            for (int x = 0; x < mapSize.x; ++x)
            {
                Transform floorPrefab = floorPrefabList[y][x];
                float tmpPosX = floorPrefab.position.x;
                float tmpPosZ = floorPrefab.position.z;
                float tmpScaleX = floorPrefab.localScale.x / 2;
                float tmpScaleZ = floorPrefab.localScale.z / 2;
                float minX = tmpPosX - tmpScaleX;
                float maxX = tmpPosX + tmpScaleX;
                float minZ = tmpPosZ - tmpScaleZ;
                float maxZ = tmpPosZ + tmpScaleZ;
                if ((minX <= playerPosition.x && playerPosition.x <= maxX) && (minZ <= playerPosition.z && playerPosition.z <= maxZ))
                {
                    targetFloorIndex.x = x;
                    targetFloorIndex.y = y;
                    break;
                }
            }
        }

        return targetFloorIndex;
    }

    private bool CheckPlayerIsOnAttackFloor(Transform floorPrefab)
    {        
        float tmpPosX = floorPrefab.position.x;
        float tmpPosZ = floorPrefab.position.z;
        float tmpScaleX = floorPrefab.localScale.x / 2;
        float tmpScaleZ = floorPrefab.localScale.z / 2;
        float minX = tmpPosX - tmpScaleX;
        float maxX = tmpPosX + tmpScaleX;
        float minZ = tmpPosZ - tmpScaleZ;
        float maxZ = tmpPosZ + tmpScaleZ;
        float playerPositionX = player.transform.position.x;
        float playerPositionZ = player.transform.position.z;
        if ((minX <= playerPositionX && playerPositionX <= maxX) && (minZ <= playerPositionZ && playerPositionZ <= maxZ))
        {            
            return true;
        }
        return false;
    }

    //플레이어가 공격받을 수 있는 상황인지 확인
    IEnumerator CheckAttackAvailableTimer()
    {
        while(true)
        {
            if(isAttack==false)
            {
                if(isAttackWait==false)
                {
                    Vector2 targetFloorIndex = CheckAttackFloor();
                    //플레이어가 공격받을수 없는 경우(플레이어의 위치와 바닥이 일치하는게 없다면)
                    if (targetFloorIndex.x == -1 || targetFloorIndex.y == -1)
                    {

                    }
                    //플레이어가 공격받을수 있는 경우
                    else
                    {
                        isAttack = true;
                        StartCoroutine(AttackTimer(targetFloorIndex));
                    }
                }
                else
                {
                    yield return new WaitForSeconds(timeBetweenFloorAttack);
                    isAttackWait = false;
                }
               
            }
            yield return new WaitForSeconds(0.1f);
        }
    }


    IEnumerator AttackTimer(Vector2 targetFloorIndex)
    {
        //가로or세로 시작 랜덤
        int randomWidthOrHeightCase = Random.Range(0, 2);        
        //가로면 (왼쪽시작or오른쪽시작) / 세로면 (위시작or아래시작)
        int randomStartDirectionCase = Random.Range(0, 2);

        int fixY = -100;
        int fixX = -100;

        int startLine = -100;
        int startDirection = -100;
        int endDirection = -100;
        int variation = -100;

        //가로공격경우       
        if (randomWidthOrHeightCase==0)
        {
            //왼쪽시작경우
            if(randomStartDirectionCase==0)
            {
                startLine = 0;
                startDirection = 0;
                endDirection = (int)mapSize.x;
                fixY = (int)targetFloorIndex.y;
                fixX = startDirection;
                variation = 1;
            }
            //오른쪽시작경우
            else if(randomStartDirectionCase==1)
            {
                startLine = 0;
                startDirection = (int)mapSize.x-1;
                endDirection = -1;
                fixY = (int)targetFloorIndex.y;
                fixX = startDirection;
                variation = -1;
            }
            
        }
        //세로공격경우
        else if (randomWidthOrHeightCase==1)
        {
            //위에서 아래로 시작
            if(randomStartDirectionCase==0)
            {
                startLine = 1;
                startDirection = 0;
                endDirection = (int)mapSize.y;
                fixY = startDirection;
                fixX = (int)targetFloorIndex.x;
                variation = 1;
            }
            //아래에서 위로 시작
            else if(randomStartDirectionCase==1)
            {
                startLine = 1;
                startDirection = (int)mapSize.y-1;
                endDirection = -1;
                fixY = startDirection;
                fixX = (int)targetFloorIndex.x;
                variation = -1;
            }
            
        }     

        while(true)
        {
            //여기에서 Floor 공격처리하면됨
            //floorPrefabList[targetY][x].gameObject.SetActive(false);                   
            Transform currentFloorPrefab = floorPrefabList[fixY][fixX];
            Material beforeMaterial = currentFloorPrefab.GetComponent<MeshRenderer>().sharedMaterial;
            ChangeFloorColor(ref currentFloorPrefab);
            if(CheckPlayerIsOnAttackFloor(currentFloorPrefab)==true)
            {
                Attack();
            }
            yield return new WaitForSeconds(timeBetweenFloorAttackMove);
            RestoreFloorColor(ref currentFloorPrefab, ref beforeMaterial);

            //가로의 경우
            if(startLine==0)
            {                
                fixX += variation;
                if (fixX == endDirection)
                {                   
                    break;
                }
            }
            //세로의 경우
            else
            {              
                fixY += variation;
                if(fixY==endDirection)
                {                   
                    break;
                }
            }

            
        }

        isAttack = false;
        isAttackWait = true;
    }

    private void ChangeFloorColor(ref Transform currentFloorPrefab)
    {
        
        //currentFloorPrefab.GetComponent<MeshRenderer>().material = attackMaterial;
    }

    private void RestoreFloorColor(ref Transform currentFloorPrefab, ref Material beforeMaterial)
    {
        //currentFloorPrefab.GetComponent<MeshRenderer>().material = beforeMaterial;
    }
    
}

