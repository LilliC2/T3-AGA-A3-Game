using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Enemy : GameBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;

    public float health;

    //patrolling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;



    //attacking
    public float timeBetweenAttacks = 20;
    public float sightRange, attackRange,playerHitRange;
    public bool playerInSightRange, playerInAttackRange, enemyInHitRange;
    public bool alreadyAttacked = false;
    bool attackRangeRnd = false;
    bool attacking;
    bool hit;

    //states
    bool walking;
    bool idle;
    bool rotateDetermined = false;

    //animation
    //animation
    Animator enemyAnimation;

    //atacking orbit
    public bool orbiting = true;
    float time;
    int rotate;
    bool jumped;
    bool preJumpPosBool = false;
    Vector3 preJumpPos;
    bool enterting;


    private void Awake()
    {
        player = GameObject.Find("PlayerAvatar").transform;
        agent = GetComponent<NavMeshAgent>();
        enemyAnimation = gameObject.GetComponent<Animator>();
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
 
    }

    private void Update()
    {
        //check for the sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        enemyInHitRange = Physics.CheckSphere(transform.position, playerHitRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) StartCoroutine(PatrolingIE());
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) StartCoroutine(AttackPlayer());

    }

    IEnumerator PatrolingIE()
    {
        rotateDetermined = false;
        attackRangeRnd = false;
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
            enemyAnimation.SetBool("Walk Forward", true);
            gameObject.GetComponent<NavMeshAgent>().speed = 1;
        }
            

        //calculate distance to walk point
        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //walkpouint reached
        if (distanceToWalkPoint.magnitude < 1f)
        {
            enemyAnimation.SetBool("Run Forward", false);
            float result = RandomFloatBetwenTwoFloats(0.5f,2f);
            yield return new WaitForSeconds(result);
            walkPointSet = false;   
        }
    }

    private void SearchWalkPoint()
    {
        //calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomx = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomx, transform.position.y, transform.position.z + randomZ);

        //check if new walkpoint is on ground
        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }       
    }

    private void ChasePlayer()
    {
        enemyAnimation.SetBool("Walk Forward", false);
        if(!attacking) enemyAnimation.SetBool("Run Forward", true);
        gameObject.GetComponent<NavMeshAgent>().speed = 5;

        agent.SetDestination(player.position);


        if( attackRangeRnd == false)
        {
            attackRange = RandomFloatBetwenTwoFloats(10f, 15);
            attackRangeRnd = true;
        }


    }

    IEnumerator AttackPlayer()
    {


        if (!preJumpPosBool)
        {
            //print("prejump set");
            preJumpPos = gameObject.transform.position;
            preJumpPosBool = true;
        }



        //make sure enemy doesnt move
        agent.SetDestination(transform.position);
        
        gameObject.GetComponent<NavMeshAgent>().speed = 1;

        transform.LookAt(player.position);
        transform.eulerAngles = new Vector3(0,transform.eulerAngles.y, 0);

        if (orbiting)
        {

            OrbitPlayer();
            
            int waitTime = 5;
            waitTime = RandomFloatBetwenTwoInts(5, 10);
            //print("Wait time: " + waitTime)

            yield return new WaitForSeconds(waitTime);
            orbiting = false;
        }

        if (!alreadyAttacked && !orbiting)
        {
            rotateDetermined = false;
            //stop previous animations
            enemyAnimation.SetBool("Strafe Left", false);
            enemyAnimation.SetBool("Strafe Right", false);

            //jump once
            if (!jumped)
            {
                enemyAnimation.SetBool("Run Forward", false);
                //enemyAnimation.SetBool("Run Forward", true);
                StartCoroutine(PlayAnimation("Jump"));
                jumped = true;
                enterting = true;
            }

            //StartCoroutine(PlayAnimation("Run Forward"));

            if (!enemyInHitRange && enterting)
            {
                
                gameObject.GetComponent<NavMeshAgent>().speed = 5;
                agent.SetDestination(player.position);//transform.position = Vector3.MoveTowards(transform.position, player.transform.position, 0.05f);
                yield return new WaitForSeconds(0.5f);
                enemyAnimation.SetBool("Run Forward", true);
                //if (jumped)
                //{
                //    yield return new WaitForSeconds(1);
                //    enemyAnimation.SetBool("Run Forward", true);
                //}

            }

            if (enemyInHitRange)
            {
                enemyAnimation.SetBool("Run Forward", false);
                transform.position = transform.position;
                //print("eneted hit range");

                if(!hit) //stab animation
                {
                    StartCoroutine(PlayAnimation("Stab Attack"));
                    hit = true;

                    // ADD ATTACK CODE IN HERE

                }
                //yield return new WaitForSeconds(4);

                if(hit)
                {
                    enterting = false;
                    yield return new WaitForSeconds(1);
                    enemyAnimation.SetBool("Run Backward", true);

                    transform.position = Vector3.MoveTowards(transform.position,preJumpPos,3 * Time.deltaTime);
                    //yield return new WaitForSeconds(1);
                    yield return new WaitForSeconds(0.5f);
                    float dist = Vector3.Distance(preJumpPos, transform.position);

                    yield return new WaitForSeconds(0.5f);
                    if (dist < 3)
                    {
                        enemyAnimation.SetBool("Run Backward", false);
                        
                        alreadyAttacked = true;
                        Invoke(nameof(ResetAttack), timeBetweenAttacks);
                        
                    }
                    

                }
            }
        }

        if(alreadyAttacked)
        {
            
            orbiting = true;
        }

        
    }

    void OrbitPlayer()
    {
        enemyAnimation.SetBool("Run Backward", false);

        preJumpPosBool = false;
        float degreesPerSecond = 10;

        if (!rotateDetermined)
        {
            //determine left or right
            rotate = RandomFloatBetwenTwoInts(0, 2);
            //determine speed of rotation
            degreesPerSecond = RandomFloatBetwenTwoFloats(9, 21);
            rotateDetermined = true;
        }

        if (rotate == 0) OrbitPlayerLeft(degreesPerSecond);
        if (rotate == 1) OrbitPlayerRight(degreesPerSecond);

        //print(rotate);


        
    }


    private void ResetOrbit()
    {
        orbiting = false;
        
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
        jumped = false;
        hit = false;
    }
    private void OrbitPlayerLeft(float _degreesPerSecond)
    {  
        transform.RotateAround(player.transform.position, Vector3.up, _degreesPerSecond * Time.deltaTime);
        enemyAnimation.SetBool("Strafe Right", false);
        enemyAnimation.SetBool("Strafe Left", true);
    }

    private void OrbitPlayerRight(float _degreesPerSecond)
    {

        transform.RotateAround(player.transform.position, -Vector3.up, _degreesPerSecond * Time.deltaTime);
        enemyAnimation.SetBool("Strafe Left", false);
        enemyAnimation.SetBool("Strafe Right", true);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        StartCoroutine(PlayAnimation("Take Damage"));
        if (health <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
    }

    private void DestroyEnemy()
    {
        StartCoroutine(PlayAnimation("Die"));
        Destroy(gameObject);
    }

    IEnumerator PlayAnimation(string _animName)
    {
        //print(_animName);

        enemyAnimation.SetBool(_animName, true);
        yield return new WaitForSeconds(enemyAnimation.GetCurrentAnimatorStateInfo(0).length);
        enemyAnimation.SetBool(_animName, false);
    }

    //visualise sight range
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(player.transform.position, playerHitRange);


    }
}
