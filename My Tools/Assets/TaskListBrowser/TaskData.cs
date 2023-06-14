using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MenuData/TaskData", fileName = "TaskData")]
public class TaskData : ScriptableObject
{
    public string TaskText;
    public bool Complete;
}
