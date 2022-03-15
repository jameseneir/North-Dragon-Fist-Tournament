using UnityEngine.SceneManagement;
using UnityEngine;

public class CharacterMenu : MonoBehaviour
{
    public CharacterStats[] characters;

    GameManager manager;

    public string nextSceneName;

    bool oneSelected;

    private void Awake()
    {
        manager = FindObjectOfType<GameManager>();
    }

    public void CreateUI()
    {
        GameObject panelPrefab = GetComponentInChildren<CharacterPanel>().gameObject;

        for(int i = 1; i < characters.Length; i++)
        {
            GameObject panelInstance = Instantiate(panelPrefab, transform);
            panelInstance.GetComponent<CharacterPanel>().stats = characters[i];
            panelInstance.GetComponent<CharacterPanel>().UpdateUI();
        }
        panelPrefab.GetComponent<CharacterPanel>().stats = characters[0];
        panelPrefab.GetComponent<CharacterPanel>().UpdateUI();
    }

    public void SelecPlayer(CharacterStats stats)
    {
        if (!oneSelected)
        {
            manager.players.Add(stats);
            oneSelected = true;
        }
            
        else
        {
            manager.players.Add(stats);
            LoadNextScene();
        }
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
