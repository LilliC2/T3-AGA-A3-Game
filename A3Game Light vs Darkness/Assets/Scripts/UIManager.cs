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
