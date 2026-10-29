using UnityEngine;

public class LevelUpHandler : MonoBehaviour
{
    public PlayerXP playerXP;
    public UpgradeManager upgradeManager;

    void Start()
    {
        playerXP.OnLevelUp += upgradeManager.ShowUpgradeChoices;
    }
}
