using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    //Map
    public Vector2 mapSize;

    //Floor 생성 관련
    public FloorController floorController;

    //Fence 생성 관련    
    public FenceGenerator fenceGenerator;

    //
    public WallGenerator wallGenerator;

    private void Awake()
    {
        //영준막음
        PrepareMap(DataManager.Instance.GetPlayInfo.Stage);
        //PrepareMap(1);
    }

    // Start is called before the first frame update
    private void Start()
    {
        GenerateMap();
    } 

    //게임 켜질때 Map생성 , 게임 상점단계에서 Map생성
    public void PrepareMap(int stage)
    {      
        floorController.PrepareFloor(mapSize, stage);
        Vector2 mapSizeTmp = new Vector2(mapSize.x+2, mapSize.y+2);
        fenceGenerator.PrepareFence(mapSizeTmp,stage);
    }

    //FightScene 들어갈때 Map활성화
    public void GenerateMap()
    {
        // 벽 생성
        wallGenerator.GenerateWalls(DataManager.Instance.GetPlayInfo.Stage);

        floorController.GenerateFloor(mapSize);
        Vector2 mapSizeTmp = new Vector2(mapSize.x+2, mapSize.y+2);
        fenceGenerator.GenerateFence(mapSizeTmp);
    }
    

}
