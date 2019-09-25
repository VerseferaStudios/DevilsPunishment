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


    public GameObject arch1;
    public GameObject arch2;
    public GameObject arch3;
    public GameObject zap1;
    public GameObject zap2;
    public GameObject zap3;

    public Animation spinner;

    bool interacting;
    bool canInteract;

    // Start is called before the first frame update
    void Start()
    {
        interacting = false;
        canInteract = true;
    }
    //public void BeginSequences()
    //{
    //    if (Input.GetKeyDown("Interact"))
    //    {
    //        interacting = true;
    //        while (Input.GetKeyDown("Interact"))
    //        {
    //            spinner.Play("Spinner");
    //        }
    //    }
    //}

    IEnumerator WindUp()
    {
        yield return new WaitForSeconds(2f);
        arch1.SetActive(true);
        yield return new WaitForSeconds(.05f);
        arch1.SetActive(false);
        yield return new WaitForSeconds(1f);
        arch2.SetActive(true);
        yield return new WaitForSeconds(.05f);
        arch2.SetActive(false);
        yield return new WaitForSeconds(.05f);
        arch3.SetActive(true);
        yield return new WaitForSeconds(.05f);
        arch3.SetActive(false);
        yield return new WaitForSeconds(1.5f);
        arch1.SetActive(true);
        yield return new WaitForSeconds(.05f);
        arch1.SetActive(false);
        yield return new WaitForSeconds(.05f);
        arch2.SetActive(true);
        yield return new WaitForSeconds(.05f);
        arch2.SetActive(false);
        yield return new WaitForSeconds(.05f);
        arch3.SetActive(true);
        yield return new WaitForSeconds(.05f);
        arch3.SetActive(false);
        yield return new WaitForSeconds(1f);
        zap1.SetActive(true);
        arch1.SetActive(true);
        yield return new WaitForSeconds(.05f);
        arch1.SetActive(false);
        StartCoroutine("StartZappin");
        yield return null;
    }

    IEnumerator StartZappin()
    {
        //as long as the players health doesnt drop below players health at the start and the interact button is being pressed
        //laserBox.SetActive(true);
        while (interacting && Input.GetKeyDown("Interact"))
        {
            timeToRelease -= Time.deltaTime;
            if (timeToRelease <= 0)
            {
                cuffed.GetComponent<CuffController>().isCuffed = false;
                interacting = false;
                Debug.Log("Cuffs have been removed");
            }
            else if (Input.GetKeyUp("Interact") && timeToRelease <= 15f && timeToRelease >= 0f)
            {
                timeToRelease = 15f;
                interacting = false;
            }
        }
        yield return null;
    }


    public void OnTriggerEnter(Collider enemy)
    {
        if (tag == "Enemy")
        {
            //laserBox.SetActive(false);
            timeToRelease = 15f;
            Input.GetKeyUp("Interact");
            interacting = false;
            canInteract = false;
            spinner.Stop();
        }
    }

    public void OnTriggerStay(Collider enemy)
    {
       if (tag == "Enemy")
        {
            //laserBox.SetActive(false);
            spinner.Stop();
            timeToRelease = 15f;
            Input.GetKeyUp("Interact");
            interacting = false;
            canInteract = false;
        }
    }

    public void OnTriggerExit(Collider enemy)
    {
        if (tag == "Enemy")
        {
            canInteract = true;
        }        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("Interact"))
        {
            interacting = true;
            while (Input.GetKeyDown("Interact"))
            {
                spinner.Play("Spinner");
            }
        }
    }
}
