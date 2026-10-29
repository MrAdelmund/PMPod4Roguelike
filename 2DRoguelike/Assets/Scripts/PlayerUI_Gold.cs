using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI_Gold : MonoBehaviour
{
    private PlayerCurrency currency;
    [SerializeField] private TMP_Text goldText;

    void Start()
    {
        currency = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCurrency>();
    }

    private void Update()
    {
        goldText.text = $"Gold: {currency.gold}";
    }
}
