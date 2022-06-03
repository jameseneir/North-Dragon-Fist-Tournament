using UnityEngine;

public enum CollectableType
{
    NONE,
    GEM,
}

public class Collectable : MonoBehaviour
{
    [SerializeField]
    LayerMask playerLayer;

    public CollectableType collectableType;

    PoolObject PoolObject;

    private void Awake()
    {
        PoolObject = GetComponent<PoolObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == playerLayer)
        {
            GamePlayManager.currentMP += 1;
            PoolObject.TurnOff();
        }
    }
}
