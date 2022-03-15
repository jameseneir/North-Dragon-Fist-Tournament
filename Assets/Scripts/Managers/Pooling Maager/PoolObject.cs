using System.Collections;
using UnityEngine;

public class PoolObject : MonoBehaviour
{
    public PoolObjectType poolObjectType;
    public float lifetime;
    private Coroutine turnOffRoutine;

    private void OnEnable()
    {
        if (turnOffRoutine != null)
        {
            StopCoroutine(turnOffRoutine);
        }
        if (lifetime > 0f)
        {
            turnOffRoutine = StartCoroutine(_ScheduleOff());
        }
    }

    public void TurnOff()
    {
        transform.parent = PoolingManager.Instance.PooledObjectsParent.transform;
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        PoolingManager.Instance.AddObject(this);
    }

    IEnumerator _ScheduleOff()
    {
        yield return new WaitForSeconds(lifetime);

        if (!PoolingManager.Instance.PoolDictionary[poolObjectType].Contains(this.gameObject))
        {
            TurnOff();
        }
    }
}