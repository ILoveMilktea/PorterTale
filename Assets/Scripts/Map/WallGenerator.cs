using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGenerator : MonoBehaviour
{
    // 벽 생성은 적 생성하는 방식으로 했습니다.

    List<GameObject> walls;

    public void GenerateWalls(int stage)
    {
        walls = new List<GameObject>();
        Dictionary<int, StageWallInfo> table = StageWallTable.Instance.GetTable("Stage" + stage.ToString());

        foreach (var stageWallInfo in table.Values)
        {
            WallStatusInfo wallStatusInfo = Tables.Instance.WallStatus.GetTuple(stageWallInfo.m_serialNumber);
            GameObject wall = ObjectPoolManager.Instance.Get(wallStatusInfo.m_name);

            if(wallStatusInfo.m_name == Const_ObjectPoolName.Wall_Griffon_Damage)
            {
                wall.transform.position = new Vector3(stageWallInfo.m_posX, -0.9f, stageWallInfo.m_posY);
            }
            else
            {
                wall.transform.position = new Vector3(stageWallInfo.m_posX, 0, stageWallInfo.m_posY);
            }
            wall.SetActive(true);

            walls.Add(wall);
        }
        
    }

    public void DestroyWalls()
    {
        foreach(var wall in walls)
        {
            if(wall != null)
            {
                wall.SetActive(false);
            }
        }

        walls.Clear();
    }

    private void OnDestroy()
    {
        DestroyWalls();
    }
}
