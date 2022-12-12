using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwap : Singleton<CameraSwap>
{
    public Camera followPlayer;
    public Camera fPCamera;


    // Start is called before the first frame update
    void Start()
    {
        followPlayer.enabled = true;
        fPCamera.enabled = false;
    }



    public void fPCameraOn()
    {
        followPlayer.enabled = false;
        fPCamera.enabled = true;
    }



    public void followPlayerCameraOn()
    {
        followPlayer.enabled = true;
        fPCamera.enabled = false;
    }
}

