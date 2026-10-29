using UnityEngine;

[CreateAssetMenu(menuName = "Roguelite/Player Class")]
public class PlayerClassSO : ScriptableObject
{
    [Header("Identity")]
    public string className;
    [TextArea]
    public string description;
    public Sprite icon;

    [Header("Base Stats")]
    public float maxHealth = 100f;
    public float moveSpeed = 5f;
    public float damageMultiplier = 1f;
    public float regenInterval = 0.5f;
    public float regenAmount = 0.2f;

    [Header("Starting Loadout")]
    public WeaponSO startingWeapon;
}
