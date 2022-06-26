using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabAttack : MonoBehaviour
{
    [SerializeField]
    Vector3 grabBoxHalfExtents;
    [SerializeField]
    Transform grabPoint;
    [SerializeField]
    LayerMask enemyLayer;

    public void Grab()
    {
        if(Physics.BoxCast(grabPoint.position, grabBoxHalfExtents, -grabPoint.forward,
            out RaycastHit hitInfo, grabPoint.rotation, 5, enemyLayer, QueryTriggerInteraction.Ignore))
        {
            hitInfo.transform.GetComponent<EnemyBase>().IsGrabbed(grabPoint);
        }
    }
}
