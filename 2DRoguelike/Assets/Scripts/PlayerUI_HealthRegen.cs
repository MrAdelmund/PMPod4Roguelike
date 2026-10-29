using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class PlayerUI_HealthRegen : MonoBehaviour
{
    private PlayerStats stats;
    [SerializeField] private Image regenUsedImage;
    [SerializeField] private Image cooldownImage;

    void Start()
    {
        stats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
    }

    private void Update()
    {
        if (stats.regenTimer >= stats.regenInterval)
        {
            regenUsedImage.enabled = false;
            cooldownImage.enabled = false;
        }
        else
        {
            regenUsedImage.enabled = true;
            cooldownImage.enabled = true;

            cooldownImage.fillAmount = (stats.regenInterval - stats.regenTimer) / stats.regenInterval;
        }
    }
}
