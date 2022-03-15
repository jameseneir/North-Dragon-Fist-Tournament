using UnityEngine;
using UnityEngine.VFX;

public class PlayVFX : MonoBehaviour
{
    [SerializeField]
    VisualEffect[] vfx;
    [SerializeField]
    float duration;

    private void OnEnable()
    {
        foreach(VisualEffect v in vfx)
        {
            v.Play();
        }
        Invoke((nameof(Disable)), duration);
    }

    void Disable()
    {
        if(gameObject.activeInHierarchy)
            gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        foreach (VisualEffect v in vfx)
        {
            v.Stop();
        }
    }
}
