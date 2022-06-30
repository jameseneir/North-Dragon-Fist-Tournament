using UnityEngine;

public class CharacterComponent : MonoBehaviour
{
    public CharacterStats stats;
    [HideInInspector]
    public bool isHurt;
    [HideInInspector]
    public bool isDead;

    public bool weaponDetected;
    public bool isUsingWeapon;
    protected Weapon currentWeapon;
    protected Weapon detectedWeapon;
    

    public Transform weaponPivot;

    protected bool cannotAttack;
    protected bool switchHurtAnimation;

    #region animatorHashes
    protected readonly int moveHash = Animator.StringToHash("Moving");
    protected readonly int jumpTriggerName = Animator.StringToHash("Jump");
    protected readonly int hurtTriggerName = Animator.StringToHash("Hurt");
    protected readonly int hurt2TriggerName = Animator.StringToHash("Hurt 2");
    protected readonly int dieTriggerName = Animator.StringToHash("Die");
    protected readonly int guardingBoolName = Animator.StringToHash("Guarding");
    protected readonly int pickUpWeaponTriggerName = Animator.StringToHash("PickUp");
    protected readonly int throwTriggerName = Animator.StringToHash("Throw");
    #endregion

    public virtual void Hurt()
    {
        
    }

    protected virtual void StartGuarding()
    {

    }

    protected virtual void StopGuarding()
    {

    }

    public virtual void ExitHurtState()
    {

    }

    public virtual void EnterAttackState(int index)
    {

    }

    public virtual void ExitAttackState()
    {

    }

    public virtual void EnableHitbox(int index)
    {

    }

    public virtual void DisableHitbox(int index)
    {

    }

    public virtual void EnableAttackVFX(int index)
    {

    }

    public virtual void DisableAttackVFX(int index)
    {

    }

    public virtual void Die()
    {

    }

    public virtual void WeaponTriggerEnter(Weapon weapon)
    {

    }

    public virtual void WeaponTriggerExit(Weapon weapon)
    {

    }

    protected virtual void EquipWeapon()
    {

    }

    protected virtual void ThrowWeapon()
    {

    }

    protected virtual void DropWeapon()
    {

    }

    public virtual void WeaponDisappear()
    {

    }
}
