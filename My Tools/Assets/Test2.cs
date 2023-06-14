using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.Events;

public class Test2 : EditorWindow
{
    public UnityAction OnCancelClicked;

    public UnityAction<string> OnSubmitClicked;

    public void CreateGUI()
    {

    }
}
