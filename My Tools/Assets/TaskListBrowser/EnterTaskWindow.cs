using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class EnterTaskWindow : EditorWindow
{
    public UnityAction<string> OnTaskCreated;

    public void CreateGUI()
    {
        m_root = rootVisualElement;

        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/TaskListBrowser/EnterTaskWindow.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        m_root.Add(labelFromUXML);

        StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/TaskListBrowser/EnterTaskWindow.uss");
        m_root.styleSheets.Add(styleSheet);


        m_submitButton = m_root.Query<Button>("submitButton");
        m_submitButton.clicked += OnSubmitButtonSelected;

        m_cancelButton = m_root.Query<Button>("cancelButton");
        m_cancelButton.clicked += OnCancelButtonSelected;

        m_textField = m_root.Query<TextField>("taskText");
    }

    private VisualElement m_root;

    private Button m_submitButton;

    private Button m_cancelButton;

    private TextField m_textField;

    private void OnSubmitButtonSelected()
    {
        OnTaskCreated?.Invoke(m_textField.text);
        this.Close();
    }

    private void OnCancelButtonSelected()
    {
        this.Close();
    }
}
