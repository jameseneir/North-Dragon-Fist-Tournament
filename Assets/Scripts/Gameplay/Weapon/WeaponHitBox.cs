using UnityEngine;

public class WeaponHitBox : HitBox
{
    [SerializeField]
    int playerLayer = 6;
    [SerializeField]
    int enemyLayer = 9;
    Weapon weapon;
    public void SetUp(int dmg, Weapon wp)
    {
        damage = dmg;
        weapon = wp;
        if(gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
        }
    }

    public void Equip(int layer)
    {
        if(layer == playerLayer)
        {
            targetLayer = enemyLayer;
        }
        else if(layer == enemyLayer)
        {
            targetLayer = playerLayer;
        }
    }

    protected override void Damage(Health health)
    {
        base.Damage(health);
        weapon.HitSomething();
        if (weapon.isThrowing)
        {
            weapon.DisableHitBox();
        }
    }
}
