using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class ThirdPersonMovement : MonoBehaviour
{
    [Header("Movement Setting")]
    public CharacterController characterController;
    public Transform cam;
    public float speed = 6f;




    //directinal
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    //jumping
    public float gravity = -9.81f;
    public float jumpHeight = 1f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    Vector3 velocity;
    bool isGrounded;


    [Header("Player Stats")]
    public float playerHealth = 100; //temp

    [Header("Animation")]

    Animator anim;
    NavMeshAgent agent;
    float animSpeed = 0;
    bool running;



    private void Start()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        
    }


    // Update is called once per frame
    void Update()
    {
        switch(speed)
        {
            case 6:
                animSpeed = 6;
                break;
            case 12:
                animSpeed = 12f;
                break;
        }

        //if (!running) animSpeed = 0;

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        anim.SetFloat("Speed", animSpeed);

        if (direction.magnitude >= 0.1f) 
        {
            running = true;

            //makes the player face the correct way
            //atan2 caculates what angle to rotate the player to face the desired direction
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            //makes so player doesnt snap to direction
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);



            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            characterController.Move(moveDir.normalized * speed * Time.deltaTime);
            
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            AnimationTrigger("Jump");
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        }

        if (running)
        {
            StartCoroutine(SpeedIncrease());
        }
        else StartCoroutine(ResetSpeed());


        velocity.y += gravity * Time.deltaTime;

        characterController.Move(velocity * Time.deltaTime);

        StartCoroutine(ResetSpeed()); 
    }

    IEnumerator SpeedIncrease()
    {
        yield return new WaitForSeconds(1);
        speed = 12;
    }

    IEnumerator ResetSpeed()
    {
        Vector3 prevPos = transform.position;
        yield return new WaitForSeconds(1);
        Vector3 currentPos = transform.position;
        running = false;

        if (prevPos == currentPos) speed = 6;
        
    }


    void AnimationTrigger(string _anim)
    {
        anim.SetTrigger(_anim);
    }

}
