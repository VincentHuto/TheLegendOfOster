using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public CharacterController controller;
    public Animator anim;
    private bool isGrounded;
    private bool isWalkingButtonDown;

    public float speed = 12f;

    public float gravity = -9.81f;

    public float jumpHeight = 3f;
    Vector3 velocity;

    public Transform groundCheck;

    public float groundDistance = 0.4f;

    public LayerMask groundMask;


    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {


        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        float y = Input.GetAxis("Jump");
        anim.SetFloat("vertical", z);
        anim.SetFloat("horizontal", x);
        anim.SetFloat("jump", y);

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
     
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

    }
}
