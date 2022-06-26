using UnityEngine;

public class ThrowingAttack : MonoBehaviour
{
    [SerializeField]
    Rigidbody projectilePrefab;

    [SerializeField]
    float throwHeight;

    float velocityY;
    float timeTillLanding;

    private void Start()
    {
        velocityY = Mathf.Sqrt(-2 * Physics.gravity.y * throwHeight);
        timeTillLanding = Mathf.Sqrt(-2 * throwHeight / Physics.gravity.y);
    }

    public void Throw(Transform spawnPoint, Vector3 direction)
    {
        Rigidbody instance = Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);
        //set velocity
        instance.velocity = new Vector3(direction.x / timeTillLanding, velocityY, direction.z / timeTillLanding);
    }
}
