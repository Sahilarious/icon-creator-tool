using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class UIColorField : ColorField
{
    public new class UxmlFactory : UxmlFactory<UIColorField, ColorField.UxmlTraits> { }

    public UIColorField()
    {
    }
}
