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

    ///<summary>
    ///The direction of the opening for the L corridor near door of type stored in this list
    ///</summary>
    public List<string> nearDoorL = new List<string>();
    public GameObject corridorT, corridorX;

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
        nearDoorL.Add("-z");
        nearDoorL.Add("-x");
        nearDoorL.Add("+z");
        nearDoorL.Add("+x");
    }

    private void Update()
    {
        if(isFirstTime && Time.time - startTime > 2f)
        {
            Debug.Log("Count Olaf =" + collidedCorridors.Count);
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
                        if(collidedCorridors[i].transform.rotation != collidedCorridors[j].transform.rotation)
                        {
                            GameObject currCorridor1 = Instantiate(corridorX, collidedCorridors[j].transform.position, Quaternion.identity);
                            Destroy(collidedCorridors[j].transform.parent.gameObject);
                        }
                        else
                        {

                        }
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
            Debug.Log("Count Olaf AFTER =" + collidedCorridors.Count);
        }
    }

    // --------------------- Converts corridorOpening indices/numbers into yRotations for corridor prefabs ---------------------
    public float convert(List<int> corridorOpenings)
    {
        corridorOpenings.Sort();
        float yRotation = 0;
        //DeadEnd
        if (corridorOpenings.Count == 1)
        {
            yRotation = 90 * corridorOpenings[0];
        }
        // I or L
        else if(corridorOpenings.Count == 2)
        {
            // I
            if((corridorOpenings[0] == 0 && corridorOpenings[1] == 2))// || (corridorOpenings[1] == 0 && corridorOpenings[0] == 2))
            {
                yRotation = 0;
            }
            // I
            else if(corridorOpenings[0] == 1 && corridorOpenings[1] == 3)
            {
                yRotation = 90;
            }
            // L
            else
            {
                yRotation = 90 * corridorOpenings[0];
                if(corridorOpenings[0] == 0 && corridorOpenings[1] == 3)
                {
                    yRotation = 270;
                }
            }
        }
        else if (corridorOpenings.Count == 3)
        {
            //T
        }
        else if (corridorOpenings.Count == 4)
        {
            //Xz
        }
        return yRotation;
    }

    //Search index of the required door
    public int neardoorLIndexSearch(string door)
    {
        for (int i = 0; i < nearDoorL.Count; i++)
        {
            if (nearDoorL[i].Equals(door))
            {
                return i;
            }
        }
        return -1;
    }
}
