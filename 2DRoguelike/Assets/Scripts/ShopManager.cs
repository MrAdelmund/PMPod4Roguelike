using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [Header("References")]
    public PlayerCurrency playerCurrency;
    public WeaponInventory weaponInventory;
    public PlayerStats playerStats;

    public GameObject shopPanel;

    public Transform statContainer;
    public Transform weaponContainer;
    public Transform relicContainer;
    public TMP_Text goldText;
    public Button rerollButton;
    public ShopButtonUI buttonPrefab;

    public int baseRerollCost = 5;
    private int currentRerollCost;

    [Header("Item Pools")]
    public List<ShopItemSO> statPool;
    public List<ShopItemSO> weaponPool;
    public List<ShopItemSO> relicPool;

    private List<ShopItemSO> currentShopItems = new List<ShopItemSO>();

    void Awake()
    {
        shopPanel.SetActive(false);
        currentRerollCost = baseRerollCost;
    }

    public void OpenShop()
    {
        shopPanel.SetActive(true);
        GenerateShopItems();
        DisplayShop();
        UpdateGoldUI();

        Time.timeScale = 0f;
    }

    void GenerateShopItems()
    {
        currentShopItems.Clear();

        currentShopItems.AddRange(GetRandomItems(statPool, 4));
        currentShopItems.AddRange(GetRandomItems(weaponPool, 2));
        currentShopItems.AddRange(GetRandomItems(relicPool, 2));
    }

    List<ShopItemSO> GetRandomItems(List<ShopItemSO> pool, int count)
    {
        List<ShopItemSO> choices = new List<ShopItemSO>();
        List<ShopItemSO> tempPool = new List<ShopItemSO>(pool);

        for (int i = 0; i < count && tempPool.Count > 0; i++)
        {
            int index = Random.Range(0, tempPool.Count);
            choices.Add(tempPool[index]);
            tempPool.RemoveAt(index);
        }

        return choices;
    }

    void DisplayShop()
    {
        rerollButton.GetComponentInChildren<TMP_Text>().text = "Reroll: "+ currentRerollCost.ToString();
        foreach (Transform t in statContainer) Destroy(t.gameObject);
        foreach (Transform t in weaponContainer) Destroy(t.gameObject);
        foreach (Transform t in relicContainer) Destroy(t.gameObject);

        foreach (ShopItemSO item in currentShopItems)
        {
            Transform parent = item.type switch
            {
                ShopItemType.Stat => statContainer,
                ShopItemType.Weapon => weaponContainer,
                ShopItemType.Relic => relicContainer,
                _ => statContainer
            };

            ShopButtonUI button = Instantiate(buttonPrefab, parent);
            button.Setup(item, OnItemPurchased);
        }
    }

    void OnItemPurchased(ShopItemSO item)
    {
        if (!playerCurrency.SpendGold(item.cost))
        {
            Debug.Log("Not enough gold!");
            return;
        }

        switch (item.type)
        {
            case ShopItemType.Weapon:
                weaponInventory.AddWeapon(item.weapon);
                break;
            case ShopItemType.Stat:
                playerStats.ApplyUpgrade(item.statUpgrade);
                break;
            case ShopItemType.Relic:
                // future
                break;
        }

        currentShopItems.Remove(item);
        DisplayShop();
        UpdateGoldUI();
    }

    public void RerollShop()
    {
        if (!playerCurrency.SpendGold(currentRerollCost))
            return;

        currentRerollCost += 5;
        GenerateShopItems();
        DisplayShop();
        UpdateGoldUI();
        rerollButton.GetComponentInChildren<TMP_Text>().text = "Reroll: " + currentRerollCost.ToString();
    }

    void UpdateGoldUI()
    {
        // Update your UI text
        goldText.text = "" + playerCurrency.gold + "g";
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
        Time.timeScale = 1f;
        FindObjectOfType<WaveManager>().OnShopClosed();
        currentRerollCost = baseRerollCost;
    }
}
