using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEnemy : Enemy
{
    public Laser laser; 

    private void OnEnable()
    {
        SetHealth();
    }

    // Update is called once per frame
    void Update()
    {
        LockOnTarget();
        if(Input.GetKeyDown(KeyCode.Q))
        {            
            Attack();
        }
       
    }

    override protected void Move()
    {

    }

    void Attack()
    {
        laser.GroundFire(muzzle,target);
    }
}
