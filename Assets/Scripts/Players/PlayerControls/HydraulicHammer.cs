﻿using UnityEngine;

public class HydraulicHammer : Controllable
{
    public Animator Anim;

    private void LateUpdate()
    {
        transform.LookAt(GameManager.Instance.Cur_BattleManager.Ball.transform);
    }

    public float KickRadius = 1.5f;
    public float Force = 100f;

    protected override void Operate_AI()
    {
    }

    protected override void Operate_Manual(PlayerNumber controllerIndex)
    {
        if (MultiControllerManager.Instance.Controllers[controllerIndex].ButtonDown[ControlButtons.RightBumper])
        {
            Bump();
        }
    }

    private void Bump()
    {
        Anim.SetTrigger("Kick");
        IKickable ko = GameManager.Instance.Cur_BattleManager.Ball;
        Vector3 diff = GameManager.Instance.Cur_BattleManager.Ball.transform.position - transform.position;

        float distance = diff.magnitude;
        if (distance < KickRadius)
        {
            ko.Kick(ParentPlayerControl.Player.PlayerInfo.RobotIndex, (diff.normalized) * Force);
            FXManager.Instance.PlayFX(FX_Type.BallKickParticleSystem, GameManager.Instance.Cur_BattleManager.Ball.transform.position, Quaternion.FromToRotation(Vector3.back, diff.normalized));
        }
    }
}