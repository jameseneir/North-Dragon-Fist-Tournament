using UnityEngine;

public enum PoolObjectType
{
    NONE,
    GEM,  
}

public class PoolObjectLoader : MonoBehaviour
{
    public static PoolObject InstantiatePrefab(PoolObjectType objType)
    {
        GameObject obj = null;

        switch (objType)
        {
            case PoolObjectType.GEM:
                {
                    obj = Instantiate(Resources.Load("Gem", typeof(GameObject)) as GameObject);
                    break;
                }
        }

        return obj.GetComponent<PoolObject>();
    }
}