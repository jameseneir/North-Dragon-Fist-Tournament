using UnityEngine;

public class EnemyThirdPerson : EnemyBase
{
    readonly int velocityHash = Animator.StringToHash("Velocity");
    protected override void BackAway()
    {
        anim.SetFloat(velocityHash, -1f, animationBlendDamp, Time.deltaTime);
        if (!move)
        {
            move = true;
            anim.SetBool(moveHash, true);
        }
    }

    protected override void MoveCloser()
    {
        anim.SetFloat(velocityHash, 1f, animationBlendDamp, Time.deltaTime);
        if(!move)
        {
            move = true;
            anim.SetBool(moveHash, true);
        }
    }

    protected override void LookAtPlayer()
    {
        transform.rotation = Quaternion.LookRotation(toPlayer, transform.up);
    }
}
