using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InvAmmoDisplay : MonoBehaviour
{
    public TextMeshProUGUI clipText;
    public TextMeshProUGUI clipStockText;
    GunController gunController;

    void Awake(){
        gunController = GunController.instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		//gameObject.SetActive(Inventory.instance !=null && Inventory.instance.equippedGun != null);
        int clip = gunController.GetClip();

        if(clip == 0) {
            clipText.color = Color.red;
        } else if(clip == gunController.GetClipSize()) {
            clipText.color = Color.green;
        } else {
            clipText.color = Color.white;
        }
        clipText.text = clip + "";

        clipStockText.text = gunController.GetClipStock() + "";
        
    }
}
