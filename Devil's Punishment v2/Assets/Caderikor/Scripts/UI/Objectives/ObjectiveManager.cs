using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveManager : MonoBehaviour
{
    public List<ObjectiveMaker> objectives = new List<ObjectiveMaker>();
    public List<GameObject> missionObjectList = new List<GameObject>();
    public GameObject missionUIPrefab;
    public Transform missionBoxListBox;

    public GameObject ObjectiveGameObject;
    public GameObject missionGameObject;

    public bool inMission = false;
    private int missionIndex = 0;

    private void Awake()
    {
        missionGameObject.SetActive(false);

        for (int i = 0; i < objectives.Count; i++)
        {
                GameObject missionPrefab = Instantiate(missionUIPrefab, missionBoxListBox);
                missionPrefab.GetComponent<Text>().text = objectives[i].missionName;
                missionObjectList.Add(missionPrefab);
                missionObjectList[i].GetComponent<MissionBox>().mission = Instantiate(objectives[i]);
        }

        string oldName = missionObjectList[0].GetComponent<Text>().text;
        string newString = ">" + oldName + "<";
        missionObjectList[0].GetComponent<Text>().text = newString;
        missionObjectList[0].GetComponent<MissionBox>().mission.isSelected = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            MoveUp();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            MoveDown();
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ChooseMission();
        }
    }

    public void UpdateList(int index)
    {
        for (int i = 0; i < missionObjectList.Count; i++)
        {
            if (missionObjectList[i].GetComponent<MissionBox>().mission.complete)
            {
                missionObjectList[i].GetComponentInChildren<Image>().color = Color.green;
            }
        }
    }

    public void UpdateList(ObjectiveMaker _Mission)
    {
        GameObject missionPrefab = Instantiate(missionUIPrefab, missionBoxListBox);
        missionObjectList.Add(missionUIPrefab);
        missionPrefab.GetComponent<Text>().text = _Mission.name;
    }

    public void MoveUp()
    {
        for (int i = 0; i < missionObjectList.Count; i++)
        {
            if (missionObjectList[i].GetComponent<MissionBox>().mission.isSelected && i-1 > -1)
            {
                missionObjectList[i].GetComponent<Text>().text = missionObjectList[i].GetComponent<MissionBox>().mission.missionName;
                string oldName = missionObjectList[i-1].GetComponent<Text>().text;
                string newString = ">" + oldName + "<";
                missionObjectList[i-1].GetComponent<Text>().text = newString;
                missionObjectList[i].GetComponent<MissionBox>().mission.isSelected = false;
                missionObjectList[i-1].GetComponent<MissionBox>().mission.isSelected = true;
                missionIndex = i-1;
                return;
            }
        }
    }

    public void MoveDown()
    {
        for (int i = 0; i < missionObjectList.Count; i++)
        {
            if (missionObjectList[i].GetComponent<MissionBox>().mission.isSelected && i+1 < missionObjectList.Count)
            {
                missionObjectList[i].GetComponent<Text>().text = missionObjectList[i].GetComponent<MissionBox>().mission.missionName;
                string oldName = missionObjectList[i+1].GetComponent<Text>().text;
                string newString = ">" + oldName + "<";
                missionObjectList[i+1].GetComponent<Text>().text = newString;
                missionObjectList[i].GetComponent<MissionBox>().mission.isSelected = false;
                missionObjectList[i+1].GetComponent<MissionBox>().mission.isSelected = true;
                missionIndex = i+1;
                return;
            }
        }
    }

    public void ChooseMission()
    {
        inMission = !inMission;

        if (inMission)
        {
            ObjectiveGameObject.SetActive(false);
            missionGameObject.SetActive(true);

            var componentsInMission = missionGameObject.GetComponentsInChildren<Text>();
            foreach (Text obj in componentsInMission)
            {
                if (obj.name == "Mission name Text") { obj.text = missionObjectList[missionIndex].GetComponent<MissionBox>().mission.missionName; }
                if (obj.name == "MissionList box") { obj.text = missionObjectList[missionIndex].GetComponent<MissionBox>().mission.description; }
            }
        }
        else 
        {
            ObjectiveGameObject.SetActive(true);
            missionGameObject.SetActive(false);
        }
    }

}
