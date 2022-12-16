using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [Header("GuidanceStone")]
    public GameObject guidanceStoneTextBox;
    public TextMeshProUGUI guidanceText;
    public GameObject interactPrompt;

    public Animator guidanceStoneAnim;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    public void GuidanceStoneUpdate(string _text)
    {
        guidanceText.SetText(_text);
        StartCoroutine(GuidanceStoneStartUp());
    }

    public IEnumerator GuidanceStoneStartUp()
    {
        print("guidance start up");

        guidanceStoneAnim.SetTrigger("StoneOn");

        yield return new WaitForSeconds(4);

        guidanceStoneAnim.SetTrigger("StoneOff");
    }

    public void InteractPrompt(bool _inRange)
    {
        if (_inRange)
        {
            interactPrompt.SetActive(true);
        }
        else interactPrompt.SetActive(false);
    }
}
