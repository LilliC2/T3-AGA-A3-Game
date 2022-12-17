using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enemy;

public class Boss : Singleton<Boss>
{
    float bossHealth = 100;
    float bossMaxHealth = 100;
    float bossDamage = 10;
    float waitBetweenAttacks = 5;
    float vunerableTime = 5;

    float attackRange;
    bool playerInBossRange;
    bool alreadyAttacked;
    public LayerMask whatIsPlayer;

    Animator bossAnim;

    public enum BossState
    {
        Vunerable, Idle, SpinAttack, StabAttack, SummonRoof, Die
    }

    BossState bossState;

    // Start is called before the first frame update
    void Start()
    {
        bossAnim = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        //check if player is in range
        playerInBossRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        //when player in range, begin boss fight 
        if (playerInBossRange) bossState = BossState.Idle;

        switch(bossState)
        {
            case BossState.Idle:
                StartCoroutine(IdleBoss());
                break;
            case BossState.SpinAttack:
                SpinAttack();
                break;
            case BossState.StabAttack:
                StabAttack();
                break;
            case BossState.Vunerable:
                break;
            case BossState.SummonRoof:
                SummonRoof();
                break;
            case BossState.Die:
                break;
        }

        //check boss health
        //time between attacks decrases lower the health
        AdjustDifficultyBasedOnHealth();
    }


    IEnumerator IdleBoss()
    {
        //idle animation bool true

        //wait time between attacks
        yield return new WaitForSeconds(waitBetweenAttacks);

        //randomise which attack goes
        int r = RandomIntBetwenTwoInts(0, 1);

        switch(r)
        {
            //spin attack
            case 0:
                bossState = BossState.SpinAttack;
                break;

            //stab attack
            case 1:
                bossState = BossState.StabAttack;
                break;
        }

    }

    void SpinAttack()
    {
        //spin animation

        //after return to idle
    }

    void StabAttack()
    {
        //stab animation
        //after return to idle
    }

    IEnumerator Vunerble()
    {
        //lying down animation bool

        yield return new WaitForSeconds(vunerableTime);

        
        if (bossHealth <= 0)
        {
            bossState = BossState.Die;
        }
        else
        {
            //getting up animation
            bossState = BossState.SummonRoof;
        }

    }

    void SummonRoof()
    {
        //roar animation
        //stun player 

        //this will be handles in boss fight manager
        //roof resummon
        //turn off lights

        bossState = BossState.Idle;

    }

    IEnumerator Die()
    {
        //trigger death animation
        //tell boss fight manger player has won
        yield return new WaitForSeconds(6.5f);
        Destroy(gameObject);
    }

    void AdjustDifficultyBasedOnHealth()
    {
        float bossHealthPercent = (bossHealth / bossMaxHealth) * 100;

        if(bossHealthPercent <= 50 && bossHealthPercent > 25)
        {
            waitBetweenAttacks = 4;
            vunerableTime = 4;
        }
        if(bossHealthPercent <= 25)
        {
            waitBetweenAttacks = 3;
            vunerableTime = 3;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Sword"))
        {
            //only if player is attacking and boss is vunerable
            if (_P.playerState == ThirdPersonMovement.PlayerState.Attack && bossState == BossState.Vunerable)
            {
                bossHealth -= _P.playerDamage;

            }
        }
    }

}
