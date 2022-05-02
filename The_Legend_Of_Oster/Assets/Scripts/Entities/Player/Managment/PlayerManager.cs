using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : CharacterManager
{
    InputHandler inputHandler;
    Animator anim;
    PlayerStats playerStats;
    CameraHandler cameraHandler;
    PlayerLocomotion playerLocomotion;
    InteractableUI interactableUI;
    public GameObject interactableUIGameObject, itemInteractableGameObject;
    UIManager uiManager;

    [Header("Player Flags")]
    public bool isSprinting, isInteracting, isInAir, isGrounded,
        isJumping, isUsingRightHand, isUsingLeftHand,
        isInvulnerable;



    private void Awake()
    {
        cameraHandler = FindObjectOfType<CameraHandler>();
        uiManager = FindObjectOfType<UIManager>();
    }

    private void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        inputHandler = GetComponent<InputHandler>();
        anim = GetComponentInChildren<Animator>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        interactableUI = FindObjectOfType<InteractableUI>();

        if (!isLocalPlayer)
        {
            cameraHandler.gameObject.SetActive(false);
        }

    }


    void Update()
    {
        float delta = Time.deltaTime;


        if (isLocalPlayer)
        {
            isInteracting = anim.GetBool("isInteracting");
            canDoCombo = anim.GetBool("canDoCombo");
            anim.SetBool("isInAir", isInAir);
            anim.SetBool("isGrounded", isGrounded);
            anim.SetBool("isJumping", isJumping);
            isInvulnerable = anim.GetBool("isInvulnerable");
            isUsingRightHand = anim.GetBool("isUsingRightHand");
            isUsingLeftHand = anim.GetBool("isUsingLeftHand");


            inputHandler.HandleAllInputs(delta);
            playerLocomotion.HandleRollingAndSprinting(delta);
            playerStats.RegenerateStamina();
            CheckForInteractableObject();
          
        }
        if (cameraHandler != null)
        {
            cameraHandler.FollowTarget(delta);
            cameraHandler.HandleCameraRotation();
        }

    }

    private void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            float delta = Time.fixedDeltaTime;
            playerLocomotion.HandleMovement(delta);
            playerLocomotion.HandleFalling(delta, playerLocomotion.moveDirection);
        }

    }

    private void LateUpdate()
    {
        if (isLocalPlayer)
        {
            //Makes sure the button only triggers once even if you accidently misclick or hold down the button
            inputHandler.rollFlag = false;
            inputHandler.rb_Input = false;
            inputHandler.rt_Input = false;
            inputHandler.lb_Input = false;
            inputHandler.lt_Input = false;
            inputHandler.d_Pad_Up = false;
            inputHandler.d_Pad_Down = false;
            inputHandler.d_Pad_Right = false;
            inputHandler.d_Pad_Left = false;
            inputHandler.pickup_Input = false;
            inputHandler.inv_Input = false;



            isJumping = anim.GetBool("isJumping");
            anim.SetBool("isGrounded", isGrounded);


            if (isJumping)
            {
                anim.SetBool("isGrounded", false);

            }

            float delta = Time.fixedDeltaTime;



            if (isInAir)
            {
                playerLocomotion.inAirTimer = playerLocomotion.inAirTimer + Time.deltaTime;
            }
        }
    }

    public void CheckForInteractableObject()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 0.3f, transform.forward, out hit, 1f, cameraHandler.ignoreLayers))
        {
            if (hit.collider.tag == "Interactable")
            {
                Interactable interactable = hit.collider.GetComponent<Interactable>();
                if (interactable != null)
                {
                    string interactableText = interactable.interactionText;
                    interactableUI.interactionText.text = interactableText;
                    if (!uiManager.IsBlenderOpen)
                    {
                        if (!isInteracting)
                        {
                            interactableUIGameObject.SetActive(true);
                        }
                        else
                        {
                            interactableUIGameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        interactableUIGameObject.SetActive(false);
                    }
                    if (inputHandler.pickup_Input)
                    {
                        hit.collider.GetComponent<Interactable>().Interact(this);
                    }
                }
            }
        }
        else
        {
            if (uiManager.IsBlenderOpen)
            {
                uiManager.CloseBlenderWindow();
            }


            if (interactableUIGameObject != null)
            {
                interactableUIGameObject.SetActive(false);
            }
            if (itemInteractableGameObject != null && inputHandler.pickup_Input == true)
            {
                itemInteractableGameObject.SetActive(false);

            }
        }
    }
}
