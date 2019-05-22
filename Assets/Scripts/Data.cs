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
    public List<GameObject> collidedCorridors = new List<GameObject>();

    public float startTime;
    public bool isFirstTime = true;
    private bool isBreakOuter = false;

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

    private void Start()
    {
        startTime = Time.time;
    }

    private void Update()
    {
        //Debug.Log("Count Olaf =================" + collidedCorridors.Count);
        if(isFirstTime && Time.time - startTime > 0.3f)
        {
            Debug.Log("----------------------wargarsg----------------------");
            for (int i = 0; i < collidedCorridors.Count; i++)
            {
                for (int j = i + 1; j < i + 4 && j < collidedCorridors.Count; j++)
                {
                    //Debug.Log(collidedCorridors[i].transform.position);
                    //Debug.Log(collidedCorridors[j].transform.position);

                    //if(collidedCorridors[i].transform.position == collidedCorridors[j].transform.position)
                    if(Mathf.Abs(collidedCorridors[i].transform.position.x - collidedCorridors[j].transform.position.x) <= 0.6f
                        && Mathf.Abs(collidedCorridors[i].transform.position.z - collidedCorridors[j].transform.position.z) <= 0.6f)
                    {
                        Destroy(collidedCorridors[i].transform.parent.gameObject);
                        collidedCorridors.RemoveAt(i);

                        if (j > i)
                        {
                            j--;
                        }

                        i--;

                        collidedCorridors.RemoveAt(j);

                        if(i > j)
                        {
                            i--;
                        }
                        j--;

                        isBreakOuter = true;
                        break;
                    }
                    
                    if (isBreakOuter)
                    {
                        isBreakOuter = false;
                        break;
                    }
                }
            }
            isFirstTime = false;
        }
    }
}
