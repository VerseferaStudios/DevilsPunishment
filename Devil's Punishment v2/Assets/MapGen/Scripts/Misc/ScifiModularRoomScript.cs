using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScifiModularRoomScript : MonoBehaviour
{

    public GameObject ceiling;
    public GameObject floor;
    public GameObject wall;
    public GameObject wallWithDoor;
    public GameObject scifiDoor;
    public GameObject ventCover;

    //Room sizes according to floor tiles; So, 1 x 1 is 6.275 x 6.275
    private int x, y;

    // Start is called before the first frame update
    void Start()
    {
        FloornCeiling();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FloornCeiling()
    {
        x = Random.Range(1, 7);
        y = Random.Range(1, 7);



    }

}
