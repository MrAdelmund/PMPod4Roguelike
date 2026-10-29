using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI_XP : MonoBehaviour
{
    private PlayerXP xp;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private Image xpImage;

    void Start()
    {
        xp = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerXP>();
    }

    private void Update()
    {
        levelText.text = $"{xp.level}";
        if (xp.currentXP == 0) xpImage.fillAmount = 0;
        else
        {
            float fillAmount = (float)xp.currentXP / (float)xp.xpToNextLevel;
            xpImage.fillAmount = fillAmount;
        }
    }
}
