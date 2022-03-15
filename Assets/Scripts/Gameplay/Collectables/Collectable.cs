using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CollectableType
{
    NONE,
    GEM,
}

public class Collectable : MonoBehaviour
{
    public CollectableType collectableType;

    PoolObject PoolObject;

    private void Awake()
    {
        PoolObject = GetComponent<PoolObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerBase player = other.gameObject.GetComponent<PlayerBase>();
        if (player != null)
        {
            player.GetItem(collectableType);
            PoolObject.TurnOff();
        }
    }
}
