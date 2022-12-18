using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFightManager : Singleton<BossFightManager>
{
    public Animator bossFightAnim;
    public GameObject victoryCamera;

    // Start is called before the first frame update
    void Start()
    {
        bossFightAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
  

    }

    public void MakeBossVunerable()
    {
        _B.bossState = Boss.BossState.Vunerable;
        _B.vunerable = true;
    }

    public void Victory()
    {
        _P.playerMovement = ThirdPersonMovement.PlayerMovement.FPS;
        OpenRoof();
        victoryCamera.SetActive(true);
        _UI.Victory();
    }

    public void OpenRoof()
    {
        print("OpenRoof");
        AnimationTrigger("OpenRoof");
        MakeBossVunerable();
    }

    public void CloseRoof()
    {
        print("CloseRoof");
        AnimationTrigger("CloseRoof");
    }

    public void BossDeath()
    {
        Victory();
    }
}
