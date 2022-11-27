using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseController : GameBehaviour
{
    public GameObject pausePanel;
    bool isPaused = false;

    void Start()
    {
        pausePanel.SetActive(false);
        isPaused = false;
        Time.timeScale = 1; //means we are running at real time, setting it at 2 means time runs as twice as fast etc. setting timescale to 0 is a pause
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }

   public void Pause()
    {
        Cursor.lockState = isPaused ? CursorLockMode.Locked : CursorLockMode.None;
        isPaused = !isPaused; //flip switch
        pausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1; //if isPaused is true, timeScale 0 else 1
    }

   public void ExitGame()
    {
        Application.Quit();
    }
}
