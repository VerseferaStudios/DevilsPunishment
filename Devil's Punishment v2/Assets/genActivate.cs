using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class genActivate : MonoBehaviour
{
    // Start is called before the first frame update

    public genPart genPartToPut;

    public enum genPart
    {
        A,
        B,
        C
    }

    public GameObject genPartObject;

    
    public void Start()
    {
        genPartObject = transform.Find("GenPart").gameObject;
    }

    //TODO:Maybe add insert animation
    private void activateGen()
    {
        genPartObject.SetActive(true);
        //Job is done, remove interactable
        Destroy(transform.Find("Interactable").gameObject);
    }

    //Referenced on Interactable Button on GeneratorHolder Interactable
    public void checkActivation()
    {
        switch (genPartToPut)
        {
            case genPart.A:
                if(Inventory.instance.ContainsItem("Generator Part A"))
                {
                    Inventory.instance.inventory.RemoveAt(10);
                    GameState.gameState.addState(GameState.gameStateType.GenPartA);
                    activateGen();
                    
                }
                break;
                
            case genPart.B:
                if (Inventory.instance.ContainsItem("Generator Part B"))
                {
                    Inventory.instance.inventory.RemoveAt(11);
                    GameState.gameState.addState(GameState.gameStateType.GenPartB);
                    activateGen();

                }
                break;

            case genPart.C:
                if (Inventory.instance.ContainsItem("Generator Part C"))
                {
                    Inventory.instance.inventory.RemoveAt(12);
                    GameState.gameState.addState(GameState.gameStateType.GenPartC);
                    activateGen();

                }
                break;
            default:
            break;
            
            
            
                




        }
    }

}
