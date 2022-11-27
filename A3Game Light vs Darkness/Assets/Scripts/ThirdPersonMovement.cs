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
    bool running;
    bool idle = true;
    bool isJumping;


    private void Start()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        anim = GetComponent<Animator>();

        
    }


    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("Speed", speed);
        if (idle)
        {
            speed = speed - 20 * Time.deltaTime;
            if(speed < 0) speed = 0;

        }

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            
            
        }

        if(isGrounded)
        {
            anim.SetBool("IsJumping", false);
            anim.SetBool("IsFalling", false);
            isJumping = false;
            AnimationTrigger("Grounded");   
        }

        else
        {

            if(isJumping && velocity.y < 0 || velocity.y < -2)
            {
                anim.SetBool("IsJumping", false);
                anim.SetBool("IsFalling", true);
            }
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f) 
        {

            anim.SetBool("IsJumping", false);
            anim.SetBool("IsFalling", false);
            isJumping = false;
            running = true;
            idle = false;

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
            //AnimationTrigger("Jump");
            anim.SetBool("IsJumping", true);
            isJumping = true;
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpHeight = jumpHeight + 3 * Time.deltaTime;

        }



        if (running)
        {
            StartCoroutine(SpeedIncrease());
        }
        else StartCoroutine(ResetSpeed());


        velocity.y += gravity * Time.deltaTime;

        characterController.Move(velocity * Time.deltaTime);

        StartCoroutine(ResetSpeed());
        //StartCoroutine(IdleCheck());
    }

    IEnumerator SpeedIncrease()
    {
        //yield return new WaitForSeconds(1);
        if(speed < 12)
        {
            speed = speed + 12 * Time.deltaTime;
            if(speed >= 5 && speed <=6)
            {
                yield return new WaitForSeconds(1f);
            }
        }
        if (speed > 12) speed = 12;
        
        //speed = 12;
    }

    IEnumerator ResetSpeed()
    {
        Vector3 prevPos = transform.position;
        yield return new WaitForSeconds(0.5f);
        Vector3 currentPos = transform.position;
        running = false;
        if (prevPos == currentPos)
            if (speed > 6)
            {
                speed = speed - 10 * Time.deltaTime;
                if (speed < 6) speed = 6;
            }
        

        StartCoroutine(IdleCheck());

    }

    IEnumerator IdleCheck()
    {
        Vector3 prevPos = transform.position;
        yield return new WaitForSeconds(1f);
        Vector3 currentPos = transform.position;

        if (prevPos == currentPos) idle = true;

    }


    void AnimationTrigger(string _anim)
    {
        anim.SetTrigger(_anim);
    }


    public void Hit()
    {

    }

    public void Shoot()
    {

    }

    public void Land()
    {

    }

    public void FootR()
    {

    }

    public void FootL()
    {
        
    }
}
