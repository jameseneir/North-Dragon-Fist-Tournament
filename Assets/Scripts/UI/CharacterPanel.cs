using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class CharacterPanel : MonoBehaviour
{
    public CharacterStats stats;

    Image characterImage;
    TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        characterImage = GetComponentInChildren<Image>();
        text = GetComponentInChildren<TextMeshProUGUI>();

        characterImage.sprite = stats.characterImage;
        text.text = stats.characterName;
    }
}
