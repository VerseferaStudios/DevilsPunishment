using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisRoomController : MonoBehaviour
{
    private BoxCollider boxCollider;
    private int roomBoundsIndex;
    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        roomBoundsIndex = Data.instance.roomBounds.Count;
        Data.instance.roomBounds.Add(boxCollider.bounds);
        StartCoroutine(MoveRoomDown());
    }
    

    private IEnumerator MoveRoomDown()
    {
        bool isBreak = false;
        int j = 0;
        while (!isBreak)
        {
            transform.parent.localPosition += new Vector3(4, 0, 0);
            for (int i = 0; i < Data.instance.roomBounds.Count; i++)
            {
                if (i == roomBoundsIndex) break;
                if (boxCollider.bounds.Intersects(Data.instance.roomBounds[i]))
                {
                    if (i == 0)
                    {
                        StartCoroutine(MoveRoomRight());
                        isBreak = true;
                        break;
                    }
                    if (i == 1) continue;
                    if (Data.instance.roomBounds[i].center.z - transform.parent.position.z > 10)
                    {
                        continue;
                    }
                    StartCoroutine(MoveRoomRight());
                    isBreak = true;
                    break;
                }
            }
            yield return new WaitForSeconds(0.1f);
            ++j;
            if (j > 1000) break;
        }
    }

    private IEnumerator MoveRoomRight()
    {
        bool isBreak = false;
        int j = 0;
        while (!isBreak)
        {
            transform.parent.localPosition += new Vector3(0, 0, 4);
            for (int i = 0; i < Data.instance.roomBounds.Count; i++)
            {
                if (boxCollider.bounds.Intersects(Data.instance.roomBounds[i]))
                {
                    if(i == 1)
                    {
                        isBreak = true;
                        Data.instance.isFixedTetrisRoom = true;
                        break;
                    }
                    if (i == 0) continue;
                    if(Data.instance.roomBounds[i].center.x - transform.parent.position.x > 10)
                    {
                        continue;
                    }
                    if (i == roomBoundsIndex) break;
                    isBreak = true;
                    Data.instance.isFixedTetrisRoom = true;
                    break;
                }
            }
            yield return new WaitForSeconds(0.1f);
            ++j;
            if (j > 1000) break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }
}
