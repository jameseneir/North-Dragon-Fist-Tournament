using UnityEngine;

public class AbilitiesSelectingButton : MonoBehaviour
{
    [SerializeField]
    AttackData data;

    public void SelectAbility()
    {
        GameManager.Instance.data.Add(data);
    }
}

