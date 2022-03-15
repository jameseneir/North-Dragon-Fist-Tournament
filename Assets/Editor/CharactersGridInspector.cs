using UnityEditor;
using UnityEngine.UIElements;

[CustomEditor(typeof(CharacterMenu))]
public class CharactersGridInspector : Editor
{
    CharacterMenu menu;

    VisualElement root;

    IMGUIContainer iMGUI;

    Button button;

    private void OnEnable()
    {
        menu = (CharacterMenu)target;
    }

    public override VisualElement CreateInspectorGUI()
    {
        root = new VisualElement();
        iMGUI = new IMGUIContainer
        {
            onGUIHandler = base.OnInspectorGUI
        };
        root.Add(iMGUI);

        button = new Button()
        {
            text = "Create character panels"
        };
        button.clicked += () =>
        {
            menu.CreateUI();
        };
        root.Add(button);

        return root;
    }
}
