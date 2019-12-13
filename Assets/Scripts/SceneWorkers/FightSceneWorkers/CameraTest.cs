using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTest : MonoBehaviour
{
    private Vector3 distance;
    private float d_y;
    private float d_z;

    public Transform player;
    public Camera mainCamera;

    private float bottomSpace;
    private float topSpace;
    private float mapHeight;

    // Start is called before the first frame update
    void Start()
    {
        // 카메라 각도 : 60, 카메라 FOV : 25, y축거리 : 27, z축거리 : y * tan30
        distance = new Vector3(10.5f, 28f, 27 * Mathf.Tan(Mathf.PI * 30 / 180) * (-1));
        d_y = distance.y - 1;
        d_z = Mathf.Abs(distance.z);

        //bottom
        float angle = 90 - mainCamera.transform.eulerAngles.x - (mainCamera.fieldOfView * 2 / 5);
        float len = (d_y+1) * Mathf.Tan(Mathf.PI * angle / 180);
        bottomSpace = Mathf.Abs(distance.z) - len;
        bottomSpace += 1f;

        //bottomSpace = mainCamera.orthographicSize * Mathf.Sin(Mathf.PI * (mainCamera.transform.eulerAngles.x / 180));
        //bottomSpace -= 1;

        //top
        topSpace = d_y - d_z;
        topSpace -= 2f;

        //float lenX = Mathf.Abs(distance.z) + player.position.y * Mathf.Tan(Mathf.PI * ((90 - mainCamera.transform.eulerAngles.x) / 180));
        //topSpace = bottomSpace + 2 * lenX;

        mapHeight = 42f;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 next = player.transform.position + distance;
        next.x = distance.x;

        transform.position = next;

        if (transform.position.z < bottomSpace - d_z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, bottomSpace - d_z);
        }
        else if (transform.position.z > mapHeight - topSpace - d_z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, mapHeight - topSpace - d_z);
        }

    }
}
