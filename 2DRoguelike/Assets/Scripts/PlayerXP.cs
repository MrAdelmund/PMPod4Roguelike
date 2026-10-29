using UnityEngine;

public class PlayerXP : MonoBehaviour
{
    public int level = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 5;

    public delegate void LevelUpAction();
    public event LevelUpAction OnLevelUp;

    public void GainXP(int amount)
    {
        currentXP += amount;

        if (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        currentXP -= xpToNextLevel;
        level++;
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.5f);

        OnLevelUp?.Invoke();
    }
}
