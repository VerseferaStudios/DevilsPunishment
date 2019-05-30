using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCollision : MonoBehaviour
{
    public GameObject corridorI, current;
    public float startTime = 0f, nextTime = 0f, i = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - startTime > nextTime)
        {
            if(i == 2)
            {
                Destroy(current);
            }
            if(i < 2 || i == 3)
            {
                current = Instantiate(corridorI, new Vector3(0, 0, 0), Quaternion.identity);
            }
            nextTime = Time.time + 2f;
            ++i;
        }
    }
}
