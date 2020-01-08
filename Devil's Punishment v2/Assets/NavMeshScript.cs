using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshScript : MonoBehaviour
{
    #region Singleton
    public static NavMeshScript instance;
    public NavMeshSurface surface;
    // Start is called before the first frame update
    void OnEnable()
    {
        surface = transform.GetComponent<NavMeshSurface>();
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;

            DontDestroyOnLoad(gameObject);
        }
    }

    #endregion

    public void updateNavMesh()
    {
        surface.BuildNavMesh();
    }


}
