using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableButton : MonoBehaviour, IInteractable
{

    public string prompt = "Press button";
    public float timeToInteract = .5f;
    public UnityEvent onPressed;

    public string Prompt() {
        return prompt;
    }

    public float TimeToInteract() {
        return timeToInteract;
    }

    public GameObject GetGameObject() {
        return gameObject;
    }

    public void OnInteract() {
        Debug.Log("This button has been interacted with.");
        onPressed.Invoke();
    }

}
