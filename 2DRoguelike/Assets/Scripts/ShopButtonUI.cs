using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ShopButtonUI : MonoBehaviour
{
    public Image iconImage;
    public TMP_Text nameText;
    public TMP_Text descriptionText;
    public TMP_Text costText;
    public Button button;

    private ShopItemSO item;
    private System.Action<ShopItemSO> onClick;

    public void Setup(ShopItemSO itemData, System.Action<ShopItemSO> callback)
    {
        item = itemData;
        onClick = callback;

        iconImage.sprite = item.icon;
        nameText.text = item.itemName;
        descriptionText.text = item.description;
        costText.text = $"Cost: {item.cost}";

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick?.Invoke(item));
    }
}
