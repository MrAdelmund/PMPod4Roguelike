using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PlayerStats))]
public class WeaponController : MonoBehaviour
{
    private List<WeaponSO> equippedWeapons = new List<WeaponSO>();
    private Dictionary<WeaponSO, float> fireTimers = new Dictionary<WeaponSO, float>();
    private PlayerStats stats;

    [Header("Weapon Slots (child transforms around player)")]
    public List<Transform> weaponSlots;

    void Awake()
    {
        stats = GetComponent<PlayerStats>();
    }

    void Update()
    {
        for (int i = 0; i < equippedWeapons.Count; i++)
        {
            WeaponSO weapon = equippedWeapons[i];
            fireTimers[weapon] += Time.deltaTime;
            float interval = 1f / weapon.fireRate;

            if (fireTimers[weapon] >= interval)
            {
                // Pass the corresponding slot as the spawn point
                Fire(weapon, i < weaponSlots.Count ? weaponSlots[i] : transform);
                fireTimers[weapon] = 0f;
            }
        }
    }

    public void RefreshWeapons(List<WeaponSO> weapons)
    {
        equippedWeapons = new List<WeaponSO>(weapons);

        fireTimers.Clear();
        for (int i = 0; i < equippedWeapons.Count; i++)
        {
            if(!fireTimers.ContainsKey(equippedWeapons[i]))
                fireTimers.Add(equippedWeapons[i], 0f);
        }

        // Assign sprites to weapon slots
        for (int i = 0; i < weaponSlots.Count; i++)
        {
            SpriteRenderer sr = weaponSlots[i].GetComponent<SpriteRenderer>();
            if (sr == null) continue; // safety check

            if (i < equippedWeapons.Count)
            {
                sr.sprite = equippedWeapons[i].sprite; // set sprite from WeaponSO
                sr.enabled = true;                   // make sure sprite renderer is visible
            }
            else
            {
                sr.sprite = null;                    // hide empty slots
                sr.enabled = false;
            }
        }
    }


    private void Fire(WeaponSO weapon, Transform spawnPoint)
    {
        Enemy target = GetNearestEnemy();
        if (target == null) return;

        Vector2 dir = (target.transform.position - spawnPoint.GetChild(0).transform.position).normalized;

        // Rotate the spawn point (weapon sprite) to face the target
        spawnPoint.right = dir; // assumes the sprite points "up"

        Projectile proj = Instantiate(
            weapon.projectilePrefab,
            spawnPoint.GetChild(0).transform.position,
            Quaternion.identity
        );

        // Make projectile face the same direction as weapon
        proj.transform.right = dir;

        float finalDamage = weapon.damage * stats.damageMultiplier;

        proj.Initialize(
            finalDamage,
            weapon.projectileSpeed,
            weapon.range,
            dir
        );
    }

    private Enemy GetNearestEnemy()
    {
        Enemy closest = null;
        float minDist = float.MaxValue;

        foreach (Enemy e in Enemy.ActiveEnemies)
        {
            if (e == null) continue;

            float d = Vector2.Distance(transform.position, e.transform.position);
            if (d < minDist)
            {
                minDist = d;
                closest = e;
            }
        }

        return closest;
    }
}
