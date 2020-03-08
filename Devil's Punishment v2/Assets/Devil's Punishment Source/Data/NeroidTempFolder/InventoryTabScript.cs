using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class InventoryTabScript : MonoBehaviour
{
    private int curTab;
    public GameObject IntentoryBut;
    public GameObject ObjectivesBut;
    public GameObject Objs;
    public GameObject Invs;
    void Start()
    {
        curTab = 0;
    }
    public void ChangeTab() // change to inventory or objectives
    {
        if (curTab == 0)
        {
            curTab = 1;
            IntentoryBut.SetActive(false);
            Invs.SetActive(false);
            ObjectivesBut.SetActive(true);
            Objs.SetActive(true);
 
        }
        else
        {
            curTab = 0;
            ObjectivesBut.SetActive(false);
            Objs.SetActive(false);
            IntentoryBut.SetActive(true);
            Invs.SetActive(true);
        } 
    }
}
