using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotGun : Gun
{
   
    ////샷건 KnockBack Force
    //public float knockBackforce = 10.0f;

    ////발사체 날라가는 방향 개수
    //public int directionNumber = 3;
    ////발사체 나가는 최대각도
    //public float projectileMaxAngle = 120.0f;
    // -----------> Gun으로 이동

    //발사체를 발사한 Object
    private GameObject source;    

    // Start is called before the first frame update
    private void Awake()
    {
        SetProjectilePrefabName(Const_ObjectPoolName.ShotGun_Bullet);
        source = GameObject.FindGameObjectWithTag("Player");
    }

    override public void Shoot()
    {

        if (Time.time > nextShotTime)
        {
            //Time.time : 게임이 시작되고 지난 시간(초)
            nextShotTime = Time.time + msBetweenShots / 1000;


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

            float tmpAngle = 0f;
            if (directionNumber != 1)
            {
                tmpAngle = -projectileMaxAngle * 0.5f;
            }

            float variation = projectileMaxAngle / (directionNumber - 1);
            //날라갈 방향만큼 총알 각도 조절
            for (int i = 0; i < directionNumber; ++i)
            {
                GameObject newProjectileObject = ObjectPoolManager.Instance.Get(projectilePrefabName);
                Transform projectileTransform = newProjectileObject.transform;
                projectileTransform.position = muzzle.position;
                projectileTransform.rotation = muzzle.rotation;
                Projectile newProjectile = newProjectileObject.GetComponent<Projectile>();

                //GameObject source = FindObjectOfType<Player>().gameObject;
                newProjectile.SetSource(source);
                newProjectile.SetRotation(Quaternion.Euler(newProjectile.transform.eulerAngles + new Vector3(0, tmpAngle, 0)));
                tmpAngle += variation;

                //일반 모드일때 총알 세팅
                newProjectile.SetMaxRange(maxRange);
                newProjectile.SetSpeed(muzzleVelocity);
                newProjectile.SetKnockBackMode(true);
                newProjectile.SetKnockBackForce(knockBackForce);
                newProjectile.SetKnockBackDuration(KnockBackDuration);
                newProjectile.SetKnockBackMuzzlePosition(muzzle.position);

                //------------- critical 적용
                float criticalDamage = damage + FightSceneController.Instance.GetPlayerATK();
                if (Random.Range(0, 100) <= criticalChance)
                {
                    criticalDamage = criticalDamage * criticalDamageRate;
                }
                newProjectile.SetDamage(criticalDamage);
                //------------- critical 적용

                // -- 속도 서서히 감속
                newProjectile.SetSpeedChangeMode(true);
                newProjectile.SetSpeedChangingRatio(0.7f);
                newProjectile.SetSpeedAfterChange(muzzleVelocity * 0.5f);
                // -- 속도 서서히 감속
                newProjectileObject.SetActive(true);
            }

        }
    }

    public override void SkillShoot()
    {
        // special, burst 모드로 한번 쏘게 해주십셔

        float tmpAngle = 0f;
        if (directionNumber != 1)
        {
            tmpAngle = -projectileMaxAngle * 0.5f;
        }

        float variation = projectileMaxAngle / (directionNumber * 2 - 1);
        //날라갈 방향만큼 총알 각도 조절
        for (int i = 0; i < directionNumber * 2; ++i)
        {
            if (i == directionNumber)
            {
                // 묻고 더블로 가!
                tmpAngle = -projectileMaxAngle * 0.5f + variation * 0.5f;
            }

            GameObject newProjectileObject = ObjectPoolManager.Instance.Get(projectilePrefabName);
            Transform projectileTransform = newProjectileObject.transform;
            projectileTransform.position = muzzle.position;
            projectileTransform.rotation = muzzle.rotation;
            Projectile newProjectile = newProjectileObject.GetComponent<Projectile>();

            //GameObject source = FindObjectOfType<Player>().gameObject;
            newProjectile.SetSource(source);
            newProjectile.SetRotation(Quaternion.Euler(newProjectile.transform.eulerAngles + new Vector3(0, tmpAngle, 0)));
            tmpAngle += variation;

            //스킬 모드일때 총알 세팅
            //총알 계속 나가는 거는 MaxRange가 1000정도면 벽 닿을때까지 안없어지고, 벽 닿으면 총알 없어져서 이렇게 했습니다
            newProjectile.SetMaxRange(1000);
            newProjectile.SetSpeed(muzzleVelocity);
            newProjectile.SetKnockBackMode(true);
            newProjectile.SetKnockBackForce(knockBackForce);
            newProjectile.SetKnockBackMuzzlePosition(muzzle.position);

            //------------- critical 적용
            float criticalDamage = damage + FightSceneController.Instance.GetPlayerATK();
            if (Random.Range(0, 100) <= criticalChance)
            {
                criticalDamage = criticalDamage * criticalDamageRate;
            }
            newProjectile.SetDamage(criticalDamage);
            //------------- critical 적용

            newProjectileObject.SetActive(true);
        }
    }

    //아직 지우지 마세요~
    //private bool CheckFanShapedRange(Transform target)
    //{
    //    //총구 위치
    //    Transform tmp = projectileSpawnTransform;
    //    //총구 벡터
    //    Vector3 sourceVec = tmp.position;
    //    sourceVec.y = 0;
    //    //총구 ~ Target의 방향 벡터
    //    Vector3 targetVec = target.transform.position - sourceVec;
    //    targetVec.y = 0;

    //    //총구의 로컬벡터
    //    Vector3 tmpLocalPosition = tmp.localPosition;
    //    //Debug.Log("localPosition" + tmpLocalPosition);

    //    //총구의 로컬벡터를 월드벡터로 변환(방향)
    //    Vector3 convertTmpLocalDirWorldDir = transform.TransformDirection(tmpLocalPosition);
    //    convertTmpLocalDirWorldDir.y = 0;

    //    //변환한 총구 방향 벡터와 (총구~Target의 방향 벡터) 간의 각도
    //    float angle = Vector3.Angle(targetVec, convertTmpLocalDirWorldDir);        
    //    Debug.Log("각도:"+angle);

    //    Vector3 debugVectorLeft = Quaternion.Euler(0, -shotGunRange, 0) * convertTmpLocalDirWorldDir*100;
    //    Vector3 debugVectorRight = Quaternion.Euler(0, shotGunRange, 0) * convertTmpLocalDirWorldDir*100;

    //    Debug.DrawLine(transform.TransformPoint(tmpLocalPosition), debugVectorLeft*100, Color.red,1.0f);
    //    Debug.DrawLine(transform.TransformPoint(tmpLocalPosition), debugVectorRight* 100, Color.red, 1.0f);        

    //    //샷건 범위각도안에 들어오면 
    //    if (angle<=shotGunRange)
    //    {
    //        return true;
    //    }

    //    return false;
    //}   

}
