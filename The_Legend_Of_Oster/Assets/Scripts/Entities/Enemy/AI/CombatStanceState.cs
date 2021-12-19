using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatStanceState : State
{

    public AttackState attackState;
    public PursueTargetState pursueTargetState;

    public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager)
    {
        float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);

        HandleRotateTowardsTarget(enemyManager);

        if (enemyManager.isPreformingAction)
        {
            enemyAnimatorManager.anim.SetFloat("Vertical", 0,0.1f, Time.deltaTime);
        }

        if(enemyManager.currentRecoveryTime <= 0 && distanceFromTarget <= enemyManager.maxAttackRange)
        {
            return attackState;
        }else if(distanceFromTarget > enemyManager.maxAttackRange)
        {
            return pursueTargetState;
        }
        else
        {
            return this;
        }
    }

    private void HandleRotateTowardsTarget(EnemyManager enemyManager)
    {
        //Rotate manually
        if (enemyManager.isPreformingAction)
        {
            Vector3 direction = enemyManager.currentTarget.transform.position - transform.position;
            direction.y = 0;
            direction.Normalize();

            if (direction == Vector3.zero)
            {
                direction = transform.forward;
            }

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            enemyManager.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, enemyManager.rotationSpeed / Time.deltaTime);
        }
        //Rotate with pathfinding (navmesh)
        else
        {
            Vector3 relativeDirection = transform.InverseTransformDirection(enemyManager.navmeshAgent.desiredVelocity);
            Vector3 targetVelocity = enemyManager.enemyRigidBody.velocity;

            enemyManager.navmeshAgent.enabled = true;
            enemyManager.navmeshAgent.SetDestination(enemyManager.currentTarget.transform.position);
            enemyManager.enemyRigidBody.velocity = targetVelocity;
            enemyManager.transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation, enemyManager.navmeshAgent.transform.rotation, enemyManager.rotationSpeed / Time.deltaTime);
        }
    }
}
