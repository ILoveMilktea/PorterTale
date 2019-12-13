using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoystickAttack : JoystickBase
{
    private Player player;

    public Image equipingWeapon;
    private bool isSwap;

    private TrajectoryLine trajectoryLine;
    protected override void Start()
    {
        base.Start();
        isSwap = true;
        player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!handleLock)
        {
            switch (state)
            {
                case TouchState.Begin:
                    state = TouchState.Stay;
                    isSwap = true;
                    break;
                case TouchState.Stay:
                    Attack();
                    break;
                case TouchState.Drag:
                    state = TouchState.Stay;
                    Attack();
                    break;
                case TouchState.End:
                    Standby();
                    CheckSwap();
                    state = TouchState.None;
                    break;
                default:
                    break;
            }
        }
    }

    private void Attack()
    {
        Vector2 attackDirection = handle.position - border.position;
        attackDirection.Normalize();
        Vector3 moveDirection3D = new Vector3(attackDirection.x, 0, attackDirection.y);

        float moveAmount = Vector2.Distance(border.position, handle.position);
        moveAmount = moveAmount / handleMoveRange;

        if (moveAmount > 0.3f)
        {
            isSwap = false;
            // joystick handle의 이동 범위가 반을 넘어가야 움직이는거
            FightSceneController.Instance.LockMoveRotate();
            FightSceneController.Instance.PlayerAttack(moveDirection3D);

            if (trajectoryLine == null)
            {
                TurnOnTrajectoryLine();
            }

            if(equipingWeapon.sprite.name == WeaponType.ShotGun.ToString())
            {
                trajectoryLine.DrawLineWithAngle(player.GetMuzzlePosition(), moveDirection3D,
                    player.GetAttackRange(), player.GetAttackAngle());
            }
            else
            {
                trajectoryLine.DrawLineUntilRange(player.GetMuzzlePosition(), moveDirection3D,
                    player.GetAttackRange());
            }
        }
        else
        {
            Standby();

            if (trajectoryLine != null)
            {
                TurnOffTrajectoryLine();
            }
        }
    }

    private void Standby()
    {
        if(trajectoryLine != null)
        {
            TurnOffTrajectoryLine();
        }

        FightSceneController.Instance.PlayerStandby();
        FightSceneController.Instance.UnLockMoveRotate();
    }

    private void CheckSwap()
    {
        if (isSwap)
        {
            FightSceneController.Instance.SwapWeapon();
        }
        isSwap = false;
    }

    public void WeaponImageSwap(WeaponType weapon)
    {
        equipingWeapon.sprite = Resources.Load<Sprite>("Image/Weapon/" + weapon.ToString());
    }

    private void TurnOnTrajectoryLine()
    {
        trajectoryLine = ObjectPoolManager.Instance.Get(Const_ObjectPoolName.TrajectoryLine).GetComponent<TrajectoryLine>();
        trajectoryLine.gameObject.SetActive(true);
        trajectoryLine.SetColor("Player");
    }
    private void TurnOffTrajectoryLine()
    {
        trajectoryLine.RemoveLine();
        trajectoryLine = null;
    }

}
