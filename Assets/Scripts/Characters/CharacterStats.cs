using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "NDFT/Character")]
public class CharacterStats : ScriptableObject
{
    public string characterName;
    public Sprite characterImage;
    public GameObject characterPrefab;
    public int maxHealth;
    public float jumpHeight;
    [Header("Audio")]
    public AudioClip walkSFX;
    public AudioClip dashSFX;
    public AudioClip jumpSFX;
    public AudioClip hurtSFX;
    public AudioClip dieSFX;
    public AudioClip[] attackSFX;
}
