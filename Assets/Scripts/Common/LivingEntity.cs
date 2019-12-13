using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//영준수정
public abstract class LivingEntity : MonoBehaviour,IDamageable
{
    public float startingHealth;
    protected float health;
    protected bool isDead=false;

    //KnockBack Timer   
    protected bool isKnockBack = false;

    // Start is called before the first frame update
    //protected virtual void Start()
    //{
    //    SetHealth();
    //}  

    public void SetHealth()
    {
        health = startingHealth;
    }

    public void TakeHit(float damage)
    {
        //영준막아놓음
        //FightSceneController.Instance.DamageToCharacter(gameObject, (int)damage); // UI & data
        Debug.Log("맞은애"+gameObject+"/"+"health"+(health-damage));
        //health -= damage;
        //Debug.Log("체력:"+health);
        //FightSceneController.Instance.DamageToCharacter(gameObject, (int)damage); // UI & data

        //if(health<=0 && !dead)
        //{
        //    Die();
        //}

    }

    public void TakeKnockBack(Vector3 dir, float force, float knockBackDuration)
    {
        if(isKnockBack==false)
        {
            //dir : 날라갈 방향
            isKnockBack = true;
            dir.y = 0;
            //GetComponent<Rigidbody>().AddForce(dir * force, ForceMode.VelocityChange);
            if (gameObject.activeSelf)
            {
                //StartCoroutine(KnockBackTimer(knockBackDuration));
                StartCoroutine(KnockBackTimer(dir, force, knockBackDuration));
            }
        }
        

    }

    public void StopKnockBack()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    public void Die()
    {
        isDead = true;
        StopAllCoroutines();
        FightSceneController.Instance.EnemyDead(gameObject);
    }

    IEnumerator KnockBackTimer(float knockBackDuration)
    {
        yield return new WaitForSeconds(knockBackDuration);
        StopKnockBack();
        isKnockBack = false;
    }
    IEnumerator KnockBackTimer(Vector3 dir, float force, float knockBackDuration)
    {
        float timer = 0f;
        Rigidbody rb = GetComponent<Rigidbody>();
        Vector3 startVelocity = dir.normalized * force;
        Vector3 velocity = startVelocity;


        while (timer < knockBackDuration)
        {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
            yield return new WaitForEndOfFrame();
            timer += Time.fixedDeltaTime;
            velocity = Vector3.Lerp(startVelocity, Vector3.zero, timer / knockBackDuration);
        }
        StopKnockBack();
        isKnockBack = false;
        //    yield return new WaitForSeconds(knockBackDuration);        
        //    StopKnockBack();
        //    isKnockBack = false;
    }
}
