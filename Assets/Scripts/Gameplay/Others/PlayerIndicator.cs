using UnityEngine;

public class PlayerIndicator : MonoBehaviour
{
    [SerializeField]
    Transform player;

    [SerializeField]
    float offset;

    private void LateUpdate()
    {
        transform.position = player.position + offset * Vector3.up;
    }
}
