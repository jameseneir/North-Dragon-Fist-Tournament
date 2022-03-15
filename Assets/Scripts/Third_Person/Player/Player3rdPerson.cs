using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player3rdPerson : PlayerBase
{
    readonly int velX = Animator.StringToHash("Velocity X");
    readonly int velZ = Animator.StringToHash("Velocity Z");
    float speedX, speedZ;
    [SerializeField]
    float lookSensitivity;

    [SerializeField]
    GameObject followTarget;

    InputAction lookInput;
    readonly string lookActionName = "Look";
    Vector2 look;
    bool rotate;

    #region Set up
    protected override void SetUp()
    {
        Cursor.lockState = CursorLockMode.Locked;
        lookInput = input.actions[lookActionName];
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

        if (localEulerRotation.x > 180 && localEulerRotation.x < 360)
        {
            localEulerRotation.x = 360;
        }
        else if (localEulerRotation.x < 180 && localEulerRotation.x > 35)
        {
            localEulerRotation.x = 35;
        }
        followTarget.transform.localEulerAngles = localEulerRotation;
    }

    void MatchRotationWithView()
    {
        if (Mathf.Abs(followTarget.transform.localEulerAngles.y) > 1)
        {
            float yAngle = followTarget.transform.eulerAngles.y;
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, yAngle, 0);
            followTarget.transform.localEulerAngles = new Vector3(followTarget.transform.eulerAngles.x, 0, 0);
        }
    }
    #endregion

    #region Movement
    protected override IEnumerator Move()
    {
        while (move)
        {
            MatchRotationWithView();
            if (isJumping)
            {
                velocity = direction.x * midAirMoveSpeed * transform.right + direction.y * midAirMoveSpeed * transform.forward + Vector3.up * yVel;
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
                        velocity = direction.x * midAirMoveSpeed * transform.right + direction.y * midAirMoveSpeed * transform.forward + Vector3.up * yVel;
                        controller.Move(velocity * Time.deltaTime);
                    }
                }
                else
                {
                    speedZ = direction.y;
                    speedX = direction.x;

                    if (isSprinting)
                    {
                        if (speedZ < -0.1f)
                        {
                            speedZ -= 1;
                        }
                        else if (speedZ > 0.1f)
                        {
                            speedZ += 1;
                        }
                        if (speedX > 0.1f)
                        {
                            speedX += 1;
                        }
                        else if (speedX < -0.1f)
                        {
                            speedX -= 1;
                        }
                    }
                    anim.SetFloat(velX, speedX, animationBlendDamp, Time.deltaTime);
                    anim.SetFloat(velZ, speedZ, animationBlendDamp, Time.deltaTime);
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

    protected override void Jump()
    {
        MatchRotationWithView();
        base.Jump();
    }

    protected override void Attack(int index)
    {
        MatchRotationWithView();
        base.Attack(index);
    }

    public override void Die()
    {
        base.Die();
        Cursor.lockState = CursorLockMode.None;
    }
}
