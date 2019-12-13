using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JoystickSkill : JoystickBase
{
    private Player player;

    public Sprite defaultSkillImage;
    public bool isSkillEquiped = false;

    private TrajectoryLine trajectoryLine;
    protected override void Start()
    {
        base.Start();

        player = FindObjectOfType<Player>();
    }
    public override void OnPointerUp(PointerEventData data)
    {
        state = TouchState.End;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (state)
        {
            case TouchState.Begin:
                state = TouchState.Stay;
                break;
            case TouchState.Stay:
                ReadySkill();
                break;
            case TouchState.Drag:
                ReadySkill();
                state = TouchState.Stay;
                break;
            case TouchState.End:
                UseSkill();
                Standby();
                state = TouchState.None;
                break;
            default:
                break;
        } 
    }

    private void ReadySkill()
    {
        Vector2 attackDirection = handle.position - border.position;
        attackDirection.Normalize();
        Vector3 moveDirection3D = new Vector3(attackDirection.x, 0, attackDirection.y);


        float moveAmount = Vector2.Distance(border.position, handle.position);
        moveAmount = moveAmount / handleMoveRange;

        if (moveAmount > 0.3f && isSkillEquiped)
        {
            FightSceneController.Instance.LockMoveRotate();
            FightSceneController.Instance.PlayerSkillReady(moveDirection3D);

            if (trajectoryLine == null)
            {
                TurnOnTrajectoryLine();
            }
            Player player = FindObjectOfType<Player>();
            if (handle.GetComponent<Image>().sprite.name == Const_ActiveSkill_1st.ShotGun)
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
    }

    private void UseSkill()
    {
        Vector2 attackDirection = handle.position - border.position;
        attackDirection.Normalize();
        Vector3 moveDirection3D = new Vector3(attackDirection.x, 0, attackDirection.y);

        float moveAmount = Vector2.Distance(border.position, handle.position);
        moveAmount = moveAmount / handleMoveRange;

        if (moveAmount > 0.3f && isSkillEquiped)
        {
            // joystick handle의 이동 범위가 반을 넘어가야 움직이는거
            FightSceneController.Instance.PlayerSkill(moveDirection3D);
        }
        else
        {
            Standby();
        }
    }

    public void SkillOn(Sprite sprite)
    {
        isSkillEquiped = true;
        handle.GetComponent<Image>().sprite = sprite;
    }
    public void SkillOff()
    {
        isSkillEquiped = false;
        handle.GetComponent<Image>().sprite = defaultSkillImage;
    }


    private void Standby()
    {
        touchPos = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;

        if (trajectoryLine != null)
        {
            TurnOffTrajectoryLine();
        }
        if(handle.GetComponent<Image>().sprite.name != Const_ActiveSkill_1st.LinearGun)
        {
            FightSceneController.Instance.UnLockMoveRotate();
        }
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
