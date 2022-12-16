using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidanceStoneManager : Singleton<GuidanceStoneManager>
{
    public List<GameObject> guidanceStonesList;
    public string[] guidanceStoneText;
    int index;

    // Start is called before the first frame update
    void Start()
    {
        guidanceStonesList.Add(GameObject.FindWithTag("GuidanceStone"));
        
        guidanceStoneText = new string[10];

        guidanceStoneText[0] = "Hit the Crystals with your sword to transfer light to them";
        guidanceStoneText[1] = "Light can be transfered through enemies as well";
        guidanceStoneText[2] = "Press 'Tab' to aim your light at the light circle and press 'E' to fire to throw yourself towards it.";
    }

    public string FindText(string _name)
    {
        print(_name);
        bool nameFound = false;

        if(!nameFound)
        {
            for (int i = 0; i < guidanceStonesList.Count; i++)
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

}
