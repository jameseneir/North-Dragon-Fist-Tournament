using UnityEngine;

public class AttackState : StateMachineBehaviour
{
    CharacterComponent character;
    readonly int atk2Hash = Animator.StringToHash("Attack 3");
    readonly int atkHash = Animator.StringToHash("Attack");

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        character = animator.GetComponent<CharacterComponent>();
        character.EnterAttackState();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        character.ExitAttackState();
        if (stateInfo.shortNameHash == atk2Hash)
        {
            animator.ResetTrigger(atkHash);
        }
    }
}
