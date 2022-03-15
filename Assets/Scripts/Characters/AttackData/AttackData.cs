using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Data", menuName = "NDFT/Attack")]
public class AttackData : ScriptableObject
{
    public string animatorTriggerName;
    [HideInInspector]
    public int animatorHashesIndex;
    public int resourceCost;
    public AudioClip hitSFX;
}
