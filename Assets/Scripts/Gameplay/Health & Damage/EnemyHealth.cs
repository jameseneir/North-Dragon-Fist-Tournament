using UnityEngine;

public class EnemyHealth : Health
{
    EnemyStats stats;

    protected override void SetUp()
    {
        stats = GetComponent<EnemyBase>().enemyStats;
        MaxHP = stats.health;
        HP = MaxHP;
        if(HPSlider != null && HPSlider.maxValue != MaxHP)
        {
            HPSlider.maxValue = MaxHP;
            HPSlider.minValue = 0;
            HPSlider.value = MaxHP;
        }
    }

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        HPSlider.maxValue = MaxHP;
    }

    public override void Recover(int amount, bool fullRecovery)
    {
        base.Recover(amount, fullRecovery);
        HPSlider.maxValue = MaxHP;
    }
}
