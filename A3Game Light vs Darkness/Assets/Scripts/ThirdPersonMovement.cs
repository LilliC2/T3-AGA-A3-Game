using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

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

    ////running
    //float sprintSpeed = 9f;
    //bool sprinting;
    //bool fatigued;
    //float fatigueSpeed = 4.5f;

    ////sneaking
    //float sneakSpeed = 4;
    //bool sneaking;
   

    [Header("Player Stats")]

    public float playerHealth = 100; //temp
    public float stamina = 10;

    
   


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        //slowly regain stamina

        //if (stamina < 10 && !sprinting && isGrounded)
        //{
        //    stamina += Time.deltaTime;
        //}


        //Debug.Log("Stamina: " + stamina);
        //if(fatigued)Debug.Log("FATIGUED");

        //inputs
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        //while sprinting
        //if (Input.GetKey(KeyCode.LeftShift)&& isGrounded)
        //{
        //    if (!fatigued)
        //    {
        //        speed = sprintSpeed;
        //        sprinting = true;
        //    }

        //    if (sprinting) stamina -= Time.deltaTime;

        //    if (stamina <= 0) fatigued = true;
                

        //    if(fatigued)
        //    {
        //        sprinting = false;
        //        speed = fatigueSpeed;

        //        if (stamina >= 10)
        //        {
        //            fatigued = false;
        //        }
        //    }


            //things to add
            //jump further when sprinting
        //}
        ////while sneaking
        //else if(Input.GetKey(KeyCode.LeftControl)&& isGrounded)
        //{
            
        //    speed = sneakSpeed;
        //    sneaking = true;

        //    //things to add
        //    //change player size
        //}
        //else
        //{
        //    speed = 6;
        //    sneaking = false;
        //    sprinting = false;
        //}


        if (direction.magnitude >= 0.1f) 
        {
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

            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            //if (sprinting && sneaking != true)
            //{
                
            //    velocity.y = 12;
            //}
            //if(sneaking && sprinting != true)
            //{
            //    velocity.y = 5;
            //}
            //else if(sprinting != true && sneaking != true)
            //{
            //    //sqrt is square root
                
            //}

            
            //to add
            // when sneakig and press jump, do a roll
        }

        velocity.y += gravity * Time.deltaTime;

        characterController.Move(velocity * Time.deltaTime);


    }
}
