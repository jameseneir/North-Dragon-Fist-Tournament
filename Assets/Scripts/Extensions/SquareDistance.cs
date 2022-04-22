using UnityEngine;

public static class SquareDistance 
{
    public static float SqurDistance(this Vector3 v, Vector3 destination)
    {
        return (v - destination).sqrMagnitude;
    }
}
