using UnityEngine.UI;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int HP { get; protected set; }
    public int MaxHP { get; protected set; }

    [HideInInspector]
    public bool guarding;

    [SerializeField]
    private CharacterComponent character;

    public Slider HPSlider;

    private void OnEnable()
    {
        SetUp();
    }

    protected virtual void SetUp()
    {
        MaxHP = character.stats.maxHealth;
        HP = MaxHP;
        if (HPSlider != null)
        {
            HPSlider.maxValue = HP;
            HPSlider.minValue = 0;
            HPSlider.value = HP;
        }
    }

    public virtual void Recover(int amount, bool fullRecovery)
    {
        if(fullRecovery)
        {
            HP = MaxHP;
        }
        else
        {
            HP += amount;
            if (HP > MaxHP)
                HP = MaxHP;
        }
        if (HPSlider != null)
            HPSlider.value = HP;
    }

    public virtual void TakeDamage(int amount)
    {
        if (character.isDead)
            return;
        if(guarding)
        {
            //not taking damage
        }
        else
        {
            HP -= amount;
            if (HP <= 0)
            {
                character.Die();
                HP = 0;
                if (HPSlider != null)
                    HPSlider.value = 0;
            }
            else
            {
                character.Hurt();
                if (HPSlider != null)
                    HPSlider.value = HP;
            }
        }
    }

    public void TakeDamageNoHurtAnimation(int amount)
    {
        if (character.isDead)
            return;
        HP -= amount;
        if (HP <= 0)
        {
            character.Die();
            HP = 0;
            if (HPSlider != null)
                HPSlider.value = 0;
        }
        else
        {
            if (HPSlider != null)
                HPSlider.value = HP;
        }
    }
}
