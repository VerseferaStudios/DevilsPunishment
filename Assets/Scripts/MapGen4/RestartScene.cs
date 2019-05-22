using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartScene : MonoBehaviour
{
    public void restartScene()
    {
        Data.instance.collisionCount = Data.instance.corridorCount = 0;
        Data.instance.allRooms = new ArrayList();
        Data.instance.startTime = Time.time;
        Data.instance.isFirstTime = true;
        Data.instance.isCollided = false;
        Data.instance.collidedCorridors = new List<GameObject>();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
