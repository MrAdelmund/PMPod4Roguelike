using UnityEngine;

public class PlayerCurrency : MonoBehaviour
{
    public int gold = 0;

    public void AddGold(int amount)
    {
        gold += amount;
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
