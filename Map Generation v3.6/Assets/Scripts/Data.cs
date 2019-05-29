using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Data : MonoBehaviour
{
    public static Data instance = null;
    public ArrayList allRooms = new ArrayList();
    public float xSize, zSize;
    public int collisionCount = 0, corridorCount = 0;
    public bool isCollided = false;
    public List<GameObject> collidedCorridors = new List<GameObject>();

    public float startTime;
    private bool isNotFirstTime = false;

    ///<summary>
    ///The direction of the opening for the L corridor near door of type stored in this list
    ///</summary>
    public List<string> nearDoorL = new List<string>();
    public GameObject corridorT1, corridorT2, corridorX;

    private float nextTime = 1f;

    private Dictionary<Vector3, int> corridorPosDict = new Dictionary<Vector3, int>();

    //public bool isPipeAtLeft = true;

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

        if(collidedCorridors.Count != 0 && Time.time - startTime > nextTime)
        {
            Debug.Log("Count Olaf =" + collidedCorridors.Count);


            //FindDuplicates(); //use this to group corridors at the same place use ConvertToOpenings and Linq.Distinct and do the necessary


            Debug.Log("----------------------wargarsg----------------------");
            for (int i = 0; i < collidedCorridors.Count; i++)
            {
                if(collidedCorridors[i] == null)
                {
                    collidedCorridors.RemoveAt(i);
                    i--;
                    continue;
                }
                for (int j = 0; /*j < i + 4 &&*/ j < collidedCorridors.Count; j++)
                {
                    if (i == j) continue;

                    if (collidedCorridors[j] == null)
                    {
                        collidedCorridors.RemoveAt(j);
                        if (i > j)
                        {
                            i--;
                        }
                        j--;
                        continue;
                    }

                    /*
                    if (collidedCorridors[i] == null)
                    {
                        collidedCorridors.RemoveAt(i);
                        i--;
                        continue;
                    }
                    */

                    //Debug.Log(collidedCorridors[i].transform.position);
                    //Debug.Log(collidedCorridors[j].transform.position);

                    if (collidedCorridors[i].transform.position == collidedCorridors[j].transform.position)
                    //if(Mathf.Abs(collidedCorridors[i].transform.position.x - collidedCorridors[j].transform.position.x) <= 0.6f
                    //    && Mathf.Abs(collidedCorridors[i].transform.position.z - collidedCorridors[j].transform.position.z) <= 0.6f)
                    {
                        //Make condition perfect er

                        if (collidedCorridors[i].transform.parent.name.Equals(collidedCorridors[j].transform.parent.name) 
                            && (collidedCorridors[i].transform.rotation == collidedCorridors[j].transform.rotation))
                        {
                            Debug.Log("Leave");

                        }
                        else if(!isNotFirstTime)
                        {

                            //Debug.Log("in at " + collidedCorridors[i].transform.position);
                            //Debug.Log(collidedCorridors[i].transform.parent.name + " " + collidedCorridors[i].transform.rotation.eulerAngles);
                            //Debug.Log(collidedCorridors[j].transform.parent.name + " " + collidedCorridors[j].transform.rotation.eulerAngles);
                            List<int> openings1 = new List<int>(), openings2 = new List<int>();
                            openings1 = ConvertToOpenings(collidedCorridors[i].transform.parent.name, collidedCorridors[i].transform.rotation.eulerAngles.y);
                            openings2 = ConvertToOpenings(collidedCorridors[j].transform.parent.name, collidedCorridors[j].transform.rotation.eulerAngles.y);
                            //Debug.Log(openings1[0] + " " + openings1[1]);
                            //Debug.Log(openings2[0] + " " + openings2[1]);
                            /*
                            for (int i = 0; i < openings1.Count; i++)
                            {
                                for (int j = 0; j < openings2.Count; j++)
                                {
                                    if (i == j) continue;
                                    if(openings1[i] == openings2[j])
                                    {

                                    }
                                }
                            }
                            */
                            openings1.AddRange(openings2);


                            openings1 = openings1.Distinct<int>().ToList<int>();


                            if (openings1.Count == 3)
                            {
                                float yRotation = ConvertToRotation(openings1);
                                GameObject currCorridor = Instantiate((yRotation == 0 || yRotation == -90) ? corridorT2 : corridorT1 , collidedCorridors[j].transform.position, Quaternion.identity);
                                if(yRotation == 0 || yRotation == 90)
                                {
                                    currCorridor.transform.localScale = new Vector3(-1, 1, 1);
                                }
                                currCorridor.transform.rotation = Quaternion.Euler(0, yRotation, 0);
                            }
                            if(openings1.Count == 4)
                            {
                                Instantiate(corridorX, collidedCorridors[j].transform.position, Quaternion.identity);
                            }

                            /*
                            //If I and I collides with different rotations
                            if (collidedCorridors[i].transform.rotation != collidedCorridors[j].transform.rotation)
                            {
                                GameObject currCorridor1 = Instantiate(corridorX, collidedCorridors[j].transform.position, Quaternion.identity);
                            }
                            //If I and L collides
                            else if (!collidedCorridors[i].transform.parent.name.Equals(collidedCorridors[j].transform.parent.name))
                            {
                                //T
                            }
                            else
                            {

                            }
                            */

                            Destroy(collidedCorridors[i].transform.parent.gameObject);
                        }

                        Destroy(collidedCorridors[j].transform.parent.gameObject);

                        // !!!!!!!!!!!!!!!!! Take care of collisions that happen when the above corridors (T and X) are instantiated, if any !!!!!!!!!!!!!!!!!
                        if (!isNotFirstTime)
                        {
                            collidedCorridors.RemoveAt(i);

                            if (j > i)
                            {
                                j--;
                            }

                            i--;
                        }

                        collidedCorridors.RemoveAt(j);

                        if(!isNotFirstTime && i > j)
                        {
                            i--;
                        }
                        j--;

                        isNotFirstTime = true;
                        break;
                    }
                    
                }

                if (isNotFirstTime)
                {
                    isNotFirstTime = false;
                }
            }

            if (collidedCorridors.Count == 1)
            {
                Destroy(collidedCorridors[0].transform.parent.gameObject);
            }
            
            Debug.Log("Count Olaf AFTER =" + collidedCorridors.Count);
            for (int q = 0; q < collidedCorridors.Count; q++)
            {
                Debug.Log(collidedCorridors[q].transform.position + " " + collidedCorridors[q].transform.parent.name);
            }

            nextTime = Time.time + 1f;

        }

    }

    // --------------------- Converts corridorOpening indices/numbers into yRotations for corridor prefabs ---------------------
    public float ConvertToRotation(List<int> corridorOpenings)
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
        //T
        else if (corridorOpenings.Count == 3)      //Check if it works-------------------------
        {
            List<int> oneToFour = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                oneToFour.Add(i);
            }
            for (int i = 0; i < corridorOpenings.Count; i++)
            {
                oneToFour.Remove(corridorOpenings[i]);
            }
            yRotation = oneToFour[0] * 90;
        }
        //X
        else if(corridorOpenings.Count == 4)
        {
            yRotation = 0;
        }
        return yRotation;
    }

    //Search index of the required door
    public int NeardoorLIndexSearch(string door)
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

    //Check if it works-------------------------
    public List<int> ConvertToOpenings(string name, float yRotation)
    {
        List<int> openings = new List<int>();
        if (name.Equals("Corridor_I(Clone)"))
        {
            openings.Add((int)(yRotation / 90f));
            openings.Add(openings[0] + 2);
            Debug.Log(openings[0] + " " + openings[1]);
        }
        else if (name.Equals("Corridor_L_1(Clone)"))
        {
            if(yRotation == 270)
            {
                openings.Add(0);
                openings.Add(3);
            }
            else
            {
                openings.Add((int)(yRotation / 90f));
                openings.Add(openings[0] + 1);
            }
        }
        else if (name.Equals("Corridor_T_.1(Clone)"))
        {
            List<int> oneToFour = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                oneToFour.Add(i);
            }
            oneToFour.Remove((int)(yRotation / 90f));
            openings.AddRange(oneToFour);
        }
        else if (name.Equals("Corridor_X_1(Clone)"))
        {
            List<int> oneToFour = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                oneToFour.Add(i);
            }
            openings.AddRange(oneToFour);
        }
        return openings;
    }

    private void FindDuplicates()
    {
        foreach (GameObject gb in collidedCorridors)
        {
            if (!corridorPosDict.ContainsKey(gb.transform.position))
            {
                corridorPosDict.Add(gb.transform.position, 1);
            }
            else
            {
                int count = 0;
                corridorPosDict.TryGetValue(gb.transform.position, out count);
                corridorPosDict.Remove(gb.transform.position);
                corridorPosDict.Add(gb.transform.position, count + 1);
            }
        }

        Debug.Log("Dictionary of duplicates!!!!!!!!!!!!!");
        foreach (KeyValuePair<Vector3, int> item in corridorPosDict)
        {
            Debug.Log(item.Key + " " + item.Value);
        }
    }
}
