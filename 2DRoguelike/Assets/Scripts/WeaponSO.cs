using UnityEngine;

[CreateAssetMenu(menuName = "Roguelite/Weapon")]
public class WeaponSO : ScriptableObject
{
    [Header("Identity")]
    public string weaponName;
    public Sprite sprite;

    [Header("Combat Stats")]
    public float damage = 10f;
    public float fireRate = 1f; // shots per second
    public float projectileSpeed = 10f;
    public float range = 5f;

    [Header("Prefabs")]
    public Projectile projectilePrefab;
}
