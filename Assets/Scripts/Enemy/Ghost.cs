using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : Enemy
{
    public float frequency = 20f;
    public float magnitude = 0.5f;

    bool facingRight = true;

    Vector3 pos;
    Vector3 localScale;
    //public GameObject target;

    Vector3 startPosition;

    //// Start is called before the first frame update
    //protected override void Start()
    //{
    //    base.Start();
    //    //pos = transform.position;
    //    //localScale = transform.localScale;
    //    startPosition = transform.position;
    //}

    protected override void Move()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        //transform.LookAt(target.transform);
        float x = Mathf.Cos(Time.time * frequency) * magnitude;
        float y=startPosition.y;
        float z= Mathf.Sin(Time.time * frequency) * magnitude;

        transform.position = new Vector3(x, y, z);
        //CheckWhereToFace();

        //if (facingRight)
        //    MoveRight();
        //else
        //    MoveLeft();

        //transform.LookAt(target.transform);
    }

    private void CheckWhereToFace()
    {
        if(pos.x<-7f)
        {
            facingRight = true;
        }
        else if(pos.x>7f)
        {
            facingRight = false;
        }

        //if(((facingRight)&&(localScale.x<0))||((!facingRight)&&(localScale.x>0)))
        //{
        //    localScale.x *= -1;
        //}
        //transform.localScale = localScale;

        
    }

    void MoveRight()
    {
        pos += transform.right * Time.deltaTime * moveSpeed;
        transform.position = pos + Vector3.forward * Mathf.Sin(Time.time * frequency) * magnitude;

    }

    void MoveLeft()
    {
        pos -= transform.right * Time.deltaTime * moveSpeed;
        transform.position = pos + Vector3.forward * Mathf.Sin(Time.time * frequency) * magnitude;
    }
}
