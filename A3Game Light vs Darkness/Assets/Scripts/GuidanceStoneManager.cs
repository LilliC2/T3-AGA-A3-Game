using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidanceStoneManager : Singleton<GuidanceStoneManager>
{
    public GameObject[] guidanceStonesList;
    public string[] guidanceStoneText;
    int index;
    public Animator guidanceStoneAnim;


    // Start is called before the first frame update
    void Start()
    {
        guidanceStoneAnim.GetComponent<Animator>();
        
        guidanceStoneText = new string[4];

        guidanceStoneText[0] = "Hit the Crystals with your sword to transfer light to them and heal you.";
        guidanceStoneText[1] = "Light can be transfered through enemies as well.";
        guidanceStoneText[2] = "Press 'Tab' to aim your light at the light circle and press 'E' to fire to throw yourself towards it.";
        guidanceStoneText[3] = "To defeat the boss, light up the crystals on the walls to weaken it, then strike.";
    }

    public string FindText(string _name)
    {
        print(_name);
        bool nameFound = false;

        if(!nameFound)
        {
            for (int i = 0; i < guidanceStonesList.Length; i++)
            {
                if (guidanceStonesList[i].name == _name)
                {
                    nameFound = true;
                    index = i;
                }
            }
        }

        print(guidanceStoneText[index]);

        return guidanceStoneText[index];
    }

    IEnumerator GuidanceStoneStartUp()
    {
        print("guidance start up");

        AnimationTrigger("StoneOn");

        yield return new WaitForSeconds(4);

        guidanceStoneAnim.SetTrigger("StoneOff");
    }

    public void StartGS()
    {
        StartCoroutine(GuidanceStoneStartUp());
    }

}
