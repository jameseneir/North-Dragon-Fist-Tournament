using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections;
using Cinemachine;

public class PlayerBase : CharacterComponent
{
    [SerializeField]
    protected PlayerInput input;

    [SerializeField]
    AudioSource audioSource;

    #region Movement
    protected Vector2 direction;
    protected Vector3 velocity;

    [SerializeField]
    protected CharacterController controller;
    float stepOffset;

    protected float jumpVelocity;
    [SerializeField]
    protected float gravity = -9.81f * 5;
    protected float yVel;
    [SerializeField]
    protected float midAirMoveSpeed;

    protected bool cannotMove;
    protected bool move;

    protected readonly int dashHash = Animator.StringToHash("Dash");
    protected readonly int jumpingBoolHash = Animator.StringToHash("IsJumping");
    protected readonly int fallingBoolHash = Animator.StringToHash("IsFalling");
    protected readonly int groundedBoolHash = Animator.StringToHash("IsGrounded");

    [SerializeField]
    protected float animationBlendDamp = 0.3f;
    #endregion

    [SerializeField]
    Health health;

    [SerializeField]
    protected Animator anim;

    PlayerUpgrades playerUpgrades;

    #region Attack
    protected RaycastHit[] buffer;

    [SerializeField]
    AttackData[] data;

    [SerializeField]
    float weaponThrowForce;

    bool isAttacking;
    bool isGuarding;

    int currentAttack;

    [SerializeField]
    GameObject[] attackVFX;

    [SerializeField]
    HitBox[] hitBoxes;

    [SerializeField]
    protected LayerMask enemyLayer;

    [SerializeField]
    protected Vector3 offset;

    [SerializeField]
    protected Vector3 proximityBox;

    [SerializeField]
    float snapDuration;

    int confidence;
    readonly int maxConfidence = 100;

    [SerializeField]
    UnityEngine.UI.Slider confidenceMeter;
    #endregion

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

    protected virtual void SetUp()
    {

    }

    private void Awake()
    {
        playerUpgrades = GetComponent<PlayerUpgrades>();
        jumpVelocity = Mathf.Sqrt(stats.jumpHeight * -2 * gravity);
        controller.Move(-0.5f * Vector3.up);
        stepOffset = controller.stepOffset;

        buffer = new RaycastHit[3];
        for(int i = 0; i < data.Length; i++)
        {
            data[i].animatorHashesIndex = Animator.StringToHash(data[i].animatorTriggerName);
        }

        #region SetUpInput
        //move
        string moveInputName = "Move";
        InputAction moveAction = input.actions[moveInputName];
        moveAction.started += ctx =>
        {
            if (!cannotMove && controller.isGrounded)
            {
                direction = ctx.ReadValue<Vector2>();
                
                if (!move)
                {
                    StartMoving();
                }
            }
        };
        moveAction.performed += ctx =>
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

        moveAction.canceled += ctx =>
        {
            if(move)
            {
                StopMoving();
            }
        };

        //jump
        string southButtonInputName = "Jump";
        InputAction southButtonInput = input.actions[southButtonInputName];
        southButtonInput.performed += ctx =>
        {
            if (controller.isGrounded && !cannotMove)
            {
                Jump();
            }
        }; 

        //heavy attack
        string northButtonInputName = "Heavy Attack";
        InputAction northButtonInput = input.actions[northButtonInputName];
        northButtonInput.performed += ctx => Attack(1);

        //pick up weapon
        string eastButtonInputName = "EastButton";
        InputAction eastButtonInput = input.actions[eastButtonInputName];
        eastButtonInput.performed += ctx =>
        {
            if (weaponDetected)
            {
                EquipWeapon();
            }
        };

        //light attack
        string westButtonInputName = "Normal Attack";
        InputAction westButtonInput = input.actions[westButtonInputName];
        westButtonInput.performed += ctx => Attack(0);

        //left trigger
        string leftTriggerInputName = "LeftTrigger";
        InputAction leftTriggerInput = input.actions[leftTriggerInputName];
        leftTriggerInput.performed += ctx =>
        {
            //do something
        };

        //guarding
        string rightTriggerInputName = "Blocking";
        InputAction rightTriggerInput = input.actions[rightTriggerInputName];
        rightTriggerInput.performed += ctx =>
        {
            if (isHurt || !controller.isGrounded)
                return;
            if(!isGuarding)
                StartGuarding();
        };
        rightTriggerInput.canceled += ctx =>
        {
            if(isGuarding)
                StopGuarding();
        };

        string leftShoulderInputName = "LeftShoulder";
        InputAction leftShoulderInput = input.actions[leftShoulderInputName];
        leftShoulderInput.performed += ctx =>
        {
            //do something
        };

        string rightShoulderInputName = "RightShoulder";
        InputAction rightShoulderInput = input.actions[rightShoulderInputName];
        rightShoulderInput.performed += ctx =>
        {
            //do something
        };
        SetUp();
        #endregion
        multi = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        screenShakeTime = new WaitForSecondsRealtime(screenShakeDuration);

        pauseTime = new WaitForSecondsRealtime(timeStopDuration);

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
        currentAttack = index;
        anim.SetTrigger(data[index].animatorHashesIndex);
    }

    bool hitboxEnabled, vfxEnabled;
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

    public override void EnterAttackState()
    {
        cannotMove = true;
        isAttacking = true;
        audioSource.PlayOneShot(stats.attackSFX[currentAttack], Random.Range(0.8f, 1));
    }

    bool isMovingForward;
    float forwardSpeed;
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
            DisableAttackVFX(currentAttack);
        }
        if(hitboxEnabled)
        {
            DisableHitbox(currentAttack);
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
        anim.SetTrigger(hurtTriggerName);
        cannotMove = true;
        cannotAttack = true;
        if (move)
            StopMoving();
        //screen shake
        multi.m_AmplitudeGain = screenShakeIntensity;
        StartCoroutine(ResetScreenShake());
        //time stop
        StartCoroutine(ResumeTime());
        Time.timeScale = 0;
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

    public override void ExitHurtState()
    {
        cannotAttack = false;
        cannotMove = false;
        isHurt = false;
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
            //switch weapon
            //drop current weapon
            weaponPivot.DetachChildren();
            currentWeapon.transform.SetPositionAndRotation(weaponPivot.position, weaponPivot.rotation);
            currentWeapon.Unequip();
            
            //equip new weapon
            anim.SetTrigger(pickUpWeaponTriggerName);
            detectedWeapon.Equip(this);
            detectedWeapon.transform.SetParent(weaponPivot);
            detectedWeapon.transform.localPosition = Vector3.zero;
            detectedWeapon.transform.localRotation = Quaternion.identity;
            currentWeapon = detectedWeapon;
            detectedWeapon = null;
        }
        else
        {
            //pick up
            anim.SetTrigger(pickUpWeaponTriggerName);
            detectedWeapon.Equip(this);
            detectedWeapon.transform.SetParent(weaponPivot);
            detectedWeapon.transform.localPosition = Vector3.zero;
            detectedWeapon.transform.localRotation = Quaternion.identity;
            currentWeapon = detectedWeapon;
            detectedWeapon = null;
            isUsingWeapon = true;
        }
    }

    protected override void ThrowWeapon()
    {
        if (move)
            StopMoving();
        anim.SetTrigger(throwTriggerName);
        weaponPivot.DetachChildren();
        currentWeapon.transform.SetPositionAndRotation(weaponPivot.position, weaponPivot.rotation);
        currentWeapon.Unequip();
        currentWeapon.Throw(weaponThrowForce * transform.forward);
        currentWeapon = null;
        isUsingWeapon = false;
    }

    protected override void DropWeapon()
    {
        weaponPivot.DetachChildren();
        currentWeapon.transform.SetPositionAndRotation(weaponPivot.position, weaponPivot.rotation);
        currentWeapon.Unequip();
        currentWeapon = null;
        isUsingWeapon = false;
    }

    public override void WeaponDisappear()
    {
        isUsingWeapon = false;
        currentWeapon = null;
    }
    #endregion

    #region Jump
    protected bool isJumping;
    protected bool falling;
    protected virtual void Jump()
    {
        audioSource.PlayOneShot(stats.jumpSFX);
        anim.SetBool(jumpingBoolHash, true);
        isJumping = true;
        yVel = jumpVelocity;
        controller.stepOffset = 0;
        StartCoroutine(Jumping());
    }

    protected float timer = 0.3f;
    IEnumerator Jumping()
    {
        while(isJumping)
        {
            if(timer > 0)
            {
                timer -= Time.deltaTime;
                yVel += gravity * Time.deltaTime;
            }
            else
            {
                if (controller.isGrounded)
                {
                    falling = false;
                    isJumping = false;
                    anim.SetBool(groundedBoolHash, true);
                    anim.SetBool(jumpingBoolHash, false);
                    anim.SetBool(fallingBoolHash, false);
                    yVel = -0.5f;
                    timer = 0.3f;
                    controller.stepOffset = stepOffset;
                    yield break;
                }
                else
                {
                    
                    anim.SetBool(groundedBoolHash, false);
                    if (yVel < 0)
                    {
                        yVel += 2 * gravity * Time.deltaTime;
                        if (!falling)
                        {
                            anim.SetBool(fallingBoolHash, true);
                            falling = true;
                        }
                    }
                    else
                    {
                        yVel += gravity * Time.deltaTime;
                    }
                }
            }
            if (!move)
            {
                controller.Move(yVel * Time.deltaTime * Vector3.up);
            }
            yield return null;
        }
    }

    protected IEnumerator Falling()
    {
        while(falling)
        {
            if(!controller.isGrounded)
            {
                yVel += gravity * Time.deltaTime;
                if (!move)
                {
                    controller.Move(yVel * Time.deltaTime * Vector3.up);
                }
            }
            else
            {
                falling = false;
                isJumping = false;
                anim.SetBool(groundedBoolHash, true);
                anim.SetBool(jumpingBoolHash, false);
                anim.SetBool(fallingBoolHash, false);
                controller.stepOffset = stepOffset;
                yVel = -0.5f;
                yield break;
            }
            yield return null;
        }
    }
    #endregion

    #region Dash
    bool isDashing;
    [SerializeField]
    float dashSpeed;
    [SerializeField]
    float dashDuration;
    float dashTimer;
    void Dash()
    {
        if(!isDashing)
        {
            isDashing = true;
            anim.SetBool(dashHash, false);
            dashTimer = 0;
            forwardSpeed = dashSpeed;
            StartCoroutine(Dashing());
        }
    }

    public void EnterDashState()
    {
        audioSource.PlayOneShot(stats.dashSFX);
    }

    public void ExitDashState()
    {
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

    public void GetItem(CollectableType collectableType)
    {
        switch(collectableType)
        {
            case CollectableType.GEM:
                playerUpgrades.EarnXPPoints();
                break;
        }
    }

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

