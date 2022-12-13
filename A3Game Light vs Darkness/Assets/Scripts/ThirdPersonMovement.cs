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
    

    public enum PlayerState
    {
        Idle, Run, Attack, Die, Jump, Fall
    }

    public enum PlayerMovement
    {
        ThirdPerson, FPS

    }

    public enum HookshotState
    {
        Normal,HookshotFlying,HookshotThrown
    }

    [Header("Enums")]
    public PlayerMovement playerMovement; 
    public PlayerState playerState;
    public HookshotState hookshotState;

    [Header("Movement Setting")]
    public CharacterController characterController;
    public Transform cam;
    Vector3 direction;
    public bool fpsMode;

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
    Vector3 velocityMomentum;
    bool isGrounded;


    [Header("Player Stats")]
    public float playerHealth = 100; //temp
    public int playerDamage = 10;
    public float speed = 6f;

    [Header("Animation")]
    Animator anim;
    bool running;
    bool idle = true;
    bool isJumping;
    float attackTime;
    string clipName;
    AnimatorClipInfo[] animCurrentClipInfo;

    [Header("Hookshot")]
    float hookshotSize;
    Vector3 hookshotPosition;
    public LineRenderer hookshotLine;
    public Transform hookshotTransform;

    [Header("Lightning Attack")]
    public LineRenderer lightingBounce;
    public List<GameObject> lightingEnemyTargets;


    private void Start()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        anim = GetComponent<Animator>();
 
    }


    // Update is called once per frame
    void Update()
    {

        switch (hookshotState)
        {
            //when not using hookshot
            case HookshotState.Normal:

                //disable hookshot
                hookshotLine.enabled = false;

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

                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    fpsMode = !fpsMode;
                    print(fpsMode);
                }

                if (fpsMode)
                {
                    playerMovement = PlayerMovement.FPS;
                    _CS.fPCameraOn();
                }
                else if (!fpsMode)
                {
                    _CS.followPlayerCameraOn();
                    playerMovement = PlayerMovement.ThirdPerson;
                }

                HookshotStart();


                switch (playerMovement)
                {
                    case PlayerMovement.ThirdPerson:
                        //WASD inputs
                        float horizontal = Input.GetAxisRaw("Horizontal");
                        float vertical = Input.GetAxisRaw("Vertical");
                        direction = new Vector3(horizontal, 0f, vertical).normalized;

                        if (Input.GetButtonDown("Jump") && isGrounded)
                        {
                            playerState = PlayerState.Jump;
                        }

                        if (direction.magnitude >= 0.1f)
                        {
                            playerState = PlayerState.Run;
                        }
                        break;
                    case PlayerMovement.FPS:
                        //cannot move
                        break;
                }


                switch (playerState)
                {
                    case PlayerState.Idle:

                        //if idle slowly decrease speed to 0
                        if (idle)
                        {
                            this.velocity = transform.position * 0;
                            speed = speed - 20 * Time.deltaTime;
                            if (speed < 0) speed = 0;
                        }

                        break;
                    case PlayerState.Run:
                        this.velocity = transform.position * 0;
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
                            velocity.y += gravity * Time.deltaTime;
                            playerState = PlayerState.Fall;
                        }

                        break;
                    case PlayerState.Fall:
                        anim.SetBool("IsJumping", false);
                        anim.SetBool("IsFalling", true);
                        isJumping = false;

                        if (isGrounded)
                        {
                            playerState = PlayerState.Idle;
                        }

                        break;

                }


                velocity.y += gravity * Time.deltaTime;

                characterController.Move(velocity * Time.deltaTime);

                //dampen momentum
                if (velocityMomentum.magnitude >= 0f)
                {
                    float momentumDrag = 3f;
                    velocity -= velocityMomentum * momentumDrag * Time.deltaTime;
                    ;
                    if (velocity.magnitude < .0f)
                    {
                        velocity = Vector3.zero;
                    }
                }

                StartCoroutine(ResetSpeed());


                break;

            //hookshot being thrown, line renderer is loading
            case HookshotState.HookshotThrown:
                //turn on hookshot
                hookshotLine.enabled = true;
                StartCoroutine(HandleHookshotThrown());
                break;

            // player moving towards hookshot target
            case HookshotState.HookshotFlying:

                //fail safe to ensure hookshot is enabled
                hookshotLine.enabled = true;
                HookShotMovement();

                break;



        }

    }

    void HookshotStart()
    {
        if (TestInputDownHookshot() && fpsMode)
        {
            if (Physics.Raycast(_CS.fPCamera.transform.position, _CS.fPCamera.transform.forward, out RaycastHit raycastHit))
            {

                if (raycastHit.collider.CompareTag("HookPoint"))
                {
                    print("Swing");

                    //debugCube.transform.position = raycastHit.point;
                    hookshotPosition = raycastHit.point;
                    hookshotSize = 0f;
                    hookshotState = HookshotState.HookshotThrown;
                }

            }
        }
    }

    IEnumerator HandleHookshotThrown()
    {
        hookshotLine.SetPosition(0, hookshotTransform.position);
        hookshotLine.SetPosition(1, hookshotPosition);
        yield return new WaitForSeconds(0.5f);

        if (hookshotLine.GetPosition(1) == hookshotPosition)
        {

            hookshotState = HookshotState.HookshotFlying;
        }
    }

    void HookShotMovement()
    {
        hookshotLine.SetPosition(0, hookshotTransform.position);
        Vector3 hookshotDir = (hookshotPosition - transform.position).normalized;
        //clamps for speed
        float hookShotSpeedMin = 10f;
        float hookShotSpeedMax = 40f;
        float hookshotSpeed = Mathf.Clamp(Vector3.Distance(transform.position, hookshotPosition), hookShotSpeedMin, hookShotSpeedMax);
        float hookshotSpeedMultipler = 2f;

        //

        characterController.Move(hookshotDir * hookshotSpeed * hookshotSpeedMultipler * Time.deltaTime);

        float reachedHookshotPositionDistance = 1f;
        if (Vector3.Distance(transform.position, hookshotPosition) < reachedHookshotPositionDistance)
        {
            //reached pos

            hookshotState = HookshotState.Normal;
            ResetGravity();

            //changes camera back when we hit the ground
            StartCoroutine(ResetCameraCheck());

        }

        if (TestInputDownHookshot())
        {
            //cancel hookshot
            hookshotState = HookshotState.Normal;
            ResetGravity();
            StartCoroutine(ResetCameraCheck());

        }

    }

    private bool TestInputDownHookshot()
    {
        return Input.GetKeyDown(KeyCode.E);
    }


    void ResetGravity()
    {
        velocity.y = -2f;
    }

    IEnumerator ResetCameraCheck()
    {
        yield return new WaitForSeconds(1);
        if (hookshotState == HookshotState.Normal) fpsMode = false;
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

        if (prevPos == currentPos && playerState != PlayerState.Attack)
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

        if(attackTime > 20)
        {
            this.velocity = transform.forward * 2;
        }

        


        //loops attack cycle after final animation in cycle is played
        

        clipName = animCurrentClipInfo[0].clip.name;
        if (clipName == "2Hand-Sword-Attack5")
        {
            attackTime = 0;

        } 




    }

    public void PlayerTakeDamage(float damage)
    {
        print("PlayerHit");
        playerHealth -= damage;
        AnimationTrigger("Hit");
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
