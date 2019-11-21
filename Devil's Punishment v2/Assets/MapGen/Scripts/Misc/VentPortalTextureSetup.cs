using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VentPortalTextureSetup : MonoBehaviour
{
    public Camera ventCam;
    public Material ventCamMat;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitForPlayerJoin());
    }

    private IEnumerator WaitForPlayerJoin()
    {
        yield return new WaitUntil(() => Player.instance != null);
        ventCam = Player.instance.transform.GetChild(1).GetChild(1).GetComponent<Camera>();
        if (ventCam.targetTexture != null)
        {
            ventCam.targetTexture.Release();
        }

        ventCam.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        ventCamMat.mainTexture = ventCam.targetTexture;
    }
}
