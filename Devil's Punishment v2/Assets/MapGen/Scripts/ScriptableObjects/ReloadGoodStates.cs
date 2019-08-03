using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ReloadGoodStates", menuName = "ScriptableObjects/ReloadGoodStates", order = 2)]
public class ReloadGoodStates : ScriptableObject
{
    public int i = 0;
    public bool isReloadingGoodStates = true;
}
