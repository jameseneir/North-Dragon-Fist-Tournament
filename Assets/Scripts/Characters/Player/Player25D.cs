using System.Collections;
using UnityEngine;

public class Player25D : PlayerBase
{
    [SerializeField]
    float rotateSpeed;
    readonly int vel = Animator.StringToHash("Velocity");
    Vector3 destination;

    protected override IEnumerator Move()
    {
        while(move)
        {
            //look
            destination = new Vector3(-direction.y, 0, direction.x);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(destination, transform.up), rotateSpeed * Time.deltaTime);
            if (isJumping)
            {
                velocity = direction.magnitude * midAirMoveSpeed * transform.forward + Vector3.up * yVelocity;
                controller.Move(velocity * Time.deltaTime);
            }
            else
            {
                if(!controller.isGrounded)
                {
                    if(!falling)
                    {
                        falling = true;
                        anim.SetBool(fallingBoolHash, true);
                        anim.SetBool(groundedBoolHash, false);
                        StartCoroutine(Falling());
                    }
                    else
                    {
                        velocity = direction.magnitude * midAirMoveSpeed * transform.forward + Vector3.up * yVelocity;
                        controller.Move(velocity * Time.deltaTime);
                    }
                }
                else
                {
                    anim.SetFloat(vel, direction.magnitude, animationBlendDamp, Time.deltaTime);
                }
            }
            yield return null;
        }
    }

    protected override void StartMoving()
    {
        move = true;
        anim.SetBool(moveHash, true);
        StartCoroutine(Move());
    }

    protected override void StopMoving()
    {
        move = false;
        direction = Vector2.zero;
        StopCoroutine(Move());
        anim.SetBool(moveHash, false);
    }
}
