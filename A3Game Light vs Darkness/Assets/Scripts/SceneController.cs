using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void ChangeScene(string _sceneName)
    {
        SceneManager.LoadScene(_sceneName);
    }

    //Reloads the current scen we are in
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //loads out title scene. must be called Title exactly
    public void ToTitleScene()
    {
        SceneManager.LoadScene("Title");
    }

    //get our active scenes name
    public string GetSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
