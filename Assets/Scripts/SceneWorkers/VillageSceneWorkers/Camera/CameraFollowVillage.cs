using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowVillage : MonoBehaviour
{
    public float xMax;
    public float xMin;   

    private Transform target;
    public float smoothSpeed = 0.125f;  
   
    void Start()
    {
        target = GameObject.Find("Player").transform;
    }
    
    void LateUpdate()
    {
        
        //transform.position = new Vector3(Mathf.Clamp(target.position.x, xMin, xMax),transform.position.y, transform.position.z);

        Vector3 desiredPosition = new Vector3(Mathf.Clamp(target.position.x, xMin, xMax), transform.position.y, transform.position.z);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
