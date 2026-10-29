using System.Collections.Generic;
using UnityEngine;

public class UpgradeUIManager : MonoBehaviour
{
    public GameObject upgradePanel;
    public List<UpgradeButtonUI> upgradeButtons;

    void Awake()
    {
        upgradePanel.SetActive(false);
    }

    public void ShowUpgrades(List<UpgradeSO> upgrades, System.Action<UpgradeSO> onSelect)
    {
        upgradePanel.SetActive(true);

        for (int i = 0; i < upgradeButtons.Count; i++)
        {
            if (i < upgrades.Count)
            {
                upgradeButtons[i].gameObject.SetActive(true);

                // Wire the callback
                upgradeButtons[i].Setup(upgrades[i], (selectedUpgrade) =>
                {
                    onSelect?.Invoke(selectedUpgrade);

                    // **Close panel immediately after selection**
                    ClosePanel();
                });
            }
            else
            {
                upgradeButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void ClosePanel()
    {
        upgradePanel.SetActive(false);
        Time.timeScale = 1;
    }
}
