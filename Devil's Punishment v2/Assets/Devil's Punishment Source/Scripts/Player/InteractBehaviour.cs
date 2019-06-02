using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InteractBehaviour : MonoBehaviour
{

    public LayerMask interactableLayer;

    public Image interactableImage;
    public Image progressBarImage;
    public Image interactablePromptBackground;
    public TextMeshProUGUI interactablePrompt;

    private InteractableLoot focusedInteractable;
    private bool interactableInVicinity = false;

    private float interactMaxTime;
    private float interactElapsedTime;

    void Start() {
        InteractableUnFocus();
    }

    private void FixedUpdate()
    {
        //SearchForInteractablesInVicinity(); Skitz:: Didn't work to move it here
    }

    void Update() {

        SearchForInteractablesInVicinity();
        if (interactableInVicinity) {
            WhileInteractableFocused();
        }

    }

    void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 2.0f);
    }

    void SearchForInteractablesInVicinity() {

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2.0f, interactableLayer.value);
        if(hitColliders.Length > 0) {
            
            float maxDot = -1.0f;

            InteractableLoot interactableToFocus = null;
            
            foreach (Collider col in hitColliders) {

                InteractableLoot nearbyInteractable = col.GetComponent<InteractableLoot>();
                if(nearbyInteractable != null) {

                    Vector3 pickupToCam = col.transform.position - Camera.main.transform.position;
                    Vector3 camForward = Camera.main.transform.forward;

                    float dot = Vector3.Dot(pickupToCam.normalized, camForward.normalized);

                    if (dot > maxDot)
                    {                       
                        maxDot = dot;
                        interactableToFocus = nearbyInteractable;
                    }
                }

            }

            if (maxDot > .7f) {
                if(interactableToFocus != null && interactableToFocus != focusedInteractable) {
                    focusedInteractable = interactableToFocus;
                    InteractableFocus();
                }
            }
            else if (interactableInVicinity)
            {
                InteractableUnFocus();
            }

        } else {
            if(interactableInVicinity) {
                InteractableUnFocus();
            }
        }

    }

    void InteractableUnFocus() {
        interactableImage.gameObject.SetActive(false);
        interactablePromptBackground.gameObject.SetActive(false);
        interactableInVicinity = false;
        focusedInteractable = null;
        interactElapsedTime = 0;
        progressBarImage.fillAmount = 0;
    }

    void InteractableFocus() {
        interactableImage.gameObject.SetActive(true);
        interactablePromptBackground.gameObject.SetActive(true);
        interactableInVicinity = true;
        interactablePrompt.text = focusedInteractable.Prompt();
        interactMaxTime = focusedInteractable.TimeToInteract();
    }

	Inventory inventory;
	private bool canPickUp()
	{
		if (focusedInteractable.item is GunItem) return true;
		inventory = gameObject.transform.parent.transform.Find("Inventory").GetComponent<Inventory>();
		if (inventory == null)
		{
			Debug.Log("Error in InteractBehavior.canPickUp(). \"inventory\" should not be null");	
			return false;
		}
		return inventory.hasSpace();
	}
    void WhileInteractableFocused() {

        Vector3 iconPosition = focusedInteractable.GetGameObject().transform.position/* + Vector3.up * .3f*/;
        Vector2 screenPoint = Camera.main.WorldToViewportPoint(iconPosition);

        interactableImage.rectTransform.anchorMin = screenPoint;
        interactableImage.rectTransform.anchorMax = screenPoint;

        progressBarImage.fillAmount = (interactElapsedTime/interactMaxTime);
		if(canPickUp())
        if(interactElapsedTime >= interactMaxTime) {
            focusedInteractable.OnInteract();
            InteractableUnFocus();
        }

        if (Input.GetButton("Interact"))
        {
            interactElapsedTime += Time.deltaTime;
        } else {
            interactElapsedTime = 0;
        }

    }

}
