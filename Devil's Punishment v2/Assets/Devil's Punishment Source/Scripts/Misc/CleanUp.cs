using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CleanUp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DestroyAllGameObjects();
        SceneManager.LoadScene(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DestroyAllGameObjects()
    {
        GameObject[] GameObjects = (FindObjectsOfType<GameObject>() as GameObject[]);

        for (int i = 0; i < GameObjects.Length; i++)
        {
            if (GameObjects[i].name != "CleanUp")
            {
                Destroy(GameObjects[i]);
            }
        }
    }
}
