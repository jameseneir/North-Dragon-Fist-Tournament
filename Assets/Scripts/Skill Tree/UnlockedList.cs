using System.Collections.Generic;
using UnityEngine;
using CI.QuickSave;

public class UnlockedList : MonoBehaviour
{
    List<int> unlockedSkills;
    public GameObject[] buttonsRow;

    private void Start()
    {
        var reader = QuickSaveReader.Create("Lee's Memory", new QuickSaveSettings()
        {
            SecurityMode = SecurityMode.Aes,
            Password = "Senify/NDFT2021",
            CompressionMode = CompressionMode.Gzip
        });

        unlockedSkills = reader.Read<List<int>>("Unlocked Skills");

        int i = 0;
        foreach(GameObject obj in buttonsRow)
        {
            if(unlockedSkills.Contains(i))
            {
                obj.SetActive(true);
            }
            else
            {
                obj.SetActive(false);
            }
            i++;
        }
    }
}
