using UnityEngine;

public class HurtState : StateMachineBehaviour
{
    CharacterComponent character;
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        character = animator.GetComponent<CharacterComponent>();
        if(!character.isDead)
            character.ExitHurtState();
    }
}
