using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystals : GameBehaviour
{
    Animator anim;
    public bool lit;
    public bool bossCrystal = false;

    // Start is called before the first frame update
    void Start()
    {
        lit =false;
        anim = GetComponent<Animator>();
    }

    private void Update()
    {

        if (!lit) StartCoroutine(GlowTurnOff());
        if (bossCrystal && _B.bossState == Boss.BossState.SummonRoof) ResetCrystal();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Sword"))
        {
            //only take damage if player is attacking
            if (_P.playerState == ThirdPersonMovement.PlayerState.Attack)
            {
                lit = true;
                if (lit) StartCoroutine(GlowStartUp());
            }
        }    

        
    }

    void ResetCrystal()
    {
       lit = false;
    }

    IEnumerator GlowStartUp()
    {
        _P.playerHealth += 30;
        AnimationTrigger("LightUp");
        yield return new WaitForSeconds(1);
        anim.SetBool("Glow", true);
    }

    IEnumerator GlowTurnOff()
    {
        AnimationTrigger("TurnOff");
        yield return new WaitForSeconds(1);
        anim.SetBool("Glow", false);
    }

    public bool LitCheck()
    {
        return lit;
    }
}
