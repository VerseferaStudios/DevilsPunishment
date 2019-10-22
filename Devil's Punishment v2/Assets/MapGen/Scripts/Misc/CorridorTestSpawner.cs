using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorridorTestSpawner : MonoBehaviour
{
    public GameObject corridorTest;
    private RoomReferences roomReferencesScript;
    private bool isLoopDone = false;

    // Start is called before the first frame update
    void Start()
    {
        roomReferencesScript = GetComponent<RoomReferences>();
        StartCoroutine(SpawnCorridorTest());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator SpawnCorridorTest()
    {
        while (!isLoopDone)
        {
            if (Data.instance.canStartCorridorTestSpawner)
            {
                for (int i = 0; i < roomReferencesScript.doors.Length; i++)
                {
                    GameObject testCorridorGB = Instantiate(corridorTest, roomReferencesScript.doors[i].transform.position, Quaternion.identity, Data.instance.mapGenHolderTransform);
                    testCorridorGB.transform.GetChild(0).GetComponent<CorridorTest>().prevRoom = gameObject;
                }
                isLoopDone = true;
            }
            yield return new WaitForSeconds(3f);
        }
    }

}
