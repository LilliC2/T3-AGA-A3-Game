using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class ThirdPersonMovement : Singleton<ThirdPersonMovement>
{
    [Header("Movement Setting")]
    public CharacterController characterController;
    public Transform cam;
    public float speed = 6f;

    public enum PlayerState
    {
        Idle, Run, Attack, Die, Jump, Fall
    }

    public PlayerState playerState;

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
    public int playerDamage = 10;

    [Header("Animation")]

    Animator anim;
    bool running;
    bool idle = true;
    bool isJumping;
    float attackTime;

    string clipName;
    AnimatorClipInfo[] animCurrentClipInfo;


    private void Start()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        anim = GetComponent<Animator>();
 
    }


    // Update is called once per frame
    void Update()
    {

        //print(playerState);

        anim.SetFloat("Speed", speed);

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        //reset animations if grounded
        if (isGrounded)
        {
            anim.SetBool("IsJumping", false);
            anim.SetBool("IsFalling", false);
            isJumping = false;
            AnimationTrigger("Grounded");
        }
        //if not grounded, starting falling
        else
        {
            if (isJumping && velocity.y < 0 || velocity.y < -2)
            {
                playerState = PlayerState.Fall;
            }
        }



        //WASD inputs
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;


        if (direction.magnitude >= 0.1f)
        {
            playerState = PlayerState.Run;
        }

        //jump animations and movement
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            playerState = PlayerState.Jump;

        }

        //attack
        if (Input.GetButton("Fire1"))
        {
            playerState = PlayerState.Attack;
        }
        //reset attackTime when mouse button is release
        if (Input.GetButtonUp("Fire1"))
        {
            speed = 6;
            attackTime = 0;
            anim.SetFloat("AttackTime", attackTime);

            playerState = PlayerState.Idle;
        }

        switch (playerState)
        {
            case PlayerState.Idle:

                //if idle slowly decrease speed to 0
                if (idle)
                {
                    speed = speed - 20 * Time.deltaTime;
                    if (speed < 0) speed = 0;
                }

                break;
            case PlayerState.Run:

                anim.SetFloat("Speed", speed);

                if (direction.magnitude >= 0.1f)
                {
                    //turn off jumping and falling animations fail safe
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


                    //move player
                    Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                    characterController.Move(moveDir.normalized * speed * Time.deltaTime);

                }

                //slowly increase speed until max when running
                if (running)
                {
                    StartCoroutine(SpeedIncrease());
                }
                //check if not running if so slowly decrease speed
                else StartCoroutine(ResetSpeed());

                break;
            case PlayerState.Attack:

                Attack();

                


                break;
            case PlayerState.Die:
                break;
            case PlayerState.Jump:

                //AnimationTrigger("Jump");
                anim.SetBool("IsJumping", true);
                isJumping = true;
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                jumpHeight = jumpHeight + 3 * Time.deltaTime;

                if (!isGrounded)
                {
                    playerState = PlayerState.Fall;
                }

                break;
            case PlayerState.Fall:
                anim.SetBool("IsJumping", false);
                anim.SetBool("IsFalling", true);
                isJumping=false;

                if(isGrounded)
                {
                    playerState = PlayerState.Idle;
                }

                break;

        }


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
        //if stationatry for 1 second, become idle
        Vector3 prevPos = transform.position;
        yield return new WaitForSeconds(1f);
        Vector3 currentPos = transform.position;

        if (prevPos == currentPos)
        {
            idle = true;
            playerState = PlayerState.Idle;
        }
    }


    public void Attack()
    {
        animCurrentClipInfo = anim.GetCurrentAnimatorClipInfo(0);
        playerState = PlayerState.Attack;
        speed = 3;
        attackTime ++;
        anim.SetFloat("AttackTime", attackTime);

        //moves player forward each attack animation

        this.transform.position = transform.forward * 2;


        //loops attack cycle after final animation in cycle is played
        

        clipName = animCurrentClipInfo[0].clip.name;
        if (clipName == "2Hand-Sword-Attack5")
        {
            attackTime = 0;

        } 




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
