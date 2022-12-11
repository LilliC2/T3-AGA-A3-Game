using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystals : GameBehaviour
{
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Sword"))
        {
            //only take damage if player is attacking
            if (_P.playerState == ThirdPersonMovement.PlayerState.Attack)
            {
                StartCoroutine(GlowStartUp());
            }
        }
    }

    IEnumerator GlowStartUp()
    {
        PlayAnimationTrigger("LightUp");
        yield return new WaitForSeconds(1);
        anim.SetBool("Glow", true);
    }
}
