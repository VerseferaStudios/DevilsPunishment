using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisRoomController : MonoBehaviour
{
    private BoxCollider boxCollider;
    private int roomBoundsIndex;

    private Bounds bounds;

    public Transform topRight, topLeft, bottomRight, bottomLeft;

    //----------------------

    public Color color = Color.green;

    private Vector3 v3FrontTopLeft;
    private Vector3 v3FrontTopRight;
    private Vector3 v3FrontBottomLeft;
    private Vector3 v3FrontBottomRight;
    private Vector3 v3BackTopLeft;
    private Vector3 v3BackTopRight;
    private Vector3 v3BackBottomLeft;
    private Vector3 v3BackBottomRight;

    //--------------------
    
        
        
    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        roomBoundsIndex = Data.instance.roomBounds.Count;
        bounds = boxCollider.bounds;
        Data.instance.roomBounds.Add(boxCollider.bounds);
        StartCoroutine(MoveRoomDown());
    }

    private void Update()
    {
        //CalcPositons(boxCollider.bounds);
        //DrawBox();
    }

    private IEnumerator MoveRoomDown()
    {
        bool isBreak = false;
        int j = 0;
        Debug.Log(isBreak + "Down");
        while (!isBreak)
        {
            transform.parent.localPosition += new Vector3(4, 0, 0);
            yield return new WaitForSeconds(0.2f);
            for (int i = 0; i < Data.instance.roomBounds.Count; i++)
            {
                if (i == roomBoundsIndex)
                {
                    Debug.Log("5");
                    continue;
                }
                if (boxCollider.bounds.Intersects(Data.instance.roomBounds[i]))
                {
                    if (i == 0)
                    {
                        Debug.Log("1");
                        isBreak = true;
                        StartCoroutine(MoveRoomRight());
                        break;
                    }
                    if (i == 1)
                    {
                        Debug.Log("6");
                        continue;
                    }
                    if (Data.instance.roomBounds[i].center.z - transform.parent.position.z > 10)
                    {
                        Debug.Log("2");
                        continue;
                    }
                    Debug.Log("8");
                    isBreak = true;
                    StartCoroutine(MoveRoomRight());
                    break;
                }
            }
            yield return new WaitForSeconds(0.2f);
            ++j;
            if (j > 1000) break;
        }
    }

    private IEnumerator MoveRoomRight()
    {
        bool isBreak = false;
        int j = 0;
        Debug.Log(isBreak + "Right");
        while (!isBreak)
        {
            transform.parent.localPosition += new Vector3(0, 0, 4);
            yield return new WaitForSeconds(0.2f);
            for (int i = 0; i < Data.instance.roomBounds.Count; i++)
            {
                if (i == roomBoundsIndex)
                {
                    Debug.Log("7");
                    continue;
                }
                if (boxCollider.bounds.Intersects(Data.instance.roomBounds[i]))
                {
                    if(i == 1)
                    {
                        Debug.Log("3");
                        isBreak = true;
                        Data.instance.isFixedTetrisRoom = true;
                        break;
                    }
                    if (i == 0)
                    {
                        Debug.Log("8");
                        continue;
                    }
                    if(Data.instance.roomBounds[i].center.x - transform.parent.position.x > 10)
                    {
                        Debug.Log("4");
                        continue;
                    }
                    Debug.Log("10");
                    isBreak = true;
                    Data.instance.isFixedTetrisRoom = true;
                    break;
                }
            }
            yield return new WaitForSeconds(0.2f);
            ++j;
            if (j > 1000) break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawCube(bounds.center, bounds.size);
        /*
        Gizmos.DrawLine(topLeft.position, topRight.position);
        Gizmos.DrawLine(topRight.position, bottomRight.position);
        Gizmos.DrawLine(bottomRight.position, bottomLeft.position);
        Gizmos.DrawLine(bottomLeft.position, topLeft.position);


        Mesh mesh = new Mesh();
        mesh.bounds = bounds;
        Vector3[] normals = new Vector3[3];
        normals[0] = Vector3.right;
        normals[1] = Vector3.up;
        normals[2] = Vector3.forward;
        mesh.normals = normals;
        
        //mesh.normals = 
        Gizmos.DrawMesh(mesh);
        */
    }

    void CalcPositons(Bounds bounds)
    {
        //Bounds bounds;
        //BoxCollider bc = GetComponent<BoxCollider>();
        //if (bc != null)
        //    bounds = bc.bounds;
        //else
        //return;
        /*
        Vector3 v3Center = bounds.center;
        Vector3 v3Extents = bounds.extents;

        v3FrontTopLeft = new Vector3(v3Center.x - v3Extents.x, v3Center.y + v3Extents.y, v3Center.z - v3Extents.z);  // Front top left corner
        v3FrontTopRight = new Vector3(v3Center.x + v3Extents.x, v3Center.y + v3Extents.y, v3Center.z - v3Extents.z);  // Front top right corner
        v3FrontBottomLeft = new Vector3(v3Center.x - v3Extents.x, v3Center.y - v3Extents.y, v3Center.z - v3Extents.z);  // Front bottom left corner
        v3FrontBottomRight = new Vector3(v3Center.x + v3Extents.x, v3Center.y - v3Extents.y, v3Center.z - v3Extents.z);  // Front bottom right corner
        v3BackTopLeft = new Vector3(v3Center.x - v3Extents.x, v3Center.y + v3Extents.y, v3Center.z + v3Extents.z);  // Back top left corner
        v3BackTopRight = new Vector3(v3Center.x + v3Extents.x, v3Center.y + v3Extents.y, v3Center.z + v3Extents.z);  // Back top right corner
        v3BackBottomLeft = new Vector3(v3Center.x - v3Extents.x, v3Center.y - v3Extents.y, v3Center.z + v3Extents.z);  // Back bottom left corner
        v3BackBottomRight = new Vector3(v3Center.x + v3Extents.x, v3Center.y - v3Extents.y, v3Center.z + v3Extents.z);  // Back bottom right corner

        v3FrontTopLeft = transform.TransformPoint(v3FrontTopLeft);
        v3FrontTopRight = transform.TransformPoint(v3FrontTopRight);
        v3FrontBottomLeft = transform.TransformPoint(v3FrontBottomLeft);
        v3FrontBottomRight = transform.TransformPoint(v3FrontBottomRight);
        v3BackTopLeft = transform.TransformPoint(v3BackTopLeft);
        v3BackTopRight = transform.TransformPoint(v3BackTopRight);
        v3BackBottomLeft = transform.TransformPoint(v3BackBottomLeft);
        v3BackBottomRight = transform.TransformPoint(v3BackBottomRight);
        */
    }

    void DrawBox()
    {
        /*
        //if (Input.GetKey (KeyCode.S)) {
        Debug.DrawLine(v3FrontTopLeft, v3FrontTopRight, color);
        Debug.DrawLine(v3FrontTopRight, v3FrontBottomRight, color);
        Debug.DrawLine(v3FrontBottomRight, v3FrontBottomLeft, color);
        Debug.DrawLine(v3FrontBottomLeft, v3FrontTopLeft, color);

        Debug.DrawLine(v3BackTopLeft, v3BackTopRight, color);
        Debug.DrawLine(v3BackTopRight, v3BackBottomRight, color);
        Debug.DrawLine(v3BackBottomRight, v3BackBottomLeft, color);
        Debug.DrawLine(v3BackBottomLeft, v3BackTopLeft, color);

        Debug.DrawLine(v3FrontTopLeft, v3BackTopLeft, color);
        Debug.DrawLine(v3FrontTopRight, v3BackTopRight, color);
        Debug.DrawLine(v3FrontBottomRight, v3BackBottomRight, color);
        Debug.DrawLine(v3FrontBottomLeft, v3BackBottomLeft, color);
        //}
        */
    }

}
