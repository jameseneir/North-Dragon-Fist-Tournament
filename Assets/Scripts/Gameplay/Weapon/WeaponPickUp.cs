using UnityEngine;

public class WeaponPickUp : MonoBehaviour
{
    [SerializeField]
    Weapon weapon;
    [SerializeField]
    bool enemyCanPickUp;

    [SerializeField]
    int playerLayer = 6;
    [SerializeField]
    int enemyLayer = 9;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == playerLayer)
        {
            other.GetComponent<PlayerBase>().WeaponTriggerEnter(weapon);
        }
        else if(enemyCanPickUp)
        {
            if (other.gameObject.layer == enemyLayer)
                other.GetComponent<EnemyBase>().WeaponTriggerEnter(weapon);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == playerLayer)
        {
            other.GetComponent<PlayerBase>().WeaponTriggerExit(weapon);
        }
        else if (enemyCanPickUp)
        {
            if(other.gameObject.layer == enemyLayer)
            {
                other.GetComponent<EnemyBase>().WeaponTriggerExit(weapon);
            }
        }
    }
}
