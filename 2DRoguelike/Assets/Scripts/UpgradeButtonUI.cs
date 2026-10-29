using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeButtonUI : MonoBehaviour
{
    public Image iconImage;
    public TMP_Text nameText;
    public TMP_Text descriptionText;
    public Button button;

    private UpgradeSO upgrade;

    public void Setup(UpgradeSO upgradeData, System.Action<UpgradeSO> onClick)
    {
        upgrade = upgradeData;

        iconImage.sprite = upgrade.icon;
        nameText.text = upgrade.upgradeName;
        descriptionText.text = upgrade.description;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick.Invoke(upgrade));
    }
    public void SetupWeapon(WeaponSO weapon, System.Action<WeaponSO> onClick)
    {
        iconImage.sprite = weapon.sprite;
        nameText.text = weapon.weaponName;
        descriptionText.text =
            $"Damage: {weapon.damage}\nFire Rate: {weapon.fireRate}";

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick.Invoke(weapon));
    }

}
