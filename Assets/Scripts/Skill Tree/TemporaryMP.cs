using UnityEngine;

public class TemporaryMP : MonoBehaviour
{
    public static int currentMP;

    public void PlayerWin()
    {
        MemoryPoint.AddMemoryPoint(currentMP);
    }
}
