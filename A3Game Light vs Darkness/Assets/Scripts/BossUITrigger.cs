using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossUITrigger : GameBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //turns on UI
            _UI.BossHealthBar();
            //begins boss fight
            _B.bossState = Boss.BossState.Idle;
        }
    }
}
