using System.Collections.Generic;
using UnityEngine;

public class PoolingManager : Singleton<PoolingManager>
{
    public Dictionary<PoolObjectType, List<GameObject>> PoolDictionary = new Dictionary<PoolObjectType, List<GameObject>>();
    public GameObject PooledObjectsParent;

    public override void Awake()
    {
        base.Awake();
    }


    public void SetUpDictionary()
    {
        PoolDictionary.Clear();

        PoolObjectType[] arr = System.Enum.GetValues(typeof(PoolObjectType)) as PoolObjectType[];

        foreach (PoolObjectType p in arr)
        {
            if (!PoolDictionary.ContainsKey(p))
            {
                PoolDictionary.Add(p, new List<GameObject>());
            }
        }

        SetupPoolObjects();
    }

    private void Start()
    {
        if (PoolDictionary.Count == 0)
        {
            SetUpDictionary();
        }
    }

    public void SetupPoolObjects()
    {
        foreach (KeyValuePair<PoolObjectType, List<GameObject>> pool in PoolDictionary)
        {
            if (pool.Key == PoolObjectType.NONE)
                continue;

            for (int i = 0; i < 3; i++)
            {
                GameObject obj = PoolObjectLoader.InstantiatePrefab(pool.Key).gameObject;
                obj.transform.parent = PooledObjectsParent.transform;
                pool.Value.Add(obj);
                obj.SetActive(false);
            }
        }
    }

    public GameObject GetObject(PoolObjectType objType)
    {
        if (PoolDictionary.Count == 0)
        {
            SetUpDictionary();
        }


        List<GameObject> list = PoolDictionary[objType];
        GameObject obj = null;

        if (list.Count > 0)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] != null)
                {
                    obj = list[i];
                    list.RemoveAt(i);
                    break;
                }
                if (i == list.Count - 1)
                {
                    obj = PoolObjectLoader.InstantiatePrefab(objType).gameObject;
                }
            }
        }
        else
        {
            obj = PoolObjectLoader.InstantiatePrefab(objType).gameObject;
        }

        return obj;
    }

    public void AddObject(PoolObject obj)
    {
        List<GameObject> list = PoolDictionary[obj.poolObjectType];
        list.Add(obj.gameObject);
        obj.gameObject.SetActive(false);
    }

    public void SpawnObj(PoolObjectType ObjectType, Vector3 position, Transform parent)
    {
        GameObject obj = GetObject(ObjectType);

        if (parent != null)
        {
            obj.transform.parent = parent;
            obj.transform.localPosition = Vector3.zero + position;
            obj.transform.localRotation = Quaternion.identity;
        }
        else
        {
            obj.transform.position = position;
            obj.transform.rotation = Quaternion.identity;
        }

        obj.SetActive(true);
    }

    public void SpawnObj(PoolObjectType ObjectType, Vector3 position, Quaternion rotation, Transform parent)
    {
        GameObject obj = GetObject(ObjectType);

        if (parent != null)
        {
            obj.transform.parent = parent;
            obj.transform.localPosition = Vector3.zero + position;
            obj.transform.localRotation = rotation;
        }
        else
        {
            obj.transform.position = position;
            obj.transform.rotation = rotation;
        }

        obj.SetActive(true);
    }
}
