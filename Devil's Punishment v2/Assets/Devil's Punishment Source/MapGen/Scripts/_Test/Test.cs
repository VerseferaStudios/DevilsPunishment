using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DebugCall();
        StartCoroutine(DebugDelayCall());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected abstract void DebugCall();

    private IEnumerator DebugDelayCall()
    {
        while (true)
        {
            yield return new WaitForSeconds(2);
            Debug.Log("from base class");
            DebugCall();
        }
    }
}
