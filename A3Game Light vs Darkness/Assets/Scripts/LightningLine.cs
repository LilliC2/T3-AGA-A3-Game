using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningLine : GameBehaviour
{
    public enum LightningState
    {
        Normal, Lightning
    }

    public LightningState lightningState;

    // Start is called before the first frame update
    void Start()
    {
        _P.lightingBounce.enabled = false;

    }

    // Update is called once per frame
    void Update()
    {
        AOELightAttack();
    }

    void AOELightAttack()
    {
        print("Call lightning");
        int inRangeCount = _P.lightingEnemyTargets.Count + 1;

        _P.lightingBounce.positionCount = inRangeCount;

        //print(lightingBounce.positionCount = inRangeCount);

        _P.lightingBounce.SetPosition(0, transform.position);

        int j = 1;

        for (int i = 0; i < inRangeCount; i++)
        {



            if (j! > i)
            {
                _P.lightingBounce.SetPosition(j, _P.lightingEnemyTargets[i].transform.position);

            }
            else
            {
                j--;
            }

            j++;
        }

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            _P.lightingBounce.enabled = true;
        }


    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {

            StartCoroutine(WaitForLighting());


        }
    }

    IEnumerator WaitForLighting()
    {
        yield return new WaitForSeconds(1);
        _P.lightingBounce.enabled = false;
    }
}
