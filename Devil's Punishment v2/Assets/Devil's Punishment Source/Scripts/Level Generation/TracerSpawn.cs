using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TracerSpawn : MonoBehaviour
{
    public int length, turns, quota;
    public RoomSpawn RS;
    private int seed;
    public Vector3 offsetX;
    public Vector3 offsetY;
    public Vector3 curpos;
    public int direction;
    public GameObject I, L1, L2, X, T;

    private void Awake()
    {
        RS = GameObject.Find("RoomSpawner").GetComponent<RoomSpawn>();
    }

    void Start()
    {
        seed = RS.seed;
        curpos = transform.position + offsetX;
        for (int i=0; i<quota; i++)
        {
            if(direction == 0)
            {                
                Vector3 temp = curpos;
                GameObject.Instantiate(I, curpos, transform.rotation);
                curpos = temp + offsetX;
            }
            if (direction == 1)
            {
                GameObject.Instantiate(I, transform.position - offsetX, transform.rotation);
            }
            if (direction == 2)
            {
                GameObject.Instantiate(I, transform.position + offsetY, transform.rotation);
            }
            if (direction == 3)
            {
                GameObject.Instantiate(I, transform.position - offsetY, transform.rotation);
            }
        }
    }
}
