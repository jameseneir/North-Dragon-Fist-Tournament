using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Data", menuName = "NDFT/Attack")]
public class AttackData : ScriptableObject
{
    public string animatorTriggerName;
    [HideInInspector]
    public int animatorHashesIndex;
    public bool multiTargetAttack;
    public int resourceCost;
    public AudioClip hitSFX;
}
