using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    Vector3 velocity;
    Rigidbody myRigidbody;
    
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        //myRigidbody.MovePosition(myRigidbody.position + velocity * Time.fixedDeltaTime);

    }

    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
        myRigidbody.MovePosition(myRigidbody.position + velocity * Time.fixedDeltaTime);
    }

    public void LookAt(Vector3 lookPoint)
    {
        Vector3 heightCorrectedPoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
        //Debug.Log("dir : " + lookPoint);
        //Debug.Log("change : " + heightCorrectedPoint);
        transform.LookAt(heightCorrectedPoint);
    }
}
