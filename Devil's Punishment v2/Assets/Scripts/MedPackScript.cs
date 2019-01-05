using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedPackScript : MonoBehaviour
{
    [SerializeField]
    private float health = 0f;

    [SerializeField]
    private float timeToApplyCompletely = 0f;

    public void UseItem(GameObject player)
    {
        HealthScript healthScript = player.GetComponent<HealthScript>();
        if (timeToApplyCompletely <= 0)
        {
            healthScript.IncreaseHealth(health);
        }
        else
        {
            healthScript.IncreaseHealth(health, timeToApplyCompletely);
        }
    }
}
