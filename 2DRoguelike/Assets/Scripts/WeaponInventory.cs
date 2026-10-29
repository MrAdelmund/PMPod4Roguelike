using System.Collections.Generic;
using UnityEngine;

public class WeaponInventory : MonoBehaviour
{
    public int maxWeapons = 6;
    public List<WeaponSO> weapons = new List<WeaponSO>();

    private WeaponController weaponController;

    void Awake()
    {
        weaponController = GetComponent<WeaponController>();
    }

    public void AddWeapon(WeaponSO weapon)
    {
        if (weapons.Count >= maxWeapons)
            return;

        weapons.Add(weapon);
        weaponController.RefreshWeapons(weapons);
    }
}
