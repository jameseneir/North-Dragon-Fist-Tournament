using UnityEngine;

public class Weapon : MonoBehaviour
{
    public bool isThrowing;
    [SerializeField]
    int weaponDamage;
    [SerializeField]
    int useTime;
    [SerializeField]
    int throwDamage;
    [SerializeField]
    int groundLayer;

    [SerializeField]
    Rigidbody rb;

    [SerializeField]
    GameObject weaponPickUpChild;
    [SerializeField]
    WeaponHitBox hitBox;
    CharacterComponent character;
    [SerializeField]
    BoxCollider[] colliders;

    [HideInInspector]
    public int targetLayer;

    [SerializeField]
    GameObject hitVFX;
    private void Start()
    {
        hitBox.SetUp(weaponDamage, this);
    }

    public void EnableHitBox()
    {
        hitBox.gameObject.SetActive(true);
    }

    public void DisableHitBox()
    {
        hitBox.gameObject.SetActive(false);
    }

    public void Equip(CharacterComponent cP)
    {
        character = cP;
        rb.isKinematic = true;
        rb.freezeRotation = true;
        hitBox.Equip(cP.gameObject.layer);
        weaponPickUpChild.SetActive(false);
        foreach(Collider col in colliders)
        {
            col.enabled = false;
        }
    }

    public void Unequip()
    {
        if(!isThrowing)
        {
            weaponPickUpChild.SetActive(true);
            foreach(Collider col in colliders)
            {
                col.enabled = true;
            }
        }
        rb.isKinematic = false;
        rb.freezeRotation = false;
        character = null;
    }

    public void Throw(Vector3 direction)
    {
        isThrowing = true;
        rb.AddForce(direction, ForceMode.Impulse);
    }

    public void HitSomething()
    {
        useTime--;
        if(useTime <= 0)
        {
            Disappear();
        }
    }

    void Disappear()
    {
        character.WeaponDisappear();
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(isThrowing)
        {
            if (collision.gameObject.layer == groundLayer)
            {
                isThrowing = false;
                foreach (Collider col in colliders)
                {
                    col.enabled = false;
                }
                weaponPickUpChild.SetActive(true);
            }
            else if(collision.gameObject.layer == targetLayer)
            {
                hitVFX.SetActive(true);
                hitVFX.transform.position = collision.GetContact(0).point;
                collision.gameObject.GetComponent<Health>().TakeDamage(throwDamage);
            }
        }
    }
}
