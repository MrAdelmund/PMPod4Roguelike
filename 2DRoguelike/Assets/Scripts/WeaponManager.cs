using UnityEngine;
using System.Collections.Generic;

public class WeaponManager : MonoBehaviour
{
    [Header("References")]
    public WeaponInventory inventory;    // Reference to your existing WeaponInventory
    public List<Transform> weaponSlots;  // child transforms around player
    public GameObject bulletPrefab;

    private List<WeaponSO> equippedWeapons = new List<WeaponSO>();

    void Awake()
    {
        equippedWeapons = inventory.weapons; // method you already have
        // Assign sprites to slots
        for (int i = 0; i < equippedWeapons.Count && i < weaponSlots.Count; i++)
        {
            WeaponSO weapon = equippedWeapons[i];
            Transform slot = weaponSlots[i];

            SpriteRenderer sr = slot.GetComponent<SpriteRenderer>();
            sr.sprite = weapon.sprite;
        }
    }

    void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        for (int i = 0; i < equippedWeapons.Count && i < weaponSlots.Count; i++)
        {
            WeaponSO weapon = equippedWeapons[i];
            Transform slot = weaponSlots[i];

            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mouseWorld - slot.position).normalized;

            // Rotate weapon sprite
            slot.up = direction;

            // Spawn bullet
            GameObject bullet = Instantiate(bulletPrefab, slot.position, Quaternion.identity);
            bullet.transform.up = direction;

            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.velocity = direction * weapon.projectileSpeed;
        }
    }

    // Call this if inventory updates (player adds a weapon mid-game)
    public void RefreshWeapons()
    {
        equippedWeapons = inventory.weapons;

        for (int i = 0; i < weaponSlots.Count; i++)
        {
            SpriteRenderer sr = weaponSlots[i].GetComponent<SpriteRenderer>();
            if (i < equippedWeapons.Count)
            {
                sr.sprite = equippedWeapons[i].sprite;
            }
            else
            {
                sr.sprite = null;
            }
        }
    }
}
