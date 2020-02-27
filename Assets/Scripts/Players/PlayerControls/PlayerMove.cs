﻿using UnityEngine;
using System.Collections;
using System.Security.Cryptography;

public class PlayerMove : MonoBehaviour, IPlayerControl
{
    private PlayerControl ParentPlayerControl;

    public void Initialize(PlayerControl parentPlayerControl)
    {
        ParentPlayerControl = parentPlayerControl;
    }

    [SerializeField] private float MoveSpeed = 0.3f;
    [SerializeField] private JoystickAxis HorizontalAxis = JoystickAxis.None;
    [SerializeField] private JoystickAxis VerticalAxis = JoystickAxis.None;

    void FixedUpdate()
    {
        if (ParentPlayerControl && ParentPlayerControl.Controllable)
        {
            Move();
        }
    }

    private void Move()
    {
        if (!MultiControllerManager.Instance.PlayerControlMap.ContainsKey(ParentPlayerControl.Player.PlayerNumber)) return;
        PlayerNumber myControllerIndex = MultiControllerManager.Instance.PlayerControlMap[ParentPlayerControl.Player.PlayerNumber];

        Vector3 diff = Vector3.zero;
        diff += Vector3.forward *  MultiControllerManager.Instance.Controllers[myControllerIndex].Axises[ControlAxis.LeftStick_H];
        diff += Vector3.right * MultiControllerManager.Instance.Controllers[myControllerIndex].Axises[ControlAxis.LeftStick_V];
        diff = diff.normalized * MoveSpeed;
        Vector3 tarPos = transform.position + diff;

        tarPos = ParentPlayerControl.Player.TryToMove(tarPos, GameManager.Instance.PlayerRadius);
        transform.position = tarPos;
    }
}