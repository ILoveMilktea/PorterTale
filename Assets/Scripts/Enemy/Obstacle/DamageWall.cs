using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageWall : MonoBehaviour
{
    public float knockBackForce = 5.0f;
    public float knockBackDuration = 1.0f;


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == Constants.PlayerTag)
        {
            Vector3 dir = collision.transform.position - transform.position;
            dir.y = 0;
            dir.Normalize();

            collision.gameObject.GetComponent<IDamageable>().TakeKnockBack(dir, knockBackForce, knockBackDuration);
            FightSceneController.Instance.DamageToCharacter(collision.gameObject, 5);
        }
    }
}
