using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartScene : MonoBehaviour
{
    public void restartScene()
    {
        //Data.instance.collisionCount = Data.instance.corridorCount = 0;
        Data.instance.allRooms = new ArrayList();
        Data.instance.startTime = Time.time;
        //Data.instance.isCollided = false;
        Data.instance.collidedCorridors = new List<GameObject>();
        Data.instance.corridorPosDict = new Dictionary<Vector3, int>();
        Data.instance.connectedRoomsThroughCollision = new List<ConnectedComponent>();
        Data.instance.connectedRooms = new List<List<Vector3>>();
        Data.instance.connectedRoomsVents = new List<List<Vector3>>();
        Data.instance.count =   0;
        Data.instance.isOnce = true;
        Data.instance.isDonePrevFnCall = true;
        Data.instance.ctr = 0;
        Data.instance.prevCount = 0;

        Data.instance.isFinishedAddAndRemoveConnectedRooms = Data.instance.isFinishedCheckCollisionsVents = Data.instance.isFinishedCheckCollisions = Data.instance.isConnectedComponentsCheckDone = false;
        Data.instance.temp = new List<ConnectedComponent>();
        Data.instance.counter = Data.instance.counter1 = 0;

        //Random.InitState(10);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //StartCoroutine(Data.instance.DoCheckPerSecond());
        //StartCoroutine(Data.instance.DoConnectedComponents());
    }
}
