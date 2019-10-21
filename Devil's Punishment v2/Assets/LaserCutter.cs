using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserCutter : MonoBehaviour
{
    private float timeToRelease = 4;
    public Player cuffed;
    public BoxCollider enemyEnter;

    public GameObject arch1;
    public GameObject arch2;
    public GameObject arch3;
    public GameObject zap1;
    public GameObject zap2;
    public GameObject zap3;

    public Animation spinner;
    public Animation windDown;

    bool interacting;
    bool canInteract;

    public GameObject fauxBlock;
    public GameObject realBlock;
    public GameObject laser;

    float coolDown = 5f;
    public GameObject smoked;

    bool enemyContested;

    // Start is called before the first frame update
    void Start()
    {
        interacting = false;
        canInteract = true;
        fauxBlock.SetActive(true);
        realBlock.SetActive(false);
        laser.SetActive(false);
        enemyContested = false;
    }

    public void BeginSequences()
    {
        StartCoroutine("WindUp");
    }

    IEnumerator WindUp()
    {
        fauxBlock.SetActive(false);
        realBlock.SetActive(true);
        spinner.GetComponent<Animation>().Play("Spinner");
        yield return new WaitForSeconds(1.5f);
        arch1.SetActive(true);
        yield return new WaitForSeconds(.5f);
        arch1.SetActive(false);
        yield return new WaitForSeconds(.75f);
        arch2.SetActive(true);
        yield return new WaitForSeconds(.5f);
        arch2.SetActive(false);
        yield return new WaitForSeconds(.75f);
        arch3.SetActive(true);
        yield return new WaitForSeconds(.5f);
        arch3.SetActive(false);
        yield return new WaitForSeconds(1f);
        arch1.SetActive(true);
        arch2.SetActive(true);
        yield return new WaitForSeconds(.5f);
        arch1.SetActive(false);
        arch2.SetActive(false);
        yield return new WaitForSeconds(.75f);
        arch2.SetActive(true);
        arch3.SetActive(true);
        yield return new WaitForSeconds(.5f);
        arch2.SetActive(false);
        arch3.SetActive(false);
        yield return new WaitForSeconds(.75f);
        arch3.SetActive(true);
        arch1.SetActive(true);
        yield return new WaitForSeconds(.5f);
        arch3.SetActive(false);
        arch1.SetActive(false);
        yield return new WaitForSeconds(1f);
        arch1.SetActive(true);
        arch2.SetActive(true);
        arch3.SetActive(true);
        yield return new WaitForSeconds(.5f);
        zap1.SetActive(true);
        yield return new WaitForSeconds(.5f);
        zap2.SetActive(true);
        yield return new WaitForSeconds(.5f);
        zap3.SetActive(true);
        laser.SetActive(true);
        interacting = true;
        yield return null;
    }

    public void LaserCoolDown()
    {
        canInteract = false;
    }

    public void StopLaser()
    {
        interacting = false;
        fauxBlock.SetActive(true);
        windDown.Play("WindDown");
        realBlock.SetActive(false);
        arch1.SetActive(false);
        arch2.SetActive(false);
        arch3.SetActive(false);
        zap1.SetActive(false);
        zap2.SetActive(false);
        zap3.SetActive(false);
        laser.SetActive(false);
    }

    public void ResetLaser()
    {
        timeToRelease = 4f;
        StopAllCoroutines();
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyEnter.tag == "Enemy")
        {
            enemyContested = true;
            ResetLaser();
            interacting = false;
            canInteract = false;
        }
        else if (enemyEnter.tag != "Enemy")
        {
            enemyContested = false;
        }

        if (interacting && !enemyContested)
        {
            timeToRelease -= Time.deltaTime;
            if (timeToRelease <= 0)
            {
                cuffed.GetComponent<CuffController>().isCuffed = false;
                Debug.Log("Cuffs have been removed");
                StopLaser();
                LaserCoolDown();
            }
        }
        if (!canInteract)
        {
            smoked.SetActive(true);
            coolDown -= Time.deltaTime;
            if (coolDown <= 0)
            {
                canInteract = true;
                smoked.SetActive(false);
                coolDown = 5f;
            }
        }
    }

}
