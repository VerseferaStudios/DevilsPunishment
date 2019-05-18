using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data : MonoBehaviour
{
    public static Data instance = null;
    public ArrayList allRooms = new ArrayList();
    public float xSize, zSize;
    public int collisionCount = 0, corridorCount = 0;
    public bool isCollided = false;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
}
