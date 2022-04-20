using UnityEngine;

public class GamePlayManager : MonoBehaviour
{
    public Transform Target { get; private set; }

    [SerializeField]
    Transform[] charactersSpawnPoints;

    [SerializeField]
    PlayerBase player;

    private void Awake()
    {
        for(int i = 0; i < 3; i++)
        {
            player.data.Add(GameManager.Instance.data[i]);
        }
        Target = player.transform;
    }
}
