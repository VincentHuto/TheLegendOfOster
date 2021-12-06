using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    InputHandler inputHandler;
    Animator anim;
    CameraHandler cameraHandler;
    PlayerLocomotion playerLocomotion;
    InteractableUI interactableUI;
    public GameObject interactableUIGameObject,itemInteractableGameObject;

    [Header("Player Flags")]
    public bool isSprinting, isInteracting, isInAir, isGrounded, isJumping, canDoCombo;

    private void Awake()
    {
        cameraHandler = FindObjectOfType<CameraHandler>();

    }

    private void Start()
    {
        inputHandler = GetComponent<InputHandler>();
        anim = GetComponentInChildren<Animator>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        interactableUI = FindObjectOfType<InteractableUI>();
    }


    void Update()
    {
        float delta = Time.deltaTime;
        isInteracting = anim.GetBool("isInteracting");
        canDoCombo = anim.GetBool("canDoCombo");
        anim.SetBool("isInAir", isInAir);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isJumping", isJumping);
        inputHandler.HandleAllInputs(delta);
        playerLocomotion.HandleRollingAndSprinting(delta);
        CheckForInteractableObject();
    }

    private void FixedUpdate()
    {
        float delta = Time.fixedDeltaTime;
        playerLocomotion.HandleMovement(delta);
        playerLocomotion.HandleFalling(delta, playerLocomotion.moveDirection);
        if (cameraHandler != null)
        {
            cameraHandler.FollowTarget(delta);
            cameraHandler.HandleCameraRotation(delta, inputHandler.mouseX, inputHandler.mouseY);
        }
    }

    private void LateUpdate()
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

        float delta = Time.fixedDeltaTime;

    

        if (isInAir)
        {
            playerLocomotion.inAirTimer = playerLocomotion.inAirTimer + Time.deltaTime;
        }
    }

    public void CheckForInteractableObject()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position,0.3f,transform.forward,out hit,1f,cameraHandler.ignoreLayers)){
            if(hit.collider.tag == "Interactable")
            {
                Interactable interactable = hit.collider.GetComponent<Interactable>();
                if(interactable != null)
                {
                    string interactableText = interactable.interactionText;
                    interactableUI.interactionText.text = interactableText;
                    interactableUIGameObject.SetActive(true);

                    if (inputHandler.pickup_Input)
                    {
                        hit.collider.GetComponent < Interactable>().Interact(this);
                    }
                }
            }
        }
        else
        {
            if(interactableUIGameObject != null)
            {
                interactableUIGameObject.SetActive(false);
            }
            if(itemInteractableGameObject != null && inputHandler.pickup_Input == true)
            {
                itemInteractableGameObject.SetActive(false);
            }
        }
    }
}
