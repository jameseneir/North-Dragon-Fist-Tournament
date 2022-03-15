using UnityEngine;

public class CharacterSelection : MonoBehaviour
{
    [HideInInspector]
    public CharacterStats stats;
    [HideInInspector]
    public CharacterMenu menu;

    public void SelectPlayer()
    {
        menu.SelecPlayer(stats);
    }
}
