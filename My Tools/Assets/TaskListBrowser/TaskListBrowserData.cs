using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;

[CreateAssetMenu(menuName = "MenuData/TaskMenuBrowserData", fileName = "TaskMenuBrowserData")]
public class TaskListBrowserData : ScriptableObject
{
    public List<ListData> ListData = new List<ListData>();
}
