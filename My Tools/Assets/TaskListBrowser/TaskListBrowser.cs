using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.VersionControl;
using System.Linq;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;

public struct TaskRemovedData
{
    public VisualElement Object;
    public VisualElement Parent;
}

public struct TaskCreatedData
{
    public VisualElement TaskContainer;
    public ListData ListData;
}

public class TaskListBrowser : EditorWindow
{
    [MenuItem("Window/TaskListBrowser")]
    public static void ShowExample()
    {
        TaskListBrowser window = GetWindow<TaskListBrowser>();
        window.titleContent = new GUIContent("TaskListBrowser");
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/TaskListBrowser/TaskListBrowser.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);

        StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/TaskListBrowser/TaskListBrowser.uss");
        root.styleSheets.Add(styleSheet);

        m_addListButton = root.Query<Button>("addListButton");
        m_addListButton.clicked += OnNewListCreated;

        Button loadBrowserButton = root.Query<Button>("loadBrowserButton");
        loadBrowserButton.clicked += OnLoadBrowserSelected;

        Button saveBrowserButton = root.Query<Button>("saveBrowserButton");
        saveBrowserButton.clicked += OnSaveBrowserSelected;

        m_listContainersParent = root.Query<VisualElement>("listContainersParent");

        //m_browserSO = new SerializedObject(m_currentTaskListBrowserData);
    }

    private Dictionary<VisualElement, TaskData> m_taskDataDict = new Dictionary<VisualElement, TaskData>();

    private Dictionary<TaskData, ListData> m_listDataDict = new Dictionary<TaskData, ListData>();

    private Button m_addListButton;

    private VisualElement m_listContainersParent;

    private List<TaskListBrowserData> m_taskListBrowserData;

    private TaskListBrowserData m_currentTaskListBrowserData;

    //private SerializedObject m_browserSO;

    private void OnNewListCreated()
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/TaskListBrowser/ListContainer.uxml");

        var listContainer = visualTree.Instantiate();
        Button addtaskButton = listContainer.Query<Button>("addTaskButton");

        VisualElement taskContainer = listContainer.Query<VisualElement>("taskContainer");

        ListData newList = ScriptableObject.CreateInstance<ListData>();


        TaskCreatedData taskCreatedData = new TaskCreatedData();
        taskCreatedData.ListData = newList;
        taskCreatedData.TaskContainer = taskContainer;

        addtaskButton.RegisterCallback<ClickEvent, TaskCreatedData>(OnTaskAdded, taskCreatedData);
        m_listContainersParent.Add(listContainer);

        string path = $"Assets/TaskListBrowser/Data/{newList.GetType().ToString()}-{Guid.NewGuid()}.asset";

        AssetDatabase.CreateAsset(newList, path);

        m_currentTaskListBrowserData.ListData.Add(newList);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();



        //m_browserSO.ApplyModifiedProperties();

        //m_currentTaskListBrowserData
    }

    private TaskCreatedData m_currentTaskData;

    private void OnTaskAdded(ClickEvent evt, TaskCreatedData taskCreatedData)
    {
        m_currentTaskData = taskCreatedData;

        EnterTaskWindow window = GetWindow<EnterTaskWindow>();
        window.titleContent = new GUIContent("Create New Task");
        window.minSize = new Vector2(300, 100);
        window.maxSize = new Vector2(300, 100);

        window.OnTaskCreated += OnSubmitTaskSelected;
    }

    private void OnSubmitTaskSelected(string taskText)
    {
        VisualTreeAsset task = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/TaskListBrowser/task.uxml");
        var taskTemplate = task.Instantiate();
        m_currentTaskData.TaskContainer.Add(taskTemplate);

        Label newtaskText = taskTemplate.Query<Label>("taskText");
        newtaskText.text = taskText;

        TaskRemovedData taskRemovedData = new TaskRemovedData();
        taskRemovedData.Object = taskTemplate;
        taskRemovedData.Parent = m_currentTaskData.TaskContainer;

        Button removeTaskButton = taskTemplate.Query<Button>("removeTaskButton");
        removeTaskButton.RegisterCallback<ClickEvent, TaskRemovedData>(OnTaskRemoved, taskRemovedData);

        TaskData newTask = ScriptableObject.CreateInstance<TaskData>();
        string path = $"Assets/TaskListBrowser/Data/{newTask.GetType().ToString()}-{Guid.NewGuid()}.asset";

        newTask.TaskText = taskText;

        AssetDatabase.CreateAsset(newTask, path);

        m_currentTaskData.ListData.Tasks.Add(newTask);

        m_taskDataDict.Add(taskTemplate, newTask);
        m_listDataDict.Add(newTask, m_currentTaskData.ListData);
    }

    private void OnTaskRemoved(ClickEvent evt, TaskRemovedData taskRemovedData)
    {
        if (m_taskDataDict.ContainsKey(taskRemovedData.Object))
        {
            TaskData taskToRemove = m_taskDataDict[taskRemovedData.Object];
            m_listDataDict[taskToRemove].Tasks.Remove(m_taskDataDict[taskRemovedData.Object]);

            AssetDatabase.DeleteAsset($"Assets/TaskListBrowser/Data/{taskToRemove.name}.asset");

            m_taskDataDict.Remove(taskRemovedData.Object);
        }

        taskRemovedData.Parent.Remove(taskRemovedData.Object);


        AssetDatabase.Refresh();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void OnLoadBrowserSelected()
    {
        var taskBrowserGuids = AssetDatabase.FindAssets("t:TaskListBrowserData", new string[] { "Assets/TaskListBrowser/Data" });
       
        List<TaskListBrowserData> taskBrowsers = new List<TaskListBrowserData>();

        foreach (var guid in taskBrowserGuids)
        {
            taskBrowsers.Add(AssetDatabase.LoadAssetAtPath<TaskListBrowserData>(AssetDatabase.GUIDToAssetPath(guid)));
        }

        m_currentTaskListBrowserData = taskBrowsers[0];

        LoadBrowser(m_currentTaskListBrowserData);
    }

    private void LoadBrowser(TaskListBrowserData taskListBrowserData)
    {
        foreach (var listData in taskListBrowserData.ListData)
        {
            if (listData != null)
            {
                LoadList(listData);
            }
        }
    }

    private void LoadList(ListData listData)
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/TaskListBrowser/ListContainer.uxml");

        var listContainer = visualTree.Instantiate();
        Button addtaskButton = listContainer.Query<Button>("addTaskButton");

        VisualElement taskContainer = listContainer.Query<VisualElement>("taskContainer");


        TaskCreatedData taskCreatedData = new TaskCreatedData();
        taskCreatedData.ListData = listData;
        taskCreatedData.TaskContainer = taskContainer;

        addtaskButton.RegisterCallback<ClickEvent, TaskCreatedData>(OnTaskAdded, taskCreatedData);
        m_listContainersParent.Add(listContainer);


        Label label = listContainer.Query<Label>("listLabel");
        label.text = listData.Label;

        foreach (var taskData in listData.Tasks)
        {
            if (m_listDataDict.ContainsKey(taskData) == false)
            {
                m_listDataDict.Add(taskData, listData);
            }
            LoadTask(taskContainer, taskData);
        }
    }

    private void LoadTask(VisualElement parentElement, TaskData taskData)
    {
        VisualTreeAsset task = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/TaskListBrowser/task.uxml");
        var taskTemplate = task.Instantiate();
        parentElement.Add(taskTemplate);

        if (m_taskDataDict.ContainsKey(taskTemplate) == false)
        {
            m_taskDataDict.Add(taskTemplate, taskData);
        }

        TaskRemovedData taskRemovedData = new TaskRemovedData();
        taskRemovedData.Object = taskTemplate;
        taskRemovedData.Parent = parentElement;

        Button removeTaskButton = taskTemplate.Query<Button>("removeTaskButton");
        removeTaskButton.RegisterCallback<ClickEvent, TaskRemovedData>(OnTaskRemoved, taskRemovedData);

        Toggle toggle = taskTemplate.Query<Toggle>();
        toggle.value = taskData.Complete;

        Label label = taskTemplate.Query<Label>("taskText");
        label.text = taskData.TaskText;
    }

    private void OnSaveBrowserSelected()
    {
        Debug.Log("Save Browser Selected");
    }


}