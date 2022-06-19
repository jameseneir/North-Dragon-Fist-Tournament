using System.Collections.Generic;
using UnityEngine;
using CI.QuickSave;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UnlockedList : MonoBehaviour
{
    List<int> unlockedSkills;
    [SerializeField]
    GameObject[] buttonsRow;

    List<AttackData> selected;

    [SerializeField]
    Image[] abilitiesIcon;

    [SerializeField]
    Sprite placeHolderIcon;
 
    public static UnlockedList instance;

    [SerializeField]
    int nextSceneIndex;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        var reader = QuickSaveReader.Create("Lee's Memory", new QuickSaveSettings()
        {
            SecurityMode = SecurityMode.Aes,
            Password = "Senify/NDFT2021",
            CompressionMode = CompressionMode.Gzip
        });

        unlockedSkills = reader.Read<List<int>>("Unlocked Skills");

        selected = new List<AttackData>();

        int i = 0;
        foreach(GameObject button in buttonsRow)
        {
            if(unlockedSkills.Contains(i))
            {
                button.SetActive(true);
            }
            else
            {
                button.SetActive(false);
            }
            i++;
        }
    }

    public void AddAbility(AttackData data)
    {
        if(!selected.Contains(data))
        {
            if(selected.Count < abilitiesIcon.Length)
            {
                selected.Add(data);
                abilitiesIcon[selected.Count - 1].sprite = data.abilityIcon;
            }
            else
            {
                selected[0] = data;
                abilitiesIcon[0].sprite = data.abilityIcon;
            }
        }
        
    }

    public void RemoveAbility(int index)
    {
        selected.RemoveAt(index);
        abilitiesIcon[index].sprite = placeHolderIcon;
    }

    public void NextScene()
    {
        GameManager.Instance.data = selected;
        SceneManager.LoadScene(nextSceneIndex);
    }
}
