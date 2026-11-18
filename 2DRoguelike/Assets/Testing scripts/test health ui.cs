using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class testhealthui : MonoBehaviour
{
    private PlayerStats stats;
    [SerializeField] private TMP_Text healthText;

    void Start()
    {
        stats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
    }

    private void Update()
    {
        healthText.text = $"{stats.currentHealth} / {stats.maxHealth}";
    }
}
