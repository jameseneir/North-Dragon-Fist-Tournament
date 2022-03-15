using UnityEngine;

public class PlayParticleFX : MonoBehaviour
{
    [SerializeField]
    ParticleSystem[] particles;
    [SerializeField]
    float duration;
    private void OnEnable()
    {
        foreach(ParticleSystem particle in particles)
        {
            particle.Play();
        }
        Invoke(nameof(Disable), duration);
    }

    void Disable()
    {
        if(gameObject.activeInHierarchy)
            gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        foreach (ParticleSystem particle in particles)
        {
            particle.Stop();
        }
    }
}
