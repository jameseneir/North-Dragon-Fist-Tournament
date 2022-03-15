using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUpgrades : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private FloatReference PlayerXP;
    public bool ResetXP;


    [Header("UI")]
    public Slider XpSLider;

    public float XP;
    public float CurrentMaxXP = 5;

    private void Start()
    {
        XP = PlayerXP.Value;

        if (XpSLider != null)
        {
            XpSLider.maxValue = CurrentMaxXP;
            XpSLider.value = XP;
            XpSLider.minValue = 0;
        }
    }

    public void EarnXPPoints()
    {
        PlayerXP.Value += 1f;
        XP = PlayerXP.Value;
        if (XpSLider != null)
            XpSLider.value = XP;
    }

    private void OnApplicationQuit()
    {
        if(ResetXP)
        {
            PlayerXP.Value = 0f;
        }
    }
}
