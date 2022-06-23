using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingAttack : MonoBehaviour
{
    [SerializeField]
    Rigidbody projectilePrefab;

    public void Throw(Transform spawnPoint, Vector3 destination, float height)
    {
        Rigidbody instance = (Rigidbody)Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);
        //set velocity
    }
}
