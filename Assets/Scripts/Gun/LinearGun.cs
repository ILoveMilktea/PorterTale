using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearGun : Gun
{
    private void Awake()
    {
        SetProjectilePrefabName(Const_ObjectPoolName.LinearGun_Bullet);
    }

    override public void Shoot()
    {
        if (Time.time > nextShotTime)
        {
            //Time.time : 게임이 시작되고 지난 시간(초)
            nextShotTime = Time.time + msBetweenShots / 1000;

            //일직선 총

            if (fireMode == FireMode.AUTO)
            {

            }
            else if (fireMode == FireMode.SINGLE)
            {
                if (!triggerReleasedSinceLastShot)
                {
                    return;
                }
            }
            else if (fireMode == FireMode.BURST)
            {
                if (shotsRemainingInBurst == 0)
                {
                    return;
                }
                --shotsRemainingInBurst;
            }

            GameObject newProjectileObject = ObjectPoolManager.Instance.Get(projectilePrefabName);
            Transform projectileTransform = newProjectileObject.transform;
            //Debug.Log("muzzle position: " + muzzle.position);
            projectileTransform.position = muzzle.position;
            projectileTransform.rotation = muzzle.rotation;
            Projectile newProjectile = newProjectileObject.GetComponent<Projectile>();
            
            //Projectile newProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation) as Projectile;

            if (skillMode == SkillMode.GENERAL)
            {

            }
            else if (skillMode == SkillMode.SPECIAL)
            {

                //발사체 관통 활성화
                newProjectile.SetPentratingActive(true);
            }

            GameObject source = FindObjectOfType<Player>().gameObject;
            newProjectile.SetSource(source);
            //newProjectile.SetDamage(damage);
            newProjectile.SetMaxRange(maxRange);
            newProjectile.SetSpeed(muzzleVelocity);

            //------------- critical 적용
            float criticalDamage = damage + FightSceneController.Instance.GetPlayerATK();
            if(Random.Range(0,100) <= criticalChance)
            {
                criticalDamage = criticalDamage * criticalDamageRate;
            }
            newProjectile.SetDamage(criticalDamage);
            //------------- critical 적용

            //Debug.Log("projectile position: " + newProjectileObject.transform.position);

            newProjectileObject.SetActive(true);
           
        }

    }
    
    public override void SkillShoot()
    {      
        StartCoroutine(SkillShootCoroutine());
    }

    
    IEnumerator SkillShootCoroutine()
    {
        float timeBetweenShoot = 0.1f;
        FightSceneController.Instance.LockMoveRotate();
        FightSceneController.Instance.LockNormalAttack();

        shotsRemainingInBurst = burstCount;
        while (shotsRemainingInBurst != 0)
        {
            --shotsRemainingInBurst;

            GameObject newProjectileObject = ObjectPoolManager.Instance.Get(projectilePrefabName);
            Transform projectileTransform = newProjectileObject.transform;
            //Debug.Log("muzzle position: " + muzzle.position);
            projectileTransform.position = muzzle.position;
            projectileTransform.rotation = muzzle.rotation;
            Projectile newProjectile = newProjectileObject.GetComponent<Projectile>();

            //Projectile newProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation) as Projectile;


            //발사체 관통 활성화
            newProjectile.SetPentratingActive(true);

            GameObject source = FindObjectOfType<Player>().gameObject;
            newProjectile.SetSource(source);
            //newProjectile.SetDamage(damage);
            newProjectile.SetMaxRange(maxRange);
            newProjectile.SetSpeed(muzzleVelocity);

            //------------- critical 적용
            float criticalDamage = damage + FightSceneController.Instance.GetPlayerATK();
            if (Random.Range(0, 100) <= criticalChance)
            {
                criticalDamage = criticalDamage * criticalDamageRate;
            }
            newProjectile.SetDamage(criticalDamage);
            //------------- critical 적용

            //Debug.Log("projectile position: " + newProjectileObject.transform.position);

            newProjectileObject.SetActive(true);

            yield return new WaitForSeconds(timeBetweenShoot);
        }

        FightSceneController.Instance.UnLockMoveRotate();
        FightSceneController.Instance.UnLockNormalAttack();
    }
}
