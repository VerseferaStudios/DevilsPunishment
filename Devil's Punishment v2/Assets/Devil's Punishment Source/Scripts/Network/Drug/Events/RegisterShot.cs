using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisterShot : MonoBehaviour
{


    public void registerShot(Vector3 start, Vector3 end)
    {
        LineRenderer randy = GetComponent<LineRenderer>();
        LineRenderer newRenderer = Instantiate(randy);
        newRenderer.SetPosition(0, start);
        newRenderer.SetPosition(1, end);
        Destroy(newRenderer, 3f);

    }
}
