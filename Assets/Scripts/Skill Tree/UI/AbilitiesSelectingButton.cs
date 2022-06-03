using UnityEngine;

public class AbilitiesSelectingButton : MonoBehaviour
{
    [SerializeField]
    AttackData data;

    public void SelectAbility()
    {
        UnlockedList.instance.AddAbility(data);
    }
}

