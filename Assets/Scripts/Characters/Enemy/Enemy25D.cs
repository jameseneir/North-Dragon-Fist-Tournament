using UnityEngine;

public class Enemy25D : EnemyBase 
{
    bool facingRight = true;
    readonly int velX = Animator.StringToHash("Velocity X");
    readonly int velZ = Animator.StringToHash("Velocity Z");
    float speedX, speedZ;

    protected override void MoveToDestination(Vector3 destination)
    {
        Vector3 toDestination = destination - transform.position;
        if(toDestination.sqrMagnitude > 5)
        {
            speedX = toDestination.normalized.x;
            speedZ = toDestination.normalized.z;
            anim.SetFloat(velX, speedX, animationBlendDamp, Time.deltaTime);
            anim.SetFloat(velZ, speedZ, animationBlendDamp, Time.deltaTime);
            if (!move)
            {
                move = true;
                anim.SetBool(moveHash, true);
            }
        }
        else
        {
            //arrive
        }
        
    }

    protected override void MoveCloser()
    {
        if(facingRight)
        {
            Vector2 toPlayerNormalized = new Vector2(toPlayer.x, toPlayer.z - 5).normalized;
            speedZ = Mathf.Abs(toPlayerNormalized.y);
            speedX = toPlayerNormalized.x;
        }
        else
        {
            Vector2 toPlayerNormalized = new Vector2(toPlayer.x, toPlayer.z + 5).normalized;
            speedZ = Mathf.Abs(toPlayerNormalized.y);
            speedX = -toPlayerNormalized.x;
        }
        anim.SetFloat(velX, speedX, animationBlendDamp, Time.deltaTime);
        anim.SetFloat(velZ, speedZ, animationBlendDamp, Time.deltaTime);
        if (!move)
        {
            move = true;
            anim.SetBool(moveHash, true);
        }
    }

    protected override void BackAway()
    {
        Vector2 toPlayerNormalized = new Vector2(toPlayer.x, toPlayer.z).normalized;
        speedZ = Mathf.Abs(toPlayerNormalized.y);
        speedX = facingRight ? toPlayerNormalized.x : -toPlayerNormalized.x;
        anim.SetFloat(velX, speedX, animationBlendDamp, Time.deltaTime);
        anim.SetFloat(velZ, -speedZ, animationBlendDamp, Time.deltaTime);
        if (!move)
        {
            move = true;
            anim.SetBool(moveHash, true);
        }
    }

    protected override void LookAtPlayer()
    {
        if ((toPlayer.z > 0 && !facingRight) || (toPlayer.z < 0 && facingRight))
        {
            Flip();
        }
    }

    bool cannotFlip;

    void Flip()
    {
        if(!cannotFlip)
        {
            facingRight = !facingRight;
            transform.rotation = Quaternion.LookRotation(transform.forward * -1, transform.up);
            cannotFlip = true;
            Invoke(nameof(ResetFlip), 1);
        }
    }

    void ResetFlip()
    {
        cannotFlip = false;
    }
}
