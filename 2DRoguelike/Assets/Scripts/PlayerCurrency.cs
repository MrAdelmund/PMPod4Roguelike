using UnityEngine;

public class PlayerCurrency : MonoBehaviour
{
    public float gold = 100;
    private PlayerStats stats;
    void Awake()
    {
        stats = GetComponent<PlayerStats>();
    }

    public void AddGold(int amount)
    {
        gold += amount * stats.goldBoost;
        // Optionally trigger UI update
    }

    public bool SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            return true;
        }

        return false; // Not enough
    }
}
