using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("GuidanceStone")]
    public GameObject guidanceStoneTextBox;
    public TextMeshProUGUI guidanceText;
    public GameObject interactPrompt;

    [Header("BossFight")]
    public GameObject bossHealthUI;
    public Image bossHealthBar;

    [Header("PlayerHUD")]
    public TextMeshProUGUI killCountText;
    public Image playerHealthBar;
    public GameObject playerUI;

    [Header("Win/Lose")]
    public GameObject vicotryScreen;
    public GameObject loseScreen;

    // Start is called before the first frame update
    void Start()
    {
        bossHealthBar.fillAmount = 200;
    }

    // Update is called once per frame
    void Update()
    {

        UpdatePlayerHealthBar();
    }

    public void UpdatePlayerHealthBar()
    {
        float playerHealthPercent = _P.playerHealth / 100;

        playerHealthBar.fillAmount = playerHealthPercent;
    }

    public void UpdateKillCount()
    {
        killCountText.SetText(_P.killCount.ToString());
    }
    

    public void UpdateBossHealthBar(float _bossHealth)
    {
        float bossHealthPercent = (_bossHealth / 500);


        bossHealthBar.fillAmount = bossHealthPercent;
    }

    public void GuidanceStoneUpdate(string _text)
    {
        guidanceText.SetText(_text);
        
    }

    public void Victory()
    {
        vicotryScreen.SetActive(true);
        playerUI.SetActive(false);
        bossHealthUI.SetActive(false);

    }

    public void GameOver()
    {
        loseScreen.SetActive(true);
        playerUI.SetActive(false);
        bossHealthUI.SetActive(false);
    }

    public void InteractPromptOn()
    {
        interactPrompt.SetActive(true);
        
    }
    public void InteractPromptOff()
    {
        interactPrompt.SetActive(false);

    }


    public void BossHealthBar()
    {
        bossHealthUI.SetActive(true);
    }
}
