using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : CharacterManager
{

    EnemyLocomotionManager enemyLocomotionManager;
    EnemyAnimatorManager enemyAnimationManager;
    EnemyStats enemyStats;
    public State currentState;
    public CharacterStats currentTarget;
    public bool isPreformingAction;
    public  NavMeshAgent navmeshAgent;
    public Rigidbody enemyRigidBody;
    public bool isInteracting;

    [SerializeField]
    public float rotationSpeed = 15;
    public float maxAttackRange = 1.5f;
    public float maximumAggroRadius = 1.5f;

    [Header("Enemy Flags")]
    public bool isInAir, isGrounded;
    int vertical, horizontal;

    public Vector3 moveDirection, normalVector, targetPosition;

    [Header("Ground & Air Detection Stats")]
    [SerializeField]
    public float groundDetectionRayStartPoint = 0.6f;
    [SerializeField]
    public float minimumDistanceNeededToBeginFall = 1.5f;
    [SerializeField]
    public float groundDirectionRayDistance = 0.2f;
    public LayerMask ignoreForGroundCheck;
    public float inAirTimer;

    [Header("Movement Stats")]
    [SerializeField]
    public float movementSpeed = 5;
    [SerializeField]
    public float walkingSpeed = 1;
    [SerializeField]
    public float sprintSpeed = 7;
    [SerializeField]
    public float fallingSpeed = 250;
    [SerializeField]
    public float jumpHeight = 3;
    [SerializeField]
    public float gravityIntensity = -15;
    [SerializeField]
    public float pushoffStrength = 10f;

    [HideInInspector]
    public Transform myTransform;

    [Header("A.I Settings")]
    public float detectionRadius = 20;
    //The higher, and lower, respectively these angles are, the greater detection FIELD OF VIEW (basically like eye sight)
    public float maximumDetectionAngle = 50;
    public float minimumDetectionAngle = -50;
    public float currentRecoveryTime = 0;

    [Header("A.I Combat Settings")]
    public bool allowAIToPerformCombos;
    public float comboLikelyHood;
    public void Initialize()
    {
        vertical = Animator.StringToHash("Vertical");
        horizontal = Animator.StringToHash("Horizontal");
    }

    private void Awake()
    {
        enemyLocomotionManager = GetComponent<EnemyLocomotionManager>();
        enemyAnimationManager = GetComponentInChildren<EnemyAnimatorManager>();
        enemyStats = GetComponent<EnemyStats>();
        navmeshAgent = GetComponentInChildren<NavMeshAgent>();
        enemyRigidBody = GetComponent<Rigidbody>();
        Initialize();
    }

    private void Start()
    {
        myTransform = transform;
        navmeshAgent.enabled = false;
        enemyRigidBody.isKinematic = false;
        isGrounded = true;
        ignoreForGroundCheck = ~(1 << 8 | 1 << 11);
    }

    private void Update()
    {
        HandleRecoveryTimer();
        isInteracting = enemyAnimationManager.anim.GetBool("isInteracting");
        isRotatingWithRootMotion = enemyAnimationManager.anim.GetBool("isRotatingWithRootMotion");
        canDoCombo = enemyAnimationManager.anim.GetBool("canDoCombo");

    }

    private void FixedUpdate()
    {
        HandleStateMachine();
        float time = Time.deltaTime;
        HandleFalling(time, moveDirection);

    }

    public void HandleFalling(float delta, Vector3 moveDirection)
    {
        isGrounded = false;
        RaycastHit hit;
        Vector3 origin = myTransform.position;
        origin.y += groundDetectionRayStartPoint;

        if (Physics.Raycast(origin, myTransform.forward, out hit, 0.4f))
        {
            moveDirection = Vector3.zero;
        }

        if (isInAir)
        {
            //Adds a push when you fall so you dont get stuck on the ledge
            enemyRigidBody.AddForce(-Vector3.up * fallingSpeed);
            enemyRigidBody.AddForce(moveDirection * fallingSpeed / pushoffStrength);
        }

        Vector3 dir = moveDirection;
        dir.Normalize();
        origin = origin + dir * groundDirectionRayDistance;

        targetPosition = myTransform.position;

        //        Debug.DrawRay(origin, -Vector3.up * minimumDistanceNeededToBeginFall, Color.red, 0.1f, false);
        if (Physics.Raycast(origin, -Vector3.up, out hit, minimumDistanceNeededToBeginFall, ignoreForGroundCheck))
        {
            normalVector = hit.normal;
            Vector3 tp = hit.point;
            isGrounded = true;
            targetPosition.y = tp.y;

            if (isInAir)
            {
                if (inAirTimer > 0.5f)
                {
                    //Debug.Log("You were in the air for " + inAirTimer);
                    enemyAnimationManager.PlayTargetAnimation("Land", true);
                    inAirTimer = 0;
                }
                else
                {
                    enemyAnimationManager.PlayTargetAnimation("Movement", false);
                    inAirTimer = 0;
                }

                isInAir = false;
            }
        }
        else
        {
            if (isGrounded)
            {
                isGrounded = false;
            }

            if (isInAir == false)
            {
                if (isPreformingAction == false)
                {
                    enemyAnimationManager.PlayTargetAnimation("Falling", true);
                }

                Vector3 vel = GetComponent<Rigidbody>().velocity;
                vel.Normalize();
                GetComponent<Rigidbody>().velocity = vel * (movementSpeed / 2);
                isInAir = true;
            }
        }

        if (isGrounded)
        {
            if (isPreformingAction)
            {
                myTransform.position = Vector3.Lerp(myTransform.position, targetPosition, Time.deltaTime / 0.1f);
            }
            else
            {
                myTransform.position = targetPosition;
            }
        }
    }
    private void HandleStateMachine()
    {
        if(currentState != null)
        {
            State nextState = currentState.Tick(this,enemyStats,enemyAnimationManager);
            if(nextState != null)
            {
                SwitchToNextState(nextState);
            }
        }

    /*    if (enemyLocomotionManager.currentTarget != null)
        {
            enemyLocomotionManager.distanceFromTarget =
                Vector3.Distance(enemyLocomotionManager.currentTarget.transform.position, transform.position);
        }
        if (enemyLocomotionManager.currentTarget == null)
        {
            enemyLocomotionManager.HandleDetection();
        }
        else if (enemyLocomotionManager.distanceFromTarget > enemyLocomotionManager.stoppingDistance)
        {
            enemyLocomotionManager.HandleMoveToTarget();
        }
        else if (enemyLocomotionManager.distanceFromTarget <= enemyLocomotionManager.stoppingDistance)
        {
            AttackTarget();
        }*/
    }
    private void SwitchToNextState(State nextState)
    {
        currentState = nextState;
    }
    private void HandleRecoveryTimer()
    {
        if (currentRecoveryTime > 0)
        {
            currentRecoveryTime -= Time.deltaTime;
        }

        if (isPreformingAction)
        {
            if (currentRecoveryTime <= 0)
            {
                isPreformingAction = false;
            }
        }
    }


  
}

