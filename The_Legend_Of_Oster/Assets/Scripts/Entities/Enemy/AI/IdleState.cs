using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State
{
    public PursueTargetState pursueTarget;
    public LayerMask detectionLayer;

    public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, enemyManager.detectionRadius, detectionLayer);

        for (int i = 0; i < colliders.Length; i++)
        {
            CharacterStats characterStats = colliders[i].transform.GetComponentInParent<CharacterStats>();


            if (characterStats != null)
            {
                //CHECK FOR TEAM ID

                Vector3 targetDirection = characterStats.transform.position - transform.position;
                float viewableAngle = Vector3.Angle(targetDirection, transform.forward);

                if (viewableAngle > enemyManager.minimumDetectionAngle && viewableAngle < enemyManager.maximumDetectionAngle)
                {
                    enemyManager.currentTarget = characterStats;
                    return pursueTarget;
                }
            }
        }
        if(enemyManager.currentTarget != null)
        {
            return pursueTarget;
        }
        else
        {
            return this;
        }
    }
}
