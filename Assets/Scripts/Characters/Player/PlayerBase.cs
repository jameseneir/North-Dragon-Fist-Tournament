using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Cinemachine;

public class PlayerBase : CharacterComponent
{
    #region Input
    [Header("Input")]
    [SerializeField]
    InputActionReference moveAction;

    [SerializeField]
    InputActionReference southButtonInput;

    [SerializeField]
    InputActionReference northButtonInput;

    [SerializeField]
    InputActionReference eastButtonInput;

    [SerializeField]
    InputActionReference westButtonInput;

    [SerializeField]
    InputActionReference leftTriggerInput;

    [SerializeField]
    InputActionReference rightTriggerInput;

    [SerializeField]
    InputActionReference leftShoulderInput;

    [SerializeField]
    InputActionReference rightShoulderInput;
    #endregion

    #region Movement
    [Header("Movement")]
    protected Vector2 direction;
    protected Vector3 velocity;

    [SerializeField]
    protected CharacterController controller;
    float stepOffset;

    protected bool isJumping;
    protected bool falling;
    bool StartGroundCheck
    {
        get { return elapsedTimeSinceJumpButtonPress <= 0; }
    }
    protected float elapsedTimeSinceJumpButtonPress = 0.3f;
    protected float jumpVelocity;
    [SerializeField]
    protected float gravity = -9.81f * 5;
    protected float yVelocity;
    [SerializeField]
    protected float midAirMoveSpeed;

    protected bool cannotMove;
    protected bool move;

    bool isDashing;
    [SerializeField]
    float dashSpeed;
    [SerializeField]
    float dashDuration;
    float dashTimer;

    protected readonly int dashHash = Animator.StringToHash("Dash");
    protected readonly int jumpingBoolHash = Animator.StringToHash("IsJumping");
    protected readonly int fallingBoolHash = Animator.StringToHash("IsFalling");
    protected readonly int groundedBoolHash = Animator.StringToHash("IsGrounded");

    [SerializeField]
    protected float animationBlendDamp = 0.3f;
    #endregion

    #region Attack
    [Header("Attack")]
    public List<AttackData> data;

    [SerializeField]
    float weaponThrowForce;

    bool isAttacking;
    bool isGuarding;

    bool CanUseSpecialAttack
    {
        get { return isAttacking && confidence < data[attackCount + 1].confidenceCost && attackCount != 0; }
    }

    int currentAttackIndex;
    int attackCount;

    bool isMovingForward;
    float forwardSpeed;

    [SerializeField]
    GameObject[] attackVFX;

    [SerializeField]
    HitBox[] hitBoxes;
    bool hitboxEnabled, vfxEnabled;

    [SerializeField]
    protected Collider[] enemiesInRangeColliders;
    protected Collider closestEnemy;

    [SerializeField]
    protected LayerMask enemyLayer;

    [SerializeField]
    protected Vector3 offset;

    [SerializeField]
    protected Vector3 proximityCheckBox;

    int confidence;
    readonly int maxConfidence = 30;

    [SerializeField]
    UnityEngine.UI.Slider confidenceMeter;
    #endregion


    [Header("Others")]
    [SerializeField]
    AudioSource audioSource;

    [SerializeField]
    Health health;

    [SerializeField]
    protected Animator anim;

    [SerializeField]
    GameObject gameOverPanel;

    #region Screen Shake
    [SerializeField]
    CinemachineVirtualCamera cam;
    CinemachineBasicMultiChannelPerlin multi;

    [SerializeField]
    float screenShakeIntensity;
    [SerializeField]
    float screenShakeDuration;
    WaitForSecondsRealtime screenShakeTime;
    #endregion

    [SerializeField]
    float timeStopDuration;
    WaitForSecondsRealtime pauseTime;

    protected virtual void SetUpForChildClasses()
    {

    }

    private void Start()
    {
        jumpVelocity = Mathf.Sqrt(stats.jumpHeight * -2 * gravity);
        controller.Move(-0.5f * Vector3.up);
        stepOffset = controller.stepOffset;

        
        for (int i = 0; i < data.Count; i++)
        {
            data[i].animatorHashesIndex = Animator.StringToHash(data[i].animatorTriggerName);
        }

        #region SetUpInput
        moveAction.action.started += ctx =>
        {
            if (!cannotMove)
            {
                direction = ctx.ReadValue<Vector2>();
                if (!move)
                {
                    StartMoving();
                }
            }
        };
        moveAction.action.performed += ctx =>
        {
            if (!cannotMove)
            {
                direction = ctx.ReadValue<Vector2>();
                if (!move)
                {
                    StartMoving();
                }
            }
        };

        moveAction.action.canceled += ctx =>
        {
            if (move)
            {
                StopMoving();
            }
        };

        //jump
        southButtonInput.action.performed += ctx =>
        {
            if (controller.isGrounded && !cannotMove)
            {
                Jump();
            }
        };

        //special attack
        northButtonInput.action.performed += ctx => SpecialAttack();

        //kick
        eastButtonInput.action.performed += ctx => Attack(1);

        //light attack
        westButtonInput.action.performed += ctx => Attack(0);

        //dash
        leftTriggerInput.action.performed += ctx =>
        {
            if (controller.isGrounded && !cannotMove)
            {
                Dash();
            }
        };

        //guarding
        rightTriggerInput.action.performed += ctx =>
        {
            if (isHurt || !controller.isGrounded)
                return;
            if (!isGuarding)
                StartGuarding();
        };

        rightTriggerInput.action.canceled += ctx =>
        {
            if (isGuarding)
                StopGuarding();
        };

        //pick up weapon
        leftShoulderInput.action.performed += ctx =>
        {
            if (weaponDetected)
            {
                EquipWeapon();
            }
        };

        
        rightShoulderInput.action.performed += ctx =>
        {
            
        };
        #endregion
        SetUpForChildClasses();

        #region SetUpScreenShake
        multi = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        screenShakeTime = new WaitForSecondsRealtime(screenShakeDuration);
        pauseTime = new WaitForSecondsRealtime(timeStopDuration);
        #endregion

        if (confidenceMeter == null) return;
        confidenceMeter.value = confidence;
        confidenceMeter.maxValue = maxConfidence;
    }

    public void IncreaseConfidence(int point)
    {
        confidence += point;
        if(confidence > maxConfidence)
        {
            confidence = maxConfidence;
        }
        confidenceMeter.value = confidence;
    }

    protected virtual void StartMoving()
    {
        
    }

    protected virtual void StopMoving()
    {
        
    }

    protected virtual IEnumerator Move()
    {
        while(move)
        {
            yield return null;
        }
    }

    #region Attack
    protected virtual void Attack(int index)
    {
        if (cannotAttack)
            return;
        if (move)
            StopMoving();
        if(isUsingWeapon)
        {
            //index = currentWeapon.attackIndex;
        }
        currentAttackIndex = index;
        anim.SetTrigger(data[index].animatorHashesIndex);
        GetClosestEnemy();
        if(closestEnemy != null)
            transform.LookAt(closestEnemy.transform, transform.up);
    }

    void GetClosestEnemy()
    {
        int enemiesInRange = Physics.OverlapBoxNonAlloc(transform.position + offset, proximityCheckBox, enemiesInRangeColliders, Quaternion.identity, enemyLayer, QueryTriggerInteraction.Ignore);
        if (enemiesInRange == 0)
            return;
        switch (enemiesInRange)
        {
            case 1:
                closestEnemy = enemiesInRangeColliders[1];
                break;
            case 2:
                if (closestEnemy == null || (closestEnemy != enemiesInRangeColliders[0] && closestEnemy != enemiesInRangeColliders[1]))
                {
                    if (transform.position.SqurDistance(enemiesInRangeColliders[0].transform.position) < transform.position.SqurDistance(enemiesInRangeColliders[1].transform.position))
                    {
                        closestEnemy = enemiesInRangeColliders[0];
                    }
                    else
                    {
                        closestEnemy = enemiesInRangeColliders[1];
                    }
                }
                break;
            case 3:
                if (closestEnemy == null || (closestEnemy != enemiesInRangeColliders[0] && closestEnemy != enemiesInRangeColliders[1] && closestEnemy != enemiesInRangeColliders[2]))
                {
                    if (transform.position.SqurDistance(enemiesInRangeColliders[0].transform.position) < transform.position.SqurDistance(enemiesInRangeColliders[1].transform.position))
                    {
                        if (transform.position.SqurDistance(enemiesInRangeColliders[0].transform.position) < transform.position.SqurDistance(enemiesInRangeColliders[2].transform.position))
                        {
                            closestEnemy = enemiesInRangeColliders[0];
                        }
                        else
                        {
                            closestEnemy = enemiesInRangeColliders[2];
                        }
                    }
                    else
                    {
                        if (transform.position.SqurDistance(enemiesInRangeColliders[1].transform.position) < transform.position.SqurDistance(enemiesInRangeColliders[2].transform.position))
                        {
                            closestEnemy = enemiesInRangeColliders[1];
                        }
                        else
                        {
                            closestEnemy = enemiesInRangeColliders[2];
                        }
                    }
                }
                break;
        }
    }

    protected virtual void SpecialAttack()
    {
        if(CanUseSpecialAttack)
        {
            Attack(attackCount + 1);
            confidence -= data[attackCount + 1].confidenceCost;
            confidenceMeter.value = confidence;
        }
    }

    public override void EnableHitbox(int index)
    {
        hitboxEnabled = true;
        hitBoxes[index].gameObject.SetActive(true);
    }

    public override void DisableHitbox(int index)
    {
        hitBoxes[index].gameObject.SetActive(false);
        hitboxEnabled = false;
    }

    public override void EnableAttackVFX(int index)
    {
        vfxEnabled = true;
        attackVFX[index].SetActive(true);
    }

    public override void DisableAttackVFX(int index)
    {
        vfxEnabled = false;
        attackVFX[index].SetActive(false);
    }

    public override void EnterAttackState(int index)
    {
        cannotMove = true;
        isAttacking = true;
        audioSource.PlayOneShot(stats.attackSFX[currentAttackIndex], Random.Range(0.8f, 1));
        attackCount = index;
    }

    public void StartMovingForward(float speed)
    {
        forwardSpeed = speed;
        if(!isMovingForward)
        {
            isMovingForward = true;
            StartCoroutine(MoveForward());
        }
    }
    
    IEnumerator MoveForward()
    {
        while(isMovingForward)
        {
            MovingForward();
            yield return null;
        }
    }

    void MovingForward()
    {

        controller.Move(forwardSpeed * Time.deltaTime * transform.forward);
    }

    public void StopMovingForward()
    {
        isMovingForward = false;
        StopCoroutine(MoveForward());
    }

    public override void ExitAttackState()
    {
        isAttacking = false;
        
        if(isMovingForward)
        {
            StopMovingForward();
        }
        if(vfxEnabled)
        {
            DisableAttackVFX(currentAttackIndex);
        }
        if(hitboxEnabled)
        {
            DisableHitbox(currentAttackIndex);
        }
        if (!isHurt && !isDead)
            cannotMove = false;
    }
    #endregion

    #region Guarding
    protected override void StartGuarding()
    {
        isGuarding = true;
        anim.SetBool(guardingBoolName, true);
        health.guarding = true;
        cannotAttack = true;
        cannotMove = true;
        if (move)
            StopMoving();
    }

    protected override void StopGuarding()
    {
        isGuarding = false;
        anim.SetBool(guardingBoolName, false);
        health.guarding = false;
        cannotAttack = false;
        cannotMove = false;
    }

    #endregion

    #region Hurt
    public override void Hurt()
    {
        isHurt = true;
        audioSource.PlayOneShot(stats.hurtSFX);
        if (!switchHurtAnimation)
            anim.SetTrigger(hurtTriggerName);
        else
            anim.SetTrigger(hurt2TriggerName);
        switchHurtAnimation = !switchHurtAnimation;
        cannotMove = true;
        cannotAttack = true;
        if (move)
            StopMoving();
        ScreenShake();
        PauseTime();
    }

    public override void ExitHurtState()
    {
        cannotAttack = false;
        cannotMove = false;
        isHurt = false;
    }
    #endregion

    #region ScreenShake & Time-pause
    void ScreenShake()
    {
        multi.m_AmplitudeGain = screenShakeIntensity;
        StartCoroutine(ResetScreenShake());
    }

    void PauseTime()
    {
        Time.timeScale = 0;
        StartCoroutine(ResumeTime());
    }

    IEnumerator ResetScreenShake()
    {
        yield return screenShakeTime;
        multi.m_AmplitudeGain = 0;
    }

    IEnumerator ResumeTime()
    {
        yield return pauseTime;
        Time.timeScale = 1;
    }
    #endregion

    public override void Die()
    {
        audioSource.PlayOneShot(stats.dieSFX);
        anim.SetTrigger(dieTriggerName);
        cannotAttack = true;
        cannotMove = true;
        isDead = true;
        if (move)
            StopMoving();
        gameOverPanel.SetActive(true);
    }

    #region Weapon
    public override void WeaponTriggerEnter(Weapon weapon)
    {
        weaponDetected = true;
        detectedWeapon = weapon;
    }

    public override void WeaponTriggerExit(Weapon weapon)
    {
        if(detectedWeapon)
        {
            if (detectedWeapon == weapon)
            {
                weaponDetected = false;
                detectedWeapon = null;
            }
        }
    }

    protected override void EquipWeapon()
    {
        if (move)
            StopMoving();
        if(isUsingWeapon)
        {
            //drop current weapon
            DetachWeaponTransform();
            currentWeapon.Unequip();
        }
        else
        {
            isUsingWeapon = true;
        }
        anim.SetTrigger(pickUpWeaponTriggerName);
        detectedWeapon.Equip(this);
        //attach weapon transform to the weapon pivot
        detectedWeapon.transform.SetParent(weaponPivot);
        detectedWeapon.transform.localPosition = Vector3.zero;
        detectedWeapon.transform.localRotation = Quaternion.identity;
        currentWeapon = detectedWeapon;
        detectedWeapon = null;
    }

    protected override void ThrowWeapon()
    {
        if (move)
            StopMoving();
        anim.SetTrigger(throwTriggerName);
        DetachWeaponTransform();
        currentWeapon.Unequip();
        currentWeapon.Throw(weaponThrowForce * transform.forward);
        currentWeapon = null;
        isUsingWeapon = false;
    }

    protected override void DropWeapon()
    {
        DetachWeaponTransform();
        currentWeapon.Unequip();
        currentWeapon = null;
        isUsingWeapon = false;
    }

    public override void WeaponDisappear()
    {
        isUsingWeapon = false;
        currentWeapon = null;
    }

    void DetachWeaponTransform()
    {
        weaponPivot.DetachChildren();
        currentWeapon.transform.SetPositionAndRotation(weaponPivot.position, weaponPivot.rotation);
    }
    #endregion

    #region Jump
    protected virtual void Jump()
    {
        audioSource.PlayOneShot(stats.jumpSFX);
        anim.SetBool(jumpingBoolHash, true);
        isJumping = true;
        yVelocity = jumpVelocity;
        controller.stepOffset = 0;
        StartCoroutine(Jumping());
    }

    IEnumerator Jumping()
    {
        while(isJumping)
        {
            //If we start ground-checking right after the player presses the jump jump button
            //controller.isGrounded will return true since
            //the player collider hasn't move up high enough so it is still touching the ground
            if(!StartGroundCheck)
            {
                elapsedTimeSinceJumpButtonPress -= Time.deltaTime;
                yVelocity += gravity * Time.deltaTime;
            }
            //start ground-checking
            else
            {
                if (controller.isGrounded)
                {
                    Landing();
                    elapsedTimeSinceJumpButtonPress = 0.3f;
                    yield break;
                }
                else
                {
                    InMidAir();
                }
            }
            if (!move)
            {
                controller.Move(yVelocity * Time.deltaTime * Vector3.up);
            }
            yield return null;
        }
    }

    void InMidAir()
    {
        anim.SetBool(groundedBoolHash, false);
        if (yVelocity < 0)
        {
            yVelocity += 2 * gravity * Time.deltaTime;
            if (!falling)
            {
                anim.SetBool(fallingBoolHash, true);
                falling = true;
            }
        }
        else
        {
            yVelocity += gravity * Time.deltaTime;
        }
    }

    protected IEnumerator Falling()
    {
        while(falling)
        {
            if(!controller.isGrounded)
            {
                yVelocity += gravity * Time.deltaTime;
                if (!move)
                {
                    controller.Move(yVelocity * Time.deltaTime * Vector3.up);
                }
            }
            else
            {
                Landing();
                yield break;
            }
            yield return null;
        }
    }

    private void Landing()
    {
        falling = false;
        isJumping = false;
        anim.SetBool(groundedBoolHash, true);
        anim.SetBool(jumpingBoolHash, false);
        anim.SetBool(fallingBoolHash, false);
        controller.stepOffset = stepOffset;
        yVelocity = -0.5f;
    }
    #endregion

    #region Dash
    void Dash()
    {
        if(!isDashing)
        {
            if (move)
                StopMoving();
            isDashing = true;
            anim.SetBool(dashHash, false);
            dashTimer = 0;
            forwardSpeed = dashSpeed;
            StartCoroutine(Dashing());
        }
    }

    public void EnterDashState()
    {
        cannotMove = true;
        audioSource.PlayOneShot(stats.dashSFX);
    }

    public void ExitDashState()
    {
        cannotMove = false;
        isDashing = false;

    }

    IEnumerator Dashing()
    {
        while(dashTimer < dashDuration && isDashing)
        {
            dashTimer += Time.deltaTime;
            MovingForward();
            yield return null;
        }
        isDashing = false;
        anim.SetBool(dashHash, false);
    }
    #endregion

    private void OnAnimatorMove()
    {
        if (!isJumping && !falling && !isAttacking && !isDashing)
        {
            velocity = anim.deltaPosition;
            velocity.y = -0.25f;
            controller.Move(velocity);
        }
    }
}

