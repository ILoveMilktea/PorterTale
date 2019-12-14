using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//발사무기 클래스
public abstract class Gun : MonoBehaviour
{
    //Auto : 자동(계속 연발)
    //Burst : 한번에 n발 연속으로 발사
    //Singe : 단발
    //public enum GunMode { LINEAR, ENERGYSPHERE, SHOTGUN};
    public enum FireMode { AUTO, SINGLE, BURST};
    public enum SkillMode { GENERAL, SPECIAL};

    //public GunMode gunMode;
    public FireMode fireMode;
    public SkillMode skillMode;
    // Active skill check
    public bool isSkillEquiped = false;
    public string skillKey;

    //총구
    public Transform muzzle;
    //발사체
    public string projectilePrefabName;
    //발사시간간격(ms)
    public float msBetweenShots = 500.0f;
    //발사체의 속도
    public float muzzleVelocity = 20.0f;
    //총 사정거리
    public float maxRange=10.0f;
    //총 데미지 ------> int로 변경
    public int damage = 1;
    //burst모드일때 한번에 최대 몇개쏠수 있는지
    public int burstCount;

    // ------------> shotgun에서 이동
    //샷건 KnockBack Force
    public float knockBackForce = 10.0f;
    public float KnockBackDuration = 0.5f;

    //발사체 날라가는 방향 개수
    public int directionNumber = 3;
    //발사체 나가는 최대각도
    public float projectileMaxAngle = 120.0f;

    // ------------> energysphere
    // 크리티컬 추가함
    public float criticalChance = 0f;
    public float criticalDamageRate = 1.5f;


    //다음 발사 시간 계산
    protected float nextShotTime=0;

    protected bool triggerReleasedSinceLastShot=true;
    protected int shotsRemainingInBurst=0;

    public abstract void Shoot();
    public abstract void SkillShoot();

    public void OnTriggerHold()
    {       
        Shoot();
        triggerReleasedSinceLastShot = false;
    }

    // 스킬 공격 추가
    public void OnSkillTriggerHold()
    {
        SkillShoot();
    }


    public void OnTriggerRelease()
    {
        triggerReleasedSinceLastShot = true;
        shotsRemainingInBurst = burstCount;
    }

    protected void SetProjectilePrefabName(string name)
    {
        projectilePrefabName = name;
    }
}
