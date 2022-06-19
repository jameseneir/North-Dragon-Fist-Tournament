using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player3rdPerson : PlayerBase
{
    [SerializeField]
    float lookSensitivity;

    [SerializeField]
    GameObject followTarget;

    [SerializeField]
    GameObject viewTransform;

    [SerializeField]
    InputAction lookInput;
    
    [SerializeField]
    float rotateSpeed;

    Vector2 look;
    bool rotate;

    readonly int vel = Animator.StringToHash("Velocity");

    #region Set up
    protected override void SetUpForChildClasses()
    {
        Cursor.lockState = CursorLockMode.Locked;
        lookInput.started += ctx =>
        {
            if (cannotMove)
                return;
            rotate = true;
            look = ctx.ReadValue<Vector2>();
            StartCoroutine(Looking());

        };
        lookInput.performed += ctx =>
        {
            if (cannotMove)
                return;
            look = ctx.ReadValue<Vector2>();
            if (!rotate)
            {
                rotate = true;
                StartCoroutine(Looking());
            }
        };

        lookInput.canceled += ctx =>
        {
            if (rotate)
            {
                rotate = false;
                StopCoroutine(Looking());
            }
        };
    }
    #endregion

    #region Look
    IEnumerator Looking()
    {
        while (rotate)
        {
            Look();
            yield return null;
        }
    }

    void Look()
    {
        followTarget.transform.rotation *= Quaternion.AngleAxis(look.x * lookSensitivity, Vector3.up);
        followTarget.transform.rotation *= Quaternion.AngleAxis(look.y * lookSensitivity, Vector3.right);

        Vector3 localEulerRotation = followTarget.transform.localEulerAngles;
        localEulerRotation.z = 0;

        if (localEulerRotation.x > 180 && localEulerRotation.x < 340)
        {
            localEulerRotation.x = 340;
        }
        else if (localEulerRotation.x < 180 && localEulerRotation.x > 35)
        {
            localEulerRotation.x = 35;
        }
        followTarget.transform.localEulerAngles = localEulerRotation;
        viewTransform.transform.localEulerAngles = new Vector3(0, followTarget.transform.localEulerAngles.y, 0);
    }
    #endregion

    #region Movement

    protected override IEnumerator Move()
    {
        while (move)
        {
            Quaternion originalRotation = followTarget.transform.rotation;
            Vector3 forward = direction.x * viewTransform.transform.right + direction.y * viewTransform.transform.forward;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(forward, transform.up), Time.deltaTime * rotateSpeed);
            followTarget.transform.rotation = originalRotation;
            viewTransform.transform.localEulerAngles = new Vector3(0, followTarget.transform.localEulerAngles.y, 0);
            if (isJumping)
            {
                velocity = midAirMoveSpeed * forward + Vector3.up * yVelocity;
                controller.Move(velocity * Time.deltaTime);
            }
            else
            {
                if (!controller.isGrounded)
                {
                    if (!falling)
                    {
                        falling = true;
                        anim.SetBool(fallingBoolHash, true);
                        anim.SetBool(groundedBoolHash, false);
                        StartCoroutine(Falling());
                    }
                    else
                    {
                        velocity = midAirMoveSpeed * forward + Vector3.up * yVelocity;
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
        anim.SetBool(moveHash, false);
        StopCoroutine(Move());
    }
    #endregion

    public override void Die()
    {
        base.Die();
        Cursor.lockState = CursorLockMode.None;
    }
}
