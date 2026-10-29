using System.Collections.Generic;
using UnityEngine;

public class WeaponShopManager : MonoBehaviour
{
    public List<WeaponSO> allWeapons;
    public WeaponInventory weaponInventory;
    public UpgradeUIManager uiManager;

    public void ShowWeaponChoices()
    {
        if (weaponInventory.weapons.Count >= weaponInventory.maxWeapons)
            return;

        List<WeaponSO> choices = GetRandomWeapons(3);
        Time.timeScale = 0f;

        //uiManager.ShowWeaponChoices(choices, OnWeaponSelected);
    }

    void OnWeaponSelected(WeaponSO weapon)
    {
        weaponInventory.AddWeapon(weapon);
        Time.timeScale = 1f;
        FindObjectOfType<WaveManager>().OnShopClosed();
    }

    List<WeaponSO> GetRandomWeapons(int count)
    {
        List<WeaponSO> pool = new List<WeaponSO>(allWeapons);
        List<WeaponSO> results = new List<WeaponSO>();

        for (int i = 0; i < count && pool.Count > 0; i++)
        {
            int index = Random.Range(0, pool.Count);
            results.Add(pool[index]);
            pool.RemoveAt(index);
        }

        return results;
    }
}
