using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectivesHandler : MonoBehaviour
{
    Dictionary<int,GameObject> objectives;
    Dictionary<int, GameObject> objectivesLights;
    List<Transform> SortList;
    void Start()
    {
        objectives = new Dictionary<int,GameObject>();
        objectivesLights = new Dictionary<int, GameObject>();
        SortList = new List<Transform>();
        foreach (Transform child in gameObject.transform)
        {
            SortList.Add(child);
        }
        int i = 0;
        foreach(Transform child in SortList)
        {
            foreach(Transform inchild in SortList)
            {
                if (inchild.GetSiblingIndex() == i)
                {
                    objectivesLights.Add(i, inchild.GetChild(0).gameObject);
                    objectives.Add(i, inchild.gameObject);
                    break;
                }
            }
            i++;
        }
        foreach(KeyValuePair<int,GameObject> obj in objectives)
        {
            Debug.Log(obj.Key);
        }
    }

    public void FinishObjective(int obj)
    {
        if (objectivesLights.ContainsKey(obj) && ! objectivesLights[obj].activeSelf)
        {
            objectivesLights[obj].SetActive(true);
        }
    }
    public void RevealObjective(int obj)
    {
        if (objectives.ContainsKey(obj) && !objectives[obj].activeSelf)
        {
            objectives[obj].SetActive(true);
        }
    }
}
