using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    EnemyWave[] waves;

    [SerializeField]
    GameObject YouWinPanel;

    int currentWave;

    public void EndWave()
    {
        if(currentWave == waves.Length - 1)
        {
            //game over
            YouWinPanel.SetActive(true);
        }
        else
        {
            currentWave++;
            waves[currentWave].gameObject.SetActive(true);
        }
    }
}
