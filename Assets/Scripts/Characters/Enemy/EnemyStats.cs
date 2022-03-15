using UnityEngine;
[CreateAssetMenu(fileName = "New Enemy", menuName = "NDFT/Enemy")]
public class EnemyStats : ScriptableObject
{
    public int health;
    public int damage;
    public float attackCooldown;
    public float attackRange;
    public float closeDistance;
    public float farDistance;
    public float dieAnimationDuration;
    public bool canUseWeapon;
    public GameObject enemyPrefab;

    [Header("Audio")]
    public AudioClip walkSFX;
    public AudioClip hurtSFX;
    public AudioClip dieSFX;
    public AudioClip[] attackSFX;

    [Header("AI")]
    public float animationBlendDamp;
    public float randomNoise;
    public float slowStateUpdateInterval;
    public float fastStateUpdateInterval;
}
