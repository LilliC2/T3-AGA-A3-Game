using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidanceStoneTrigger : GameBehaviour
{
    public GameObject guidanceStone;
    public bool playerInRange = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange)
        {
            PlayerInteract();
        }
        _UI.InteractPrompt(playerInRange);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInRange = true;
            print("Press I to interact");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInRange = false;
            
        }
    }

    public void PlayerInteract()
    {

        if(Input.GetKeyDown(KeyCode.I))
        {
            string myStone = guidanceStone.name;
            string text = _GSM.FindText(myStone);
            _UI.GuidanceStoneUpdate(text);
            _GSM.StartGS();

        }
    }
}
