using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable {

    string Prompt();
    GameObject GetGameObject();
    void OnInteract();
    float TimeToInteract();


}
