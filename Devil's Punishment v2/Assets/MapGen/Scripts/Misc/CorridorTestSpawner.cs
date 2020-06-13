using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorridorTestSpawner : MonoBehaviour
{
    public GameObject corridorTest;
    private RoomReferencesStatic roomReferencesScript;
    private bool isLoopDone = false;

    // Start is called before the first frame update
    void Start()
    {
        roomReferencesScript = GetComponent<RoomReferencesStatic>();
        StartCoroutine(SpawnCorridorTest());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator SpawnCorridorTest()
    {
        yield return new WaitForSeconds(2f);
        while (!isLoopDone)
        {
            if (Data.instance.canStartCorridorTestSpawner)
            {
                for (int i = 0; i < roomReferencesScript.doors.Length; i++)
                {
                    Vector3 spawnPos = roomReferencesScript.doors[i].transform.position;
                    spawnPos.y = 0.5f + (Mathf.Abs(transform.position.y - Data2ndFloor.instance.floor2Height) < 2 ? Data2ndFloor.instance.floor2Height : 0);
                    GameObject testCorridorGB = Instantiate(corridorTest, spawnPos, Quaternion.identity, Data.instance.mapGenHolderTransform);
                    testCorridorGB.transform.GetChild(0).GetComponent<CorridorTest>().prevRoom = gameObject;
                }
                isLoopDone = true;
            }
            yield return new WaitForSeconds(3f);
        }
    }

}
