using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//RequireComponent : 요구되는 의존 컴포넌트 자동으로 이 스크립트를 추가한 게임오브젝트에 추가
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity
{
    //플레이어 상태
    public enum State { Idle, Attacking, Attacked, KnockBack };
    public float moveSpeed = 8;
    public bool isAttacking = false;
    public Animator animator;

    PlayerController controller;
    GunController gunController;
    Camera viewCamera;
    CooldownTimer cooldownTimer; // 스킬 쿨타임


    int count = 0;

    // Start is called before the first frame update
    public void Start()
    {
        controller = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();
        viewCamera = Camera.main;
        cooldownTimer = FindObjectOfType<CooldownTimer>(); // 스킬 쿨타임
        SetCooldownTimer();
    }

    private void SetCooldownTimer()
    {
        cooldownTimer.SetCooldownTime(5); // 임시 5초
        cooldownTimer.StartFight();
    }

    public void Move(Vector3 direction, float amount)
    {
        if(isKnockBack)
        {
            animator.SetBool("isMove", false);
        }
        else
        {
            Vector3 moveVelocity = direction.normalized * moveSpeed * amount;
            controller.Move(moveVelocity);
            if (!isAttacking)
            {
                controller.LookAt(transform.position + direction);
            }

            //Debug.Log("move" + direction);

            animator.SetBool("isMove", true);
        }
    }

    public void StopMove()
    {
        controller.Move(Vector3.zero);

        animator.SetBool("isMove", false); 
    }

    public void Attack(Vector3 direction)
    {
        //Debug.Log("attack" + direction);
        controller.LookAt(transform.position + direction);
        gunController.OnTirggerHold();
        //gunController.OnTriggerRelease(); // 연사를 위해 임시추가
    }
    public void SkillReady(Vector3 direction)
    {
        controller.LookAt(transform.position + direction);
    }
    public void SkillAttack(Vector3 direction)
    {
        controller.LookAt(transform.position + direction);

        if (cooldownTimer.IsSkillReady())
        {
            gunController.OnSkillTriggerHold();
            cooldownTimer.SkillUse();
        }
    }

    public void Standby()
    {
        gunController.OnTriggerRelease();
    }

    public WeaponType SwapWeapon()
    {
        return gunController.SwapWeapon();
    }


    public void PlayerDead()
    {
        StopMove();
        Standby();
        animator.SetTrigger("isDead");
    }

    public float GetAttackRange()
    {
        return gunController.CurrentWeaponRange();
    }
    public float GetAttackAngle()
    {
        return gunController.CurrentWeaponAngle();
    }
    public Vector3 GetMuzzlePosition()
    {
        return gunController.weaponHold.position;
    }

}
