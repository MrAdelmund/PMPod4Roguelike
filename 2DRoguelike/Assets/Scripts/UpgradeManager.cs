using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public List<UpgradeSO> allUpgrades;
    public PlayerStats playerStats;

    public void ShowUpgradeChoices()
    {
        Time.timeScale = 0f;

        List<UpgradeSO> choices = GetRandomUpgrades(3);
        upgradeUI.ShowUpgrades(choices, ApplyUpgrade);
    }

    List<UpgradeSO> GetRandomUpgrades(int count)
    {
        List<UpgradeSO> pool = new List<UpgradeSO>(allUpgrades);
        List<UpgradeSO> results = new List<UpgradeSO>();

        for (int i = 0; i < count && pool.Count > 0; i++)
        {
            int index = Random.Range(0, pool.Count);
            results.Add(pool[index]);
            pool.RemoveAt(index);
        }

        return results;
    }

    public UpgradeUIManager upgradeUI;


    public void ApplyUpgrade(UpgradeSO upgrade)
    {
        playerStats.ApplyUpgrade(upgrade);
        FindObjectOfType<WaveManager>().OnShopClosed();
    }

}
