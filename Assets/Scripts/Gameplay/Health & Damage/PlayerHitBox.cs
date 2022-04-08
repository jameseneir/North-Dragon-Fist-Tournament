using UnityEngine;

public class PlayerHitBox : HitBox
{
    [SerializeField]
    PlayerBase player;

    protected override void Damage(Health health)
    {
        health.TakeDamage(damage);
        player.IncreaseConfidence(1);
    }
}
