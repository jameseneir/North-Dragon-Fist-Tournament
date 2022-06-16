using UnityEngine;

public enum PoolObjectType
{
    NONE,
    GEM,  
    HIT_IMPACT_01,
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
                
           case PoolObjectType.HIT_IMPACT_01:
                {
                    obj = Instantiate(Resources.Load("HitImpact01", typeof(GameObject)) as GameObject);
                    break;
                }
        }

        return obj.GetComponent<PoolObject>();
    }
}