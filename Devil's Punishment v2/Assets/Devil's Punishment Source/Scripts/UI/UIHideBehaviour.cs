using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct UIList
{
    public List<GameObject> ui;
}

public class UIHideBehaviour : MonoBehaviour
{
    public List<UIList> uiToHide;
    public KeyCode[] keyCodes;

    private void Start()
    {
        if(keyCodes.Length != uiToHide.Count)
        {
            Debug.LogError("keyCodes.Length should be equal to uiToHide.Length");
            enabled = false;
        }
    }

    private void Update()
    {
        HideUIInputCheck();
    }

    private void HideUIInputCheck()
    {
        for (int i = 0; i < uiToHide.Count; i++)
        {
            if (Input.GetKeyDown(keyCodes[i]))
            {
                for (int j = 0; j < uiToHide[i].ui.Count; j++)
                {
                    uiToHide[i].ui[j].SetActive(!uiToHide[i].ui[j].activeSelf);
                }
            }
        }
    }

}
