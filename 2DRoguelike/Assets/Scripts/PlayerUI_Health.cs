using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI_Health : MonoBehaviour
{
    private PlayerStats stats;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Image healthImage;

    void Start()
    {
        stats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
    }

    private void Update()
    {
        healthText.text = $"{stats.currentHealth} / {stats.maxHealth}";
        healthImage.fillAmount = stats.currentHealth / stats.maxHealth;
    }
}
