﻿using System.Collections;
using Bolt;
using UnityEngine;

public class Ball : EntityEventListener<IBallState>
{
    public Collider Collider;
    public Rigidbody RigidBody;

    void OnTriggerEnter(Collider c)
    {
        if (BoltNetwork.IsServer)
        {
            if (c.gameObject.GetComponent<GoalCollider>())
            {
                Player p = c.GetComponentInParent<Player>();
                GameManager.Cur_BattleManager.Score_Server(p, (TeamNumber) p.state.PlayerInfo.TeamNumber);
            }
        }
    }

    public override void Attached()
    {
        state.SetTransforms(state.Transform, transform);
    }

    public override void SimulateOwner()
    {
        // MeshRenderer.transform.localScale = Vector3.one * transform.position.y / 2.5f;
    }

    public void Reset()
    {
        if (BoltNetwork.IsServer)
        {
            RigidBody.velocity = Vector3.zero;
            RigidBody.angularVelocity = Vector3.zero;
            RigidBody.useGravity = true;
        }
    }

    public void Kick(TeamNumber teamNumber, Vector3 force)
    {
        if (BoltNetwork.IsServer)
        {
            RigidBody.AddForce(force);
        }
    }
}