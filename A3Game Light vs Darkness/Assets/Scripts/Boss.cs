using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enemy;

public class Boss : Singleton<Boss>
{
    [Header("Boss Stats")]
    public float bossHealth = 500;
    public float bossMaxHealth = 500;
    float bossDamage = 20;
    float waitBetweenAttacks = 10;
    float vunerableTime = 10;

    [Header("Attacks")]
    public float attackRange = 20;
    bool alreadyAttacked;
    public LayerMask whatIsPlayer;
    public bool vunerable;
    bool randomAttackTrue;

    bool killAdded = false;

    public GameObject bossSpinCollider;
    public GameObject bossStabCollider;
    public GameObject hitCollider;

    bool roofSummoned;

    [Header("Player")]
    public GameObject player;

    Animator bossAnim;
    bool deathAnim;

    public enum BossState
    {
        Vunerable, Idle, SpinAttack, StabAttack, SummonRoof, Die
    }

    public BossState bossState;

    // Start is called before the first frame update
    void Start()
    {
        bossAnim = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        //update UI with boss health
        _UI.UpdateBossHealthBar(bossHealth);

        //check if player is in range

        //when player in range, begin boss fight 
        //if (playerInBossRange) bossState = BossState.Idle;

        switch (bossState)
        {
            case BossState.Idle:
                roofSummoned = false;
                alreadyAttacked = false;
                StartCoroutine(IdleBoss());
                break;
            case BossState.SpinAttack:
                if(!alreadyAttacked) StartCoroutine(SpinAttack());
                break;
            case BossState.StabAttack:
                
                if(!alreadyAttacked) StartCoroutine(StabAttack());

                break;
            case BossState.Vunerable:
                if (vunerable) StartCoroutine(Vunerble());

                break;
            case BossState.SummonRoof:
                if(!roofSummoned) StartCoroutine(SummonRoof());
                break;
            case BossState.Die:
                if(!deathAnim) StartCoroutine(Die());

                break;
        }


            //check boss health
            //time between attacks decrases lower the health
            //AdjustDifficultyBasedOnHealth();
    }


    IEnumerator IdleBoss()
    {
        
        randomAttackTrue = false;

        bossSpinCollider.SetActive(false);
        bossStabCollider.SetActive(false);
        //idle animation bool true
        //wait time between attacks

        yield return new WaitForSecondsRealtime(waitBetweenAttacks);
        
        
            //randomise which attack goes
            if (!randomAttackTrue)
            {
                randomAttackTrue = true;
                int r = RandomIntBetwenTwoInts(0, 2);


                switch (r)
                {
                    //spin attack
                    case 0:
                        randomAttackTrue = true;
                        bossState = BossState.SpinAttack;

                        break;

                    //stab attack
                    case 1:

                        randomAttackTrue = true;
                        bossState = BossState.StabAttack;
                        break;
                }
            



        
             }
    }

    IEnumerator SpinAttack()
    {
        alreadyAttacked = true;
        
        bossSpinCollider.SetActive(true);
        AnimationTrigger("SpinAttack");
        bossSpinCollider.SetActive(false);
        yield return new WaitForSeconds(waitBetweenAttacks);

        
        bossState = BossState.Idle;

        //after return to idle
    }

    IEnumerator StabAttack()
    {
        this.transform.LookAt(player.transform.position);
        alreadyAttacked = true;
        bossStabCollider.SetActive(true);
        AnimationTrigger("StabAttack");
        bossStabCollider.SetActive(false);
        yield return new WaitForSeconds(waitBetweenAttacks);

        
        bossState = BossState.Idle;
        //after return to idle
    }

    IEnumerator Vunerble()
    {
        
        vunerable = false;
        AnimationTrigger("Vunerable");
        yield return new WaitForSeconds(0.5f);
        bossAnim.SetBool("OnGround", true);
        yield return new WaitForSeconds(vunerableTime);


        if (bossHealth <= 0)
        {
            bossState = BossState.Die;
        }
        else
        {
            bossAnim.SetBool("OnGround", false);
            bossState = BossState.SummonRoof;
        }

    }

    IEnumerator SummonRoof()
    {
        hitCollider.SetActive(false);
        print("Summon Roof");
        roofSummoned = true;
        AnimationTrigger("SummonRoof");
        //stun player 

        yield return new WaitForSeconds(1f);
        //roof resummon
        _BFM.CloseRoof();

        bossState = BossState.Idle;

    }

    IEnumerator Die()
    {
        deathAnim = true;
        //trigger death animation
        bossAnim.SetBool("Die", true);
        //tell boss fight manger player has won
        _BFM.BossDeath();
        if(!killAdded)
        {
            _P.killCount++;
            killAdded = true;
        }
        
        _UI.UpdateKillCount();
        yield return new WaitForSeconds(6.5f);

    }

    void AdjustDifficultyBasedOnHealth()
    {
        float bossHealthPercent = (bossHealth / bossMaxHealth) * 100;

        if (bossHealthPercent <= 50 && bossHealthPercent > 25)
        {
            print("Less than 50");
            waitBetweenAttacks = 4;
            vunerableTime = 7;
        }
        if (bossHealthPercent <= 25)
        {
            print("Less than 25");
            waitBetweenAttacks = 3;
            vunerableTime = 5;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Sword"))
        {
            //only if player is attacking and boss is vunerable
            if (_P.playerState == ThirdPersonMovement.PlayerState.Attack && bossState == BossState.Vunerable)
            {
                print("Boss Hit");
                bossHealth -= _P.playerDamage;
                _UI.UpdateBossHealthBar(bossHealth);

            }
        }

        if (other.gameObject.CompareTag("Player"))
        {
            if(bossState != BossState.Vunerable)
            {
                
                _P.PlayerTakeDamage(bossDamage);
            }
            
        }
    }


}
