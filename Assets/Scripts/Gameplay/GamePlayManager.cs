using UnityEngine;

public class GamePlayManager : MonoBehaviour
{
    GameManager manager;

    public Transform Target { get { return target; } }
    Transform target;

    [SerializeField]
    Transform[] charactersSpawnPoints;

    private void Awake()
    {
        manager = FindObjectOfType<GameManager>();
        SpawnPlayer(0);
    }

    void SpawnPlayer(int index)
    {
        CharacterStats stats = manager.players[index];
        GameObject character = Instantiate(stats.characterPrefab, charactersSpawnPoints[index].position, charactersSpawnPoints[index].rotation);

        target = character.transform;
    }
}
