using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFnsMapGen : MonoBehaviour
{

    public ArrayList allRooms = new ArrayList();
    protected float xSize = 48f, zSize = 48f;

    // ---------------------------- Call offset functions accordingly ----------------------------
    protected void CallOffsetAndDoorAndSqGridFns(GameObject spawnedRoom, float yRotation, RoomReferencesStatic roomReferences)
    {
        if (yRotation == 90)
        {
            ChangeDoorNames();
            //ChangeDoorNames(spawnedRoom, "Door+x");
            GiveOffsetToRoom(0.226f);
            //spawnedRoom.transform.localPosition = new Vector3(spawnedRoom.transform.localPosition.x + 0.226f,  //*
            //                                                  spawnedRoom.transform.localPosition.y,           //* This is for Start Room
            //                                                  spawnedRoom.transform.localPosition.z + 0.065f); //*


            //ExistingCorridorsFn(roomReferences, (int)yRotation, 0.226f);


            //Transform corridorsOfRoom = spawnedRoom.transform.GetChild(2);

            //for (int z = 0; z < corridorsOfRoom.childCount; z++)
            //{
            //    corridorsOfRoom.GetChild(z).localPosition = new Vector3(0, 0, 0.226f);
            //}


        }
        else if (yRotation == 180 || yRotation == 270 || yRotation == -90)
        {
            float reqYRotationForCorridor = 0;
            if (yRotation == 180)
            {
                ChangeDoorNames();
                //ChangeDoorNames(spawnedRoom, "Door-z");
                GiveOffsetToRoom(-0.08f);
                //ExistingCorridorsFn(roomReferences, (int)yRotation, -0.08f);
                reqYRotationForCorridor = 0;

                //-----------------234567-----------------
                //if (spawnedRoom.name.Equals("End Room(Clone)"))
                {
                    spawnedRoom.transform.GetChild(0).localPosition = new Vector3(spawnedRoom.transform.GetChild(0).localPosition.x - 0.303f, spawnedRoom.transform.GetChild(0).localPosition.y, spawnedRoom.transform.GetChild(0).localPosition.z + 0.31f);
                }

            }
            else if (yRotation == 270 || yRotation == -90)
            {
                ChangeDoorNames();
                //ChangeDoorNames(spawnedRoom, "Door-x");
                GiveOffsetToRoom(0.226f);
                //spawnedRoom.transform.localPosition = new Vector3(spawnedRoom.transform.localPosition.x + 0.226f,  //*
                //                                                  spawnedRoom.transform.localPosition.y,           //* This is for Start Room
                //                                                  spawnedRoom.transform.localPosition.z - 0.065f); //*

                //ExistingCorridorsFn(roomReferences, (int)yRotation, 0.226f);

                //Transform corridorsOfRoom = spawnedRoom.transform.GetChild(2);
                //for (int z = 0; z < corridorsOfRoom.childCount; z++)
                //{
                //    corridorsOfRoom.GetChild(z).localPosition = new Vector3(0, 0, 0.226f);
                //}


                reqYRotationForCorridor = 90;

                //-----------------234567-----------------
                //if (spawnedRoom.name.Equals("End Room(Clone)"))
                {
                    spawnedRoom.transform.GetChild(0).localPosition = new Vector3(spawnedRoom.transform.GetChild(0).localPosition.x - 0.31f, spawnedRoom.transform.GetChild(0).localPosition.y, spawnedRoom.transform.GetChild(0).localPosition.z - 0.303f);
                }

            }

            for (int j = 0; j < spawnedRoom.transform.GetChild(2).childCount; j++)
            {
                spawnedRoom.transform.GetChild(2).GetChild(j).rotation = Quaternion.Euler(0, reqYRotationForCorridor, 0);
            }



        }
        //probably +z....
        else
        {
            GiveOffsetToRoom(-0.08f);
            //ExistingCorridorsFn(roomReferences, (int)yRotation, -0.08f);
        }


        // ---------------------------- Shift/Give offset to room prefab correctly ----------------------------
        void GiveOffsetToRoom(float offset)
        {
            spawnedRoom.transform.GetChild(0).localPosition = new Vector3(spawnedRoom.transform.GetChild(0).localPosition.x, spawnedRoom.transform.GetChild(0).localPosition.y, spawnedRoom.transform.GetChild(0).localPosition.z + offset);
            //Transform corridorsOfRoomParent = spawnedRoom.GetChild(2);
            //for (int i = 0; i < corridorsOfRoomParent.childCount; i++)
            //{
            //    corridorsOfRoomParent.GetChild(i).GetChild(0).localPosition = new Vector3(0, 0, offset);
            //}
        }


        // -------------------- Change door names --------------------
        void ChangeDoorNames()
        {
            GameObject[] doors = spawnedRoom.GetComponent<RoomReferencesStatic>().doors;
            for (int i = 0; i < doors.Length; i++)
            {
                Debug.Log("doorRot ; yRotInt = " + ((int)yRotation / 90) % 4);
                spawnedRoom.GetComponent<RoomReferencesStatic>().doors[i].name = FindDoorName(((int)yRotation / 90) % 4, spawnedRoom.GetComponent<RoomReferencesStatic>().doors[i].name);
            }
        }

        string FindDoorName(int yRotInt, string oldName)
        {
            if (yRotInt == 0)
            {
                return oldName;
            }
            else
            {
                //yRotInt = Mathf.Abs(yRotInt);
                //Debug.Log("doorRot ; oldName 4 and 5 idx = " + (oldName[4] + "" + oldName[5]));
                int idx = Data.instance.doorRotationHelper.IndexOf((oldName[4] + "" + oldName[5]).ToString());
                //Debug.Log("doorRot ; idx b4 = " + idx);
                idx += yRotInt;
                //Debug.Log("doorRot ; idx b/w = " + idx);
                idx %= 4;
                if (idx < 0)
                {
                    idx += 4;
                }
                //Debug.Log("doorRot ; idx after = " + idx);
                return "Door" + Data.instance.doorRotationHelper[idx];
            }
        }
    }


    protected void DoorHelper2(RoomReferencesStatic roomReferencesStatic, bool isDevMode
        , ref SquareGrid squareGrid, ref List<Transform> cubes, GameObject testGridCube)
    {

        float tempPosVal;
        if (roomReferencesStatic.topRightItemGen.position.x > roomReferencesStatic.bottomLeftItemGen.position.x)
        {

        }
        else
        {
            tempPosVal = roomReferencesStatic.topRightItemGen.position.x;
            roomReferencesStatic.topRightItemGen.position = new Vector3(roomReferencesStatic.bottomLeftItemGen.position.x
                                                                 , roomReferencesStatic.topRightItemGen.position.y
                                                                 , roomReferencesStatic.topRightItemGen.position.z);
            roomReferencesStatic.bottomLeftItemGen.position = new Vector3(tempPosVal
                                                                 , roomReferencesStatic.bottomLeftItemGen.position.y
                                                                 , roomReferencesStatic.bottomLeftItemGen.position.z);

            tempPosVal = roomReferencesStatic.topRightMapGen.position.x;
            roomReferencesStatic.topRightMapGen.position = new Vector3(roomReferencesStatic.bottomLeftMapGen.position.x
                                                                 , roomReferencesStatic.topRightMapGen.position.y
                                                                 , roomReferencesStatic.topRightMapGen.position.z);
            roomReferencesStatic.bottomLeftMapGen.position = new Vector3(tempPosVal
                                                                 , roomReferencesStatic.bottomLeftMapGen.position.y
                                                                 , roomReferencesStatic.bottomLeftMapGen.position.z);

        }
        if (roomReferencesStatic.topRightItemGen.position.z > roomReferencesStatic.bottomLeftItemGen.position.z)
        {

        }
        else
        {
            tempPosVal = roomReferencesStatic.topRightItemGen.position.z;
            roomReferencesStatic.topRightItemGen.position = new Vector3(roomReferencesStatic.topRightItemGen.position.x
                                                                 , roomReferencesStatic.topRightItemGen.position.y
                                                                 , roomReferencesStatic.bottomLeftItemGen.position.z);
            roomReferencesStatic.bottomLeftItemGen.position = new Vector3(roomReferencesStatic.bottomLeftItemGen.position.x
                                                                 , roomReferencesStatic.bottomLeftItemGen.position.y
                                                                 , tempPosVal);

            tempPosVal = roomReferencesStatic.topRightMapGen.position.z;
            roomReferencesStatic.topRightMapGen.position = new Vector3(roomReferencesStatic.topRightMapGen.position.x
                                                                 , roomReferencesStatic.topRightMapGen.position.y
                                                                 , roomReferencesStatic.bottomLeftMapGen.position.z);
            roomReferencesStatic.bottomLeftMapGen.position = new Vector3(roomReferencesStatic.bottomLeftMapGen.position.x
                                                                 , roomReferencesStatic.bottomLeftMapGen.position.y
                                                                 , tempPosVal);

        }

        //this.roomReferences = roomReferencesStatic;

        for (int q = -Mathf.CeilToInt(roomReferencesStatic.topRightMapGen.position.x / 4); q <= -Mathf.FloorToInt(roomReferencesStatic.bottomLeftMapGen.position.x / 4); q++)
        {
            //Debug.Log("|");
            for (int r = -Mathf.CeilToInt(roomReferencesStatic.topRightMapGen.position.z / 4); r <= -Mathf.FloorToInt(roomReferencesStatic.bottomLeftMapGen.position.z / 4); r++)
            {
                //Debug.Log("-");

                squareGrid.tiles[q, r].tile = TileType.Wall;
                if (isDevMode)
                {
                    cubes.Add(Instantiate(testGridCube, new Vector3(q * -4, 0, r * -4), Quaternion.identity).transform);
                    //Instantiate(testGridCube, new Vector3(q * -4, 0, r * -4), Quaternion.identity);
                }
            }
        }
    }

    // --------------------------------- Checks for collisions between ROOMS ---------------------------------
    protected bool NoCollisions(float[] arr)
    {
        for (int i = 0; i < allRooms.Count; i++)
        {
            if ((Mathf.Abs(arr[0] - ((float[])allRooms[i])[0]) < xSize) && (Mathf.Abs(arr[1] - ((float[])allRooms[i])[1]) < zSize))
            {
                return false;
            }
        }
        return true;
    }

}
