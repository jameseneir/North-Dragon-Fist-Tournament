using UnityEngine;

public class PlayerHitBox : HitBox
{
    [SerializeField]
    PlayerBase player;

    [SerializeField]
    int confidenceIncreaseOnHit;

    protected override void Damage(Health health)
    {
        health.TakeDamage(damage);
        player.IncreaseConfidence(confidenceIncreaseOnHit);
    }
}
