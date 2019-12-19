using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Objective", menuName = "Mission maker/Objective")]
public class ObjectiveMaker : ScriptableObject
{
    public string missionName;
    public bool complete;
    public bool isSelected;
    
    [TextArea]
    public string description;
 
    ///<summary>
    ///Use 2 keys in the valueList key 0 = current value
    ///Use key 1 for max value
    ///</summary>
    public Dictionary<string, int[]> valueList = new Dictionary<string, int[]>();
}
