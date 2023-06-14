using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Linq;

public class Test : EditorWindow
{
    VisualElement container;

    [MenuItem("Testing/Test Window")]
    public static void ShowWindow()
    {
        Test window = GetWindow<Test>();
        window.titleContent = new GUIContent("Test Window");
        window.minSize = new Vector2(500, 500);
    }

    public void CreateGUI()
    {
        container = rootVisualElement;
        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Sample.uxml");
        container.Add(visualTree.Instantiate());

        StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/test.uss");
        container.styleSheets.Add(styleSheet);

        m_submitButton = container.Query<Button>("submitButton");

        m_submitButton.clicked += SubmitButtonClicked;
    }
    

    public void SubmitButtonClicked()
    {
        //Test2 window = GetWindow<Test2>();
        //window.titleContent = new GUIContent("Test 2nd Window");
        //window.minSize = new Vector2(300, 200);
        //window.maxSize = new Vector2(300, 200);
        //window.OnSubmitClicked = OnSubmitClicked;

        var newContainer = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/progressContainer.uxml");
        container.Query<VisualElement>("outerContainer").First().Add(newContainer.Instantiate());
    }

    public void OnDestroy()
    {
        m_submitButton.clicked -= SubmitButtonClicked;
    }

    private Button m_submitButton;

    private void OnSubmitClicked(string taskText)
    {

    }
}
