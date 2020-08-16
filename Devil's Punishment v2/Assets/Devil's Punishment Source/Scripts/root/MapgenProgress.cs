using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapgenProgress : MonoBehaviour
{

    #region Singleton
    public static MapgenProgress instance;
    public TextMeshProUGUI percentageGUI;
    public Image panel;
    public int percentage = 0;

    void Start()
    {

        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;

            DontDestroyOnLoad(gameObject);
        }
    }
    #endregion

    private void OnEnable()
    {
        RoomNewVents.Floor1VentsDone += _100PerCentLoad;
    }

    private void OnDisable()
    {
        RoomNewVents.Floor1VentsDone -= _100PerCentLoad;
    }

    public void addProgress(int prog)
    {


        //prog = (int)(prog / 116f * 100f);
        //percentage += prog;
        //percentageGUI.text = percentage.ToString() + "%";



        /*
        float val = 255 - percentage * (255 / 100);
        Color a = new Color(val, 0, val, val);
        panel.CrossFadeColor(a, 1f, false, true);
        panel.gameObject.GetComponent<CanvasGroup>().alpha = 1-percentage * .01f;
        */

    }

    private void _100PerCentLoad()
    {
        percentageGUI.text = "100%";
        percentageGUI.color = Color.blue;
    }

    public void loadedMap()
    {
        //percentage = 100;


        //percentageGUI.text = percentage.ToString() + "%";


        /*
        float val = 255 - percentage * (255 / 100);
        Color a = new Color(val, 0, val, val);
        panel.CrossFadeColor(a, 1f, false, true);
        panel.gameObject.GetComponent<CanvasGroup>().alpha = 1 - percentage * .01f;
        */



        //panel.gameObject.SetActive(false);



        //Player.instance.playerCameraGb.SetActive(true);
        // NOW WE can build the Navmesh haha

        //NavMeshScript.instance.updateNavMesh();
        //NPCManager.instance.OnMapGenerationDone();
    }




}
