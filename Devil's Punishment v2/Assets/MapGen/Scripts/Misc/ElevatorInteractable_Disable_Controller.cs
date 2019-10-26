using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorInteractable_Disable_Controller : MonoBehaviour
{
    public BoxCollider[] interactableElevatorColliders;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnableOrDisableInteractables(bool isEnabled)
    {
        for (int i = 0; i < interactableElevatorColliders.Length; i++)
        {
            interactableElevatorColliders[i].enabled = isEnabled;
        }
    }

}
