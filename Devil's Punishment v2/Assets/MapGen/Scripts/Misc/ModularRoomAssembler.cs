using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Rectangular / Cuboidal room for now
public class ModularRoomAssembler : MonoBehaviour
{

    public Transform door_corridor_Transform;
    public Vector3 door_corridor_Pos;

    private Transform roomHolderTransform;
    private Transform walls_holder;
    private Transform floor_holder;
    private Transform ceiling_holder;
    
    public GameObject side_wall;
    public GameObject wall_with_door;
    public GameObject floor;
    public GameObject ceiling;
    public GameObject grill;
    [SerializeField]
    private int size_x, size_z;

    // Start is called before the first frame update
    void Start()
    {

        door_corridor_Pos = door_corridor_Transform.position;

        ChooseSize();
        DecideRoomOriginOrCorner_CreateHolders();
        PlaceFloor();
        PlaceWall();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ChooseSize()
    {
        size_x = Random.Range(20 / 4, 40 / 4);
        size_z = Random.Range(20 / 4, 40 / 4);
    }

    private void DecideRoomOriginOrCorner_CreateHolders()
    {
        Vector3 startFloorPos = new Vector3(door_corridor_Pos.x + Random.Range(-size_x * 4, 0), door_corridor_Pos.y, door_corridor_Pos.z - 4);
        new GameObject("startFloorPos").transform.position = startFloorPos;
        roomHolderTransform = new GameObject("Modular Room 1").transform;
        roomHolderTransform.position = startFloorPos;
        walls_holder = new GameObject("walls_holder").transform;
        floor_holder = new GameObject("floor_holder").transform;
        ceiling_holder = new GameObject("ceiling_holder").transform;
        walls_holder.parent = floor_holder.parent = ceiling_holder.parent = roomHolderTransform;
    }

    private void PlaceFloor()
    {
        for (int i = 0; i <= size_x; i++)
        {
            for (int j = 0; j <= size_z; j++)
            {
                Transform t = Instantiate(floor).transform;
                t.parent = floor_holder;
                t.localPosition = new Vector3(i * 4, 0, -j * 4);
            }
        }
    }

    private void PlaceWall()
    {
        int height = 2;
        Vector3 startWallPos = new Vector3(roomHolderTransform.position.x, roomHolderTransform.position.y, roomHolderTransform.position.z - 4 / 2);
        for (int i = 0; i <= size_x; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Transform t = Instantiate(side_wall).transform;
                t.parent = walls_holder;
                t.localPosition = new Vector3(i * 4, 2 + j * 4, 4 / 2);
                t.localEulerAngles = new Vector3(t.localEulerAngles.x, t.localEulerAngles.y + 90, t.localEulerAngles.z);

                t = Instantiate(side_wall).transform;
                t.parent = walls_holder;
                t.localPosition = new Vector3(i * 4, 2 + j * 4, -4 / 2 - size_z * 4);
                t.localEulerAngles = new Vector3(t.localEulerAngles.x, t.localEulerAngles.y + 90, t.localEulerAngles.z);
            }
        }
    }
}
