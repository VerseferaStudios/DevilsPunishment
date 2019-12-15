using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VentPortalTextureSetup : MonoBehaviour
{
    public Camera ventCam;
    public Material ventCamMat;
    public GameObject ventCoverHolder;
    private MeshRenderer meshRenderer;
    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        StartCoroutine(WaitForPlayerJoin());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!ventCoverHolder.activeSelf && other.tag.StartsWith("Player"))
        {
            ventCam = other.transform.GetChild(1).GetChild(1).GetComponent<Camera>();
            if (ventCam.targetTexture != null)
            {
                ventCam.targetTexture.Release();
            }

            ventCam.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
            ventCamMat.mainTexture = ventCam.targetTexture;
            meshRenderer.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.StartsWith("Player"))
        {
            meshRenderer.enabled = false;
        }
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
