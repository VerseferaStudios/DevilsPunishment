using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserCutter : MonoBehaviour
{
    public Player health;
    private int healthStart;
    private float timeToRelease = 15;
    public Player cuffed;
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
            else if (Input.GetKeyUp("Interact"))
            {
                timeToRelease = 15f;
            }
        }
        yield return null;
    }

    //public void OnTriggerEnter(Collider enemy)
    //{
    //    if ()
    //    {
    //        return;
    //    }
    //}

    // Update is called once per frame
    void Update()
    {
        
    }
}
