using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void TakeHit(float damage);
    void TakeKnockBack(Vector3 dir, float force, float knockBackDuration);
}
