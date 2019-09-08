using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserCutter : MonoBehaviour
{
    public Player health;
    private int healthStart;
    private float timeToRelease = 15;
    public Player cuffed;
    public BoxCollider enemyEnter;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("StartZappin");
        health.GetComponent<Health>().curHealth = healthStart;
    }

    IEnumerator StartZappin()
    {
        //as long as the players health doesnt drop below players health at the start and the interact button is being pressed
        while (Input.GetKeyDown("Interact"))
        {
            timeToRelease -= Time.deltaTime;
            if (timeToRelease <= 0)
            {
                cuffed.GetComponent<CuffController>().isCuffed = false;
                Debug.Log("Cuffs have been removed");
            }
            else if (Input.GetKeyUp("Interact") && timeToRelease <= 15f && timeToRelease >= 0f)
            {
                timeToRelease = 15f;
            }
        }
        yield return null;
    }

    public void OnTriggerEnter(Collider enemy)
    {
        if (tag == "Enemy")
        {
            timeToRelease = 15f;
            Input.GetKeyUp("Interact");
        }
    }

    public void OnTriggerStay(Collider enemy)
    {
       if (tag == "Enemy")
        {
            timeToRelease = 15f;
            Input.GetKeyUp("Interact");
        }
    }

    public void OnTriggerExit(Collider enemy)
    {
        if (tag == "Enemy")
        {
            
        }        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
