using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

//only works for 2 characters
public class Menu_PlayerInput : MonoBehaviour
{
    [SerializeField]
    GameObject player1SelectionPanelPrefab;
    [SerializeField]
    GameObject player2SelectionPanel;
    CharacterMenu menu;
    
    private void Awake()
    {
        menu = FindObjectOfType<CharacterMenu>();
        EventSystem eventSystem = FindObjectOfType<EventSystem>();
        RectTransform refRect = menu.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
        GameObject gridInstance = Instantiate(menu.gameObject, menu.transform.parent);
        Transform charactersGrid = gridInstance.transform;
        Destroy(charactersGrid.GetComponent<CharacterMenu>());
        Destroy(charactersGrid.GetComponent<Image>());

        Destroy(charactersGrid.GetChild(0).gameObject);
        Destroy(charactersGrid.GetChild(1).gameObject);

        GameObject player1SelectionPanel = Instantiate(player1SelectionPanelPrefab, charactersGrid);
        player1SelectionPanel.GetComponent<RectTransform>().localScale = refRect.localScale;
        CharacterSelection characterSelections0 = player1SelectionPanel.GetComponent<CharacterSelection>();
        characterSelections0.menu = menu;
        characterSelections0.stats = menu.characters[0];

        GameObject selectionInstance = Instantiate(player1SelectionPanel, charactersGrid);
        CharacterSelection characterSelections1 = selectionInstance.GetComponent<CharacterSelection>();
        characterSelections1.menu = menu;
        characterSelections1.stats = menu.characters[1];

        eventSystem.firstSelectedGameObject = player1SelectionPanel;
    }
}
