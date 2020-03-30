using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Loader : MonoBehaviour
{
    public GameObject data, data2ndFloor;//, roomsLoaderPrefab;
    //public Button reloadMapGenBtn;

    private void Awake()
    {
        if(Data.instance == null)
        {
            Instantiate(data);
            //GameObject gb = Instantiate(roomsLoaderPrefab);
            //reloadMapGenBtn.onClick.AddListener(gb.GetComponent<MapGen3>().ReloadMapGen);
        }

        if(Data2ndFloor.instance == null)
        {
            Instantiate(data2ndFloor);
        }

      //  NavMeshScript.instance.updateNavMesh();
    }
}
