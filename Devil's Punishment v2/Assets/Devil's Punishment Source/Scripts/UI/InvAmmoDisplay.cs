﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InvAmmoDisplay : MonoBehaviour
{
    public TextMeshProUGUI clipText;
    public TextMeshProUGUI clipStockText;
    public GunController gunController;
    public GameObject HandgunIcon;
    public GameObject ShotgunIcon;
    public GameObject AssaultRifleIcon;

    void Awake(){
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

    public void SetGunIcons(){
		HandgunIcon.SetActive(false);
		ShotgunIcon.SetActive(false);
		AssaultRifleIcon.SetActive(false);
        if (Inventory.instance.equippedGun != null){
            switch (Inventory.instance.equippedGun.weaponClassification)
            {
                case GunItem.WeaponClassification.HANDGUN:
                    HandgunIcon.SetActive(true);
                    break;
                case GunItem.WeaponClassification.SHOTGUN:
                    ShotgunIcon.SetActive(true);
                    break;
                case GunItem.WeaponClassification.ASSAULTRIFLE:
                    AssaultRifleIcon.SetActive(true);
                    break;

                default: // Pass
                    break;
            }
        }
    }
}
