using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableDoors : InteractableLoot
{
    /*
    public override void OnInteract()
    {
        StartCoroutine(OpenVentCover());
    }
    */
    private IEnumerator OpenVentCover()
    {
        while (transform.localEulerAngles.z >= 89) //get sth better
        {
            transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, transform.localEulerAngles + new Vector3(0, 0, 90), Time.deltaTime);
            yield return new WaitForSeconds(0.1f);
        }
    }

}
