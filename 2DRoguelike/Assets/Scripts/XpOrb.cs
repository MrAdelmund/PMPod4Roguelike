using UnityEngine;

public class XpOrb : MonoBehaviour
{
    public int xpAmount = 1;

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerXP playerXP = other.GetComponent<PlayerXP>();
        if (playerXP != null)
        {
            playerXP.GainXP(xpAmount);
            Destroy(gameObject);
        }
    }
}
