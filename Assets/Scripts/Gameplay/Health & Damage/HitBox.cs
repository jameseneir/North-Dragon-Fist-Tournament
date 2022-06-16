using UnityEngine;

public class HitBox : MonoBehaviour
{
    [SerializeField]
    GameObject hitVFX;
    [SerializeField]
    protected int targetLayer = 9;
    public int damage;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == targetLayer)
        {
            if(hitVFX != null)
            {
                PoolingManager.Instance.SpawnObj(PoolObjectType.HIT_IMPACT_01, other.ClosestPointOnBounds(transform.position), null);
                //hitVFX.SetActive(true);
                //hitVFX.transform.position = other.ClosestPointOnBounds(transform.position);
            }
            Damage(other.GetComponent<Health>());
        }
    }

    protected virtual void Damage(Health health)
    {
        health.TakeDamage(damage);
    }
}