﻿using UnityEngine;

public class ScoreRingSingleTrigger : MonoBehaviour
{
    public ScoreRingSingle ScoreRingSingle;

    void OnTriggerEnter(Collider c)
    {
        if (GameManager.Instance.M_NetworkMode == GameManager.NetworkMode.Local || BoltNetwork.IsServer)
        {
            if (c.gameObject.GetComponentInParent<PlayerCollider>())
            {
                Player p = c.GetComponentInParent<Player>();
                ((BattleManager_FlagRace) GameManager.Instance.Cur_BallBattleManager).EatDropScoreRingSingle(p, ScoreRingSingle);
            }
        }
    }
}