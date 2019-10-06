using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable {

    string Prompt();
    GameObject GetGameObject();
    void OnFocus();
    void OnReleaseFocus();
    void OnInteract();
    float TimeToInteract();
    Item GetGunItem();
    void SetPlayerController(PlayerController playerController);

}
