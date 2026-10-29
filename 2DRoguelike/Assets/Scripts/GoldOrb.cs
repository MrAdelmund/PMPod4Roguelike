using UnityEngine;

public class GoldOrb : MonoBehaviour
{
    public int goldAmount = 1;

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerCurrency currency = other.GetComponent<PlayerCurrency>();
        if (currency != null)
        {
            currency.AddGold(goldAmount);
            Destroy(gameObject);
        }
    }
}
