using UnityEngine;
using System.Collections;

public class ProjectileEffect : MonoBehaviour
{
    public GameObject impactParticle; // Effect spawned when projectile hits a collider
    public GameObject muzzleParticle; // Effect instantly spawned when gameobject is spawned

    public void FireEffect(Vector3 firePoint)
    {
        ParticleManager.Instance.OnParticle(muzzleParticle.name, 2.0f, firePoint);
    }

    public void HitEffect(Vector3 hitPoint)
    {
        if(impactParticle)
        {
            ParticleManager.Instance.OnParticle(impactParticle.name, 3.5f, hitPoint);
        }
    }
}