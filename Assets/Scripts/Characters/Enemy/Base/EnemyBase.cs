using UnityEngine;
using System.Collections;
using System;

public class EnemyBase : CharacterComponent, IComparable<EnemyBase>
{
    public EnemyStats enemyStats;

    #region Movement
    protected Transform target;
    [SerializeField]
    protected Animator anim;
    [SerializeField]
    protected CharacterController controller;
    protected PlayerBase player;
    protected Vector3 toPlayer = Vector3.zero;
    public float Distance { get; private set; }

    Vector3 velocity;
    #endregion

    #region Attack
    [SerializeField]
    AttackData[] data;

    [SerializeField]
    GameObject[] attackVFX;

    [SerializeField]
    HitBox[] hitBoxes;

    WaitForSeconds atkCoolDown;

    int currentAttack;
    bool hitboxEnabled, vfxEnabled;
    #endregion

    #region StateMachine
    public int State { get { return state; } }
    int state = -1;
    // 0: engage, 1: keep close distance, 2: keep far distance, 3: attacking, 4: hurt
    int previousState;

    Enum currentSubState;
    enum SubState
    {
        MOVE_CLOSER,
        IDLE,
        BACK_AWAY,
        GUARDING,
    }

    protected bool move;
    WaitForSeconds stateUpdate;
    bool close;
    WaitForSeconds slowStateUpdate;
    WaitForSeconds fastStateUpdate;

    float closeDistance;
    float farDistance;
    float atkRange;
    #endregion

    protected float animationBlendDamp;

    [SerializeField]
    float randomAwakeDelay = 3f;

    [SerializeField]
    AudioSource audioSource;

    [HideInInspector]
    public EnemyWave wave;
    // Start is called before the first frame update
    void Start()
    {
        animationBlendDamp = enemyStats.animationBlendDamp;
        #region Set up ranges
        float randomRange = enemyStats.randomNoise;
        float initial = randomRange;
        randomRange = UnityEngine.Random.Range(-initial, initial);
        closeDistance = enemyStats.closeDistance * enemyStats.closeDistance + randomRange;
        farDistance = enemyStats.farDistance * enemyStats.farDistance + randomRange;
        atkRange = enemyStats.attackRange * enemyStats.attackRange;
        #endregion

        atkCoolDown = new WaitForSeconds(enemyStats.attackCooldown);
        slowStateUpdate = new WaitForSeconds(enemyStats.slowStateUpdateInterval);
        fastStateUpdate = new WaitForSeconds(enemyStats.fastStateUpdateInterval);
        stateUpdate = slowStateUpdate;

        //parse animator string values to hashes to improve performance
        for (int i = 0; i < data.Length; i++)
        {
            data[i].animatorHashesIndex = Animator.StringToHash(data[i].animatorTriggerName);
        }
    }

    private void Update()
    {
        //skip update if the enemy is attacking, hurt or dead
        if (state > 2 || state < 0)
            return;

        toPlayer = new Vector3(target.position.x - transform.position.x, 0, target.position.z - transform.position.z);
        LookAtPlayer();

        switch (currentSubState)
        {
            case SubState.MOVE_CLOSER:
                MoveCloser();
                break;
            case SubState.IDLE:
                Idle();
                break;
            case SubState.BACK_AWAY:
                BackAway();
                break;
                /*case 3:
                    if(!isGuarding)
                    {
                        StartGuarding();
                    }
                    break;*/
        }
    }

    public void AssignPlayer(PlayerBase playerBase, EnemyWave enemyWave)
    {
        player = playerBase;
        target = player.transform;
        wave = enemyWave;
        state = 0;
        randomAwakeDelay = UnityEngine.Random.Range(0f, 3f);
        Invoke(nameof(StartEnemyUpdate), randomAwakeDelay);
    }

    void StartEnemyUpdate()
    {
        StartCoroutine(ChangeStateIfNeeded());
    }

    public void AssignRole(int role)
    {
        //if the enemy is in attacking state or hurt state
        //we will assign the role later on when that state is finished
        if (state < 3)
            state = role;
        else
            previousState = role;
    }

    IEnumerator ChangeStateIfNeeded()
    {
        while(state < 3)
        {
            yield return stateUpdate;
            toPlayer = new Vector3(target.position.x - transform.position.x, 0, target.position.z - transform.position.z);
            Distance = toPlayer.sqrMagnitude;
            if (state == 0)
            {
                Engage();
            }
            else if (state == 1)
            {
                KeepCloseDistance();
            }
            else if (state == 2)
            {
                KeepFarDistance();
            }
        }
    }

    void Engage()
    {
        if (Distance > closeDistance)
        {
            currentSubState = SubState.MOVE_CLOSER;
        }
        else
        {
            if (!close)
            {
                close = true;
                stateUpdate = fastStateUpdate;
            }
            if (Distance > atkRange)
            {
                if (cannotAttack)
                {
                    currentSubState = SubState.IDLE;
                }
                else
                {
                    currentSubState = SubState.MOVE_CLOSER;
                }
            }
            else
            {
                if (cannotAttack)
                {
                    //idle
                    currentSubState = SubState.IDLE;
                    //guarding
                    //substate = 3;

                }
                else
                {
                    SelectAttack();
                }
            }
        }
    }

    void KeepCloseDistance()
    {
        if (Distance > closeDistance)
        {
            currentSubState = SubState.MOVE_CLOSER;
            if (close)
            {
                close = false;
                stateUpdate = slowStateUpdate;
            }
        }
        else if (Distance > atkRange)
        {
            currentSubState = SubState.IDLE;
            if (!close)
            {
                close = true;
                stateUpdate = fastStateUpdate;
            }
        }
        else
        {
            if (!close)
            {
                close = true;
                stateUpdate = fastStateUpdate;
            }
            if (!cannotAttack)
            {
                SelectAttack();
            }
            else
            {
                currentSubState = SubState.BACK_AWAY;
            }
        }
    }

    void KeepFarDistance()
    {
        if (Distance > farDistance)
        {
            currentSubState = SubState.MOVE_CLOSER;
            if (close)
            {
                close = false;
                stateUpdate = slowStateUpdate;
            }
        }
        else if (Distance > closeDistance)
        {
            currentSubState = SubState.IDLE;
            if (close)
            {
                close = false;
                stateUpdate = slowStateUpdate;
            }
        }
        else
        {
            currentSubState = SubState.BACK_AWAY;
            if (!close)
            {
                close = true;
                stateUpdate = fastStateUpdate;
            }
        }
    }

    protected void Idle()
    {
        if (move)
        {
            move = false;
            anim.SetBool(moveHash, false);
        }
    }

    protected virtual void MoveToDestination(Vector3 destination)
    {

    }

    protected virtual void BackAway()
    {

    }

    protected virtual void MoveCloser()
    {

    }

    protected virtual void LookAtPlayer()
    {

    }

    #region Attack
    protected virtual void SelectAttack()
    {
        //simple enemies only have 1 type of attack
        //complex enemies will have more types of attack
        Attack(0);
    }

    void Attack(int index)
    {
        if(state < 3)
        {
            currentAttack = index;
            move = false;
            previousState = state;
            anim.SetBool(moveHash, false);
            state = 3;
            anim.SetTrigger(data[index].animatorHashesIndex);
            cannotAttack = true;
            StartCoroutine(AttackCoolDown());
        }
        else if(state == 3)
        {
            anim.SetTrigger(data[index].animatorHashesIndex);
            cannotAttack = true;
            StartCoroutine(AttackCoolDown());
        }
    }

    public override void EnableHitbox(int index)
    {
        hitboxEnabled = true;
        hitBoxes[index].gameObject.SetActive(true);
    }

    public override void DisableHitbox(int index)
    {
        hitboxEnabled = false;
        hitBoxes[index].gameObject.SetActive(false);
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
        audioSource.PlayOneShot(enemyStats.attackSFX[currentAttack], UnityEngine.Random.Range(0.8f, 1));
    }

    public override void ExitAttackState()
    {
        if (isDead)
            return;
        if (isMovingForward)
        {
            StopMovingForward();
        }
        if (vfxEnabled)
        {
            DisableAttackVFX(currentAttack);
        }
        if (hitboxEnabled)
        {
            DisableHitbox(currentAttack);
        }
        state = previousState;
        StartCoroutine(ChangeStateIfNeeded());
    }

    IEnumerator AttackCoolDown()
    {
        yield return atkCoolDown;
        cannotAttack = false;
    }
    #endregion

    #region ForwardMovement
    bool isMovingForward;
    float forwardSpeed;
    public void StartMovingForward(float speed)
    {
        forwardSpeed = speed;
        if (!isMovingForward)
        {
            isMovingForward = true;
            StartCoroutine(MoveForward());
        }
    }

    IEnumerator MoveForward()
    {
        while (isMovingForward)
        {
            controller.Move(forwardSpeed * Time.deltaTime * transform.forward);
            yield return null;
        }
    }

    public void StopMovingForward()
    {
        isMovingForward = false;
        StopCoroutine(MoveForward());
    }
    #endregion

    #region Hurt
    public override void ExitHurtState()
    {
        if (isDead)
            return;
        isHurt = false;
        state = previousState;
        StartCoroutine(ChangeStateIfNeeded());
    }

    public override void Hurt()
    {
        if (state < 3)
        {
            if (move)
                move = false;
            anim.SetBool(moveHash, false);
            previousState = state;
            StopCoroutine(ChangeStateIfNeeded());
        }
        audioSource.PlayOneShot(enemyStats.hurtSFX);
        isHurt = true;
        state = 4;
        if (!use2HurtAnim)
            anim.SetTrigger(hurtTriggerName);
        else
            anim.SetTrigger(hurt2TriggerName);
        use2HurtAnim = !use2HurtAnim;
    }
    #endregion

    public override void Die()
    {
        state = 5;
        isDead = true;
        audioSource.PlayOneShot(enemyStats.dieSFX);
        anim.SetTrigger(dieTriggerName);
        wave.EnemyDie(this);
        PoolingManager.Instance.SpawnObj(PoolObjectType.GEM, transform.position + new Vector3(0f, 2.5f, 0f), null);
        Destroy(gameObject, enemyStats.dieAnimationDuration);
    }

    public int CompareTo (EnemyBase other)
    {
        return Mathf.RoundToInt(Distance - other.Distance);
    }

    void OnAnimatorMove()
    {
        velocity = anim.deltaPosition;
        velocity.y = -0.5f;
        controller.Move(velocity);
    }
}

    
