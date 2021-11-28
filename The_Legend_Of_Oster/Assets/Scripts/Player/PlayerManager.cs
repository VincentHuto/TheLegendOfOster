using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    InputHandler inputHandler;
    Animator anim;
    CameraHandler cameraHandler;
    PlayerLocomotion playerLocomotion;

    [Header("Player Flags")]
    public bool isSprinting, isInteracting, isInAir, isGrounded, isJumping;

    private void Awake()
    {
        cameraHandler = FindObjectOfType<CameraHandler>();
        inputHandler = GetComponent<InputHandler>();
        anim = GetComponentInChildren<Animator>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
    }


    void Update()
    {
        float delta = Time.deltaTime;
        isInteracting = anim.GetBool("isInteracting");
        anim.SetBool("isInAir", isInAir);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isJumping", isJumping);
        inputHandler.HandleAllInputs(delta);
        playerLocomotion.HandleMovement(delta);
        playerLocomotion.HandleRollingAndSprinting(delta);
        playerLocomotion.HandleFalling(delta, playerLocomotion.moveDirection);

    }

    private void FixedUpdate()
    {
        float delta = Time.fixedDeltaTime;

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
        inputHandler.sprintFlag = false;
        inputHandler.rb_Input = false;
        inputHandler.rt_Input = false;
        inputHandler.lb_Input = false;
        inputHandler.lt_Input = false;
        isJumping = anim.GetBool("isJumping");
        anim.SetBool("isGrounded", isGrounded);
        

        if (isInAir)
        {
            playerLocomotion.inAirTimer = playerLocomotion.inAirTimer + Time.deltaTime;
        }
    }


}
