using UnityEngine;

public class AttackState : StateMachineBehaviour
{
    CharacterComponent character;
    [SerializeField]
    private int attackIndex;
    readonly int atkHash = Animator.StringToHash("Attack");

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        character = animator.GetComponent<CharacterComponent>();
        character.EnterAttackState(attackIndex);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        character.ExitAttackState();
        if(attackIndex == 3)
        {
            animator.ResetTrigger(atkHash);
        }
    }
}
