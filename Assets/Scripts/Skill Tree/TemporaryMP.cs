using UnityEngine;

public class TemporaryMP : MonoBehaviour
{
    public int currentMP;

    public void PlayerWin()
    {
        MemoryPoint.ChangeMemoryPoint(currentMP);
    }
}
