using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField]
    int HP;
    [SerializeField]
    bool fullRecovery;

    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<Health>().Recover(HP, fullRecovery);
        Destroy(gameObject);
    }
}
