using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RoomNewVents))]
public class EditorScriptRoomNewVents : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        RoomNewVents roomNewVents = target as RoomNewVents;

        if(GUILayout.Button("Find SquareGrid details"))
        {
            Vector3 pos = roomNewVents.sqGridDetails.position;
            int[] idx = roomNewVents.GetIdx(pos);
            PrintSQDetails(idx, roomNewVents.squareGrid);
            Debug.Log("position = " + roomNewVents.GetPos(idx));
        }

    }

    private void PrintSQDetails(int[] idx, SquareGrid sqGrid)
    {
        Cell cell = sqGrid.tiles[idx[0], idx[1]];
        Debug.Log("Square Grid details => ");
        Debug.Log("corridorIdx = " + cell.corridorIdx);
        Debug.Log("corridorYRot = " + cell.corridorYRot);
        Debug.Log("childEulerZ = " + cell.childEulerZ);
        Debug.Log("childEulerX = " + cell.childEulerX);
        Debug.Log("idx[0] = " + idx[0]);
        Debug.Log("idx[1] = " + idx[1]);
        Debug.Log("tiletype = " + cell.tile);
        Debug.Log("cell.allowedDIRS[0] = " + cell.disallowedDIRS[0].vector3());
        Debug.Log("cell.allowedDIRS[1] = " + cell.disallowedDIRS[1].vector3());
    }

}
/*
private string checkCollisions(Vector3 From, Vector3 to)
{
    int ctr = 0;
    bool goToNext = false;
    Vector3 FromTemp = From;
    Vector3 targetPos = new Vector3(FromTemp.x, 0.5f, to.z);
    //From to targetPos, x constant
    for (int i = 0; i < Data.instance.allRooms.Count; i++)
    {
        ////Debug.Log(((int[])Data.instance.allRooms[i])[1]);
        if(Mathf.Abs(-((int[])Data.instance.allRooms[i])[1] - FromTemp.x) != Data.instance.xSize/2 + 1 && Mathf.Abs(-((int[])Data.instance.allRooms[i])[1] - FromTemp.x) < Data.instance.xSize)
        {
            ctr++;
            goToNext = true;
            break;
        }
    }
    if (goToNext)
    {
        //targetPos to to, z constant
        FromTemp = targetPos;
        targetPos = to;
        int i = 0;
        for (i = 0; i < Data.instance.allRooms.Count; i++)
        {
            if (Mathf.Abs(-((int[])Data.instance.allRooms[i])[0] - FromTemp.z) != Data.instance.zSize/2 + 1 && Mathf.Abs(-((int[])Data.instance.allRooms[i])[0] - FromTemp.z) < Data.instance.zSize)
            {
                goToNext = true;
                break;
            }
        }
        if (i == Data.instance.allRooms.Count)
            return "xz";
    }
    if (goToNext)
    {
        targetPos = new Vector3(to.x, 0.5f, FromTemp.z);
        FromTemp = From;
        //z const
        for (int i = 0; i < Data.instance.allRooms.Count; i++)
        {
            if (Mathf.Abs(-((int[])Data.instance.allRooms[i])[0] - FromTemp.z) != Data.instance.zSize/2 + 1 && Mathf.Abs(-((int[])Data.instance.allRooms[i])[0] - FromTemp.z) < Data.instance.zSize)
            {
                goToNext = true;
                break;
            }
        }
    }
    if (goToNext)
    {
        //x const
        FromTemp = targetPos;
        targetPos = to;
        int i = 0;
        for (i = 0; i < Data.instance.allRooms.Count; i++)
        {
            if (Mathf.Abs(-((int[])Data.instance.allRooms[i])[1] - FromTemp.x) != Data.instance.xSize/2 + 1 && Mathf.Abs(-((int[])Data.instance.allRooms[i])[1] - FromTemp.x) < Data.instance.xSize)
            {
                goToNext = true;
                break;
            }
        }
        if (i == Data.instance.allRooms.Count)
            return "zx";
    }
    if (goToNext)
        return "No";
    else
        return "No";
}
*/
// Update is called once per frame



/*
 * //Colour scheme
            if (k == 2)
            {
                spawnPoints[k].GetComponent<Renderer>().material.color = Color.green;
                spawnPoints[k + 1].GetComponent<Renderer>().material.color = Color.green;
            }
            else if (k == 4)
            {
                spawnPoints[k].GetComponent<Renderer>().material.color = Color.red;
                spawnPoints[k + 1].GetComponent<Renderer>().material.color = Color.red;
            }
            else if (k == 6)
            {
                spawnPoints[k].GetComponent<Renderer>().material.color = Color.white;
                spawnPoints[k + 1].GetComponent<Renderer>().material.color = Color.white;
            }
            else if (k == 8)
            {
                spawnPoints[k].GetComponent<Renderer>().material.color = Color.yellow;
                spawnPoints[k + 1].GetComponent<Renderer>().material.color = Color.yellow;
            }
            else if (k == 10)
            {
                spawnPoints[k].GetComponent<Renderer>().material.color = Color.cyan;
                spawnPoints[k + 1].GetComponent<Renderer>().material.color = Color.cyan;
            }
            */
