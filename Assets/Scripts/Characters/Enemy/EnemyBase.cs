using UnityEngine;
using System.Collections;
using System;

public class EnemyBase : CharacterComponent, IComparable<EnemyBase>
{
    [HideInInspector]
    public EnemyWave wave;

    protected Transform target;
    [SerializeField]
    protected Animator anim;
    [SerializeField]
    protected CharacterController controller;

    protected PlayerBase player;

    protected Vector3 toPlayer = Vector3.zero;

    public float Distance { get; private set; }
    float closeDistance;
    float farDistance;
    float atkRange;

    [SerializeField]
    AttackData[] data;

    [SerializeField]
    GameObject[] attackVFX;

    [SerializeField]
    HitBox[] hitBoxes;

    public EnemyStats enemy;
    public int State { get { return state; } }
    int state = -1;
    // 0: engage, 1: keep close distance, 2: keep far distance, 3: attacking, 4: hurt
    int previousState;
    //0: move closer, 1: idle, 2: back away, 3: guarding
    int substate;

    protected bool move;
    WaitForSeconds atkCoolDown;
    WaitForSeconds stateUpdate;
    bool close;
    WaitForSeconds slowStateUpdate;
    WaitForSeconds fastStateUpdate;

    protected float animationBlendDamp;
    [SerializeField]
    float randomAwakeDelay = 3f;

    [SerializeField]
    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        animationBlendDamp = enemy.animationBlendDamp;
        #region Set up ranges
        float randomRange = enemy.randomNoise;
        float initial = randomRange;
        randomRange = UnityEngine.Random.Range(-initial, initial);
        closeDistance = enemy.closeDistance * enemy.closeDistance + randomRange;
        farDistance = enemy.farDistance * enemy.farDistance + randomRange;
        atkRange = enemy.attackRange * enemy.attackRange;
        #endregion

        atkCoolDown = new WaitForSeconds(enemy.attackCooldown);
        slowStateUpdate = new WaitForSeconds(enemy.slowStateUpdateInterval);
        fastStateUpdate = new WaitForSeconds(enemy.fastStateUpdateInterval);
        stateUpdate = slowStateUpdate;

        for (int i = 0; i < data.Length; i++)
        {
            data[i].animatorHashesIndex = Animator.StringToHash(data[i].animatorTriggerName);
        }
    }

    public void AssignPlayer(PlayerBase playerBase, EnemyWave enemyWave)
    {
        player = playerBase;
        target = player.transform;
        wave = enemyWave;
        state = 0;
        randomAwakeDelay = UnityEngine.Random.Range(0f, 3f);
        Invoke(nameof(EnemyAwake), randomAwakeDelay);
    }

    void EnemyAwake()
    {
        StartCoroutine(StateCheck());
    }

    public void AssignRole(int role)
    {
        if (state < 3)
            state = role;
        else
            previousState = role;
    }

    IEnumerator StateCheck()
    {
        while(state < 3)
        {
            yield return stateUpdate;
            toPlayer = new Vector3(target.position.x - transform.position.x, 0, target.position.z - transform.position.z);
            Distance = toPlayer.sqrMagnitude;
            if (state == 0)
            {
                if (Distance > closeDistance)
                {
                    substate = 0;
                }
                else
                {
                    if(!close)
                    {
                        close = true;
                        stateUpdate = fastStateUpdate;
                    }
                    if (Distance > atkRange)
                    {
                        if (cannotAttack)
                        {
                            substate = 1;
                        }
                        else
                        {
                            substate = 0;
                        }
                    }
                    else
                    {
                        if (cannotAttack)
                        {
                            //idle
                            substate = 1;
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

            //keeping close distance
            else if (state == 1)
            {
                if (Distance > closeDistance)
                {
                    substate = 0;
                    if(close)
                    {
                        close = false;
                        stateUpdate = slowStateUpdate;
                    }
                }
                else if (Distance > atkRange)
                {
                    substate = 1;
                    if(!close)
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
                        substate = 2;
                    }
                }
            }
            //keeping far distance
            else if (state == 2)
            {
                if (Distance > farDistance)
                {
                    substate = 0;
                    if (close)
                    {
                        close = false;
                        stateUpdate = slowStateUpdate;
                    }
                }
                else if (Distance > closeDistance)
                {
                    substate = 1;
                    if (close)
                    {
                        close = false;
                        stateUpdate = slowStateUpdate;
                    }
                }
                else
                {
                    substate = 2;
                    if(!close)
                    {
                        close = true;
                        stateUpdate = fastStateUpdate;
                    }
                }
            }
        }
    }

    private void Update()
    {
        if (state > 2 || state < 0)
            return;

        toPlayer = new Vector3(target.position.x - transform.position.x, 0, target.position.z - transform.position.z);
        LookAtPlayer();

        switch(substate)
        {
            //move closer
            case 0:
                MoveCloser();
                break;
            case 1:
                Idle();
                break;
            case 2:
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

    protected virtual void BackAway()
    {

    }

    protected virtual void MoveCloser()
    {

    }

    #region attack
    protected virtual void SelectAttack()
    {
        //simple enemies only have 1 type of attack
        //complex enemies will have more types of attack
        Attack(0);
    }

    int currentAttack;
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

    bool hitboxEnabled, vfxEnabled;
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

    public override void EnterAttackState(int index)
    {
        audioSource.PlayOneShot(enemy.attackSFX[currentAttack], UnityEngine.Random.Range(0.8f, 1));
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
        StartCoroutine(StateCheck());
    }

    public void Damage()
    {
        if (isHurt || isDead)
            return;
    }

    IEnumerator AttackCoolDown()
    {
        yield return atkCoolDown;
        cannotAttack = false;
    }
    #endregion

    protected void Idle()
    {
        if (move)
        {
            move = false;
            anim.SetBool(moveHash, false);
        }
    }

    protected virtual void LookAtPlayer()
    {

    }

    public override void ExitHurtState()
    {
        if (isDead)
            return;
        isHurt = false;
        state = previousState;
        StartCoroutine(StateCheck());
    }

    public override void Hurt()
    {
        if (state < 3)
        {
            if (move)
                move = false;
            anim.SetBool(moveHash, false);
            previousState = state;
            StopCoroutine(StateCheck());
        }
        audioSource.PlayOneShot(enemy.hurtSFX);
        isHurt = true;
        state = 4;
        anim.SetTrigger(hurtTriggerName);
    }

    public override void Die()
    {
        state = 5;
        isDead = true;
        audioSource.PlayOneShot(enemy.dieSFX);
        anim.SetTrigger(dieTriggerName);
        wave.EnemyDie(this);
        PoolingManager.Instance.SpawnObj(PoolObjectType.GEM, transform.position + new Vector3(0f, 2.5f, 0f), null);
        Destroy(gameObject, enemy.dieAnimationDuration);
    }

    public int CompareTo (EnemyBase other)
    {
        return Mathf.RoundToInt(Distance - other.Distance);
    }

    Vector3 velocity;
    void OnAnimatorMove()
    {
        velocity = anim.deltaPosition;
        velocity.y = -0.5f;
        controller.Move(velocity);
    }
}

    
