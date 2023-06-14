using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MenuData/ListData", fileName = "ListData")]
public class ListData : ScriptableObject
{
    public string Label;
    public List<TaskData> Tasks = new List<TaskData>();
}
