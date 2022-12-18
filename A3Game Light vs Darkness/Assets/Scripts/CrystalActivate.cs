using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalActivate : GameBehaviour
{
    public GameObject[] crystalsRequiredToActivate;
    public List<bool> crystalsLit;
    public GameObject openGameobject;
    public bool allLit;
    public bool bossActive;
    bool openedRoof;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < crystalsRequiredToActivate.Length; i++)
        {
            crystalsLit.Add(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int j = 0; j != crystalsRequiredToActivate.Length; j++)
        {
            crystalsLit[j] = crystalsRequiredToActivate[j].GetComponent<Crystals>().LitCheck();
            if (crystalsLit[j] == true)
            {
                CheckIfListIsTrue();
            }

            if (!allLit && j == crystalsRequiredToActivate.Length) j = 0;

        }

        if(allLit && !bossActive) openGameobject.SetActive(false);
        if(!openedRoof && allLit && bossActive)
        {
            if (allLit && bossActive) _BFM.OpenRoof();
            openedRoof = true;
        }
        

    }

    void CheckIfListIsTrue()
    {
        bool checkTrue = true;

        for (int i = 0; i < crystalsLit.Count; i++)
        {
            if (crystalsLit[i] == false)
            {
                checkTrue = false;
                openedRoof = false;
            }
        }

        if (checkTrue) allLit = true;
    }

}
