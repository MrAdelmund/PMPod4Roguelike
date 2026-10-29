using UnityEngine;

public enum ShopItemType { Weapon, Stat, Relic }

[CreateAssetMenu(fileName = "NewShopItem", menuName = "Shop/Shop Item")]
public class ShopItemSO : ScriptableObject
{
    public string itemName;
    public ShopItemType type;
    public int cost = 1;

    [Header("Weapon (if type=Weapon)")]
    public WeaponSO weapon;

    [Header("Stat Upgrade (if type=Stat)")]
    public UpgradeSO statUpgrade;

    [Header("Relic (if type=Relic)")]
    // public RelicSO relic; // future expansion

    public Sprite icon;          // optional for UI
    [TextArea] public string description; // for UI display
}
