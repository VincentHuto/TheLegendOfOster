using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyLocomotionManager : MonoBehaviour
{
    EnemyManager enemyManager;
    EnemyAnimatorManager enemyAnimatorManager;

    public Vector3 moveDirection, normalVector, targetPosition;

    [Header("Ground & Air Detection Stats")]
    [SerializeField]
    float groundDetectionRayStartPoint = 0.6f;
    [SerializeField]
    float minimumDistanceNeededToBeginFall = 1.5f;
    [SerializeField]
    float groundDirectionRayDistance = 0.2f;
    LayerMask ignoreForGroundCheck;
    public float inAirTimer;

    [Header("Movement Stats")]
    [SerializeField]
    float movementSpeed = 5;
    [SerializeField]
    float walkingSpeed = 1;
    [SerializeField]
    float sprintSpeed = 7;
    [SerializeField]
    float fallingSpeed = 250;
    [SerializeField]
    float jumpHeight = 3;
    [SerializeField]
    float gravityIntensity = -15;
    [SerializeField]
    float pushoffStrength = 10f;

    [HideInInspector]
    public Transform myTransform;
    public CapsuleCollider characterCollider;
    public CapsuleCollider characterCollisionBlockerCollider;


    private void Awake()
    {
     
    }

    private void Start()
    {
        Physics.IgnoreCollision(characterCollider, characterCollisionBlockerCollider, true);
    }

}
