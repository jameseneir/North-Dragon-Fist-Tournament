using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int targetLayer;
    [SerializeField]
    int damage;
    [SerializeField]
    GameObject hitVFXPrefab;
    [SerializeField]
    bool selfDestroy;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == targetLayer)
        {
            Instantiate(hitVFXPrefab, collision.GetContact(0).point, Quaternion.identity);
            if(damage > 0)
            {
                collision.gameObject.GetComponent<Health>().TakeDamage(damage);
            }
            if(selfDestroy)
            {
                Destroy(gameObject);
            }
        }
    }
}
