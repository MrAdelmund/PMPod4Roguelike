using UnityEngine;

public enum PlayerStatType
{
    MaxHealth,
    MoveSpeed,
    DamageMultiplier,
    HealthRegen
}

[CreateAssetMenu(menuName = "Roguelite/Upgrade")]
public class UpgradeSO : ScriptableObject
{
    [Header("Identity")]
    public string upgradeName;
    [TextArea]
    public string description;
    public Sprite icon;

    [Header("Upgrade Effect")]
    public PlayerStatType statType;
    public float flatBonus;
    public float percentBonus;

    [Header("Stacking")]
    public bool canStack = true;
    public int maxStacks = 0; // 0 = infinite
}
