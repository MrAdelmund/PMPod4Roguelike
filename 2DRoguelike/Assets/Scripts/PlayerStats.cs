using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public PlayerClassSO playerClass;

    [Header("Runtime Stats (Read Only)")]
    public float currentHealth;
    public float maxHealth;
    public float moveSpeed;
    public float damageMultiplier;
    public float regenInterval;
    public float regenAmount;

    public float regenTimer;
    private bool dead = false;
    public void Initialize(PlayerClassSO newClass)
    {
        playerClass = newClass;

        maxHealth = playerClass.maxHealth;
        currentHealth = maxHealth;
        moveSpeed = playerClass.moveSpeed;
        damageMultiplier = playerClass.damageMultiplier;
        regenInterval = playerClass.regenInterval;
        regenAmount = playerClass.regenAmount;

        regenTimer = 0;
    }
    public void ApplyUpgrade(UpgradeSO upgrade)
    {
        switch (upgrade.statType)
        {
            case PlayerStatType.MaxHealth:
                maxHealth += upgrade.flatBonus;
                currentHealth += upgrade.flatBonus;

                float prevMaxHealth = maxHealth;
                maxHealth *= (1 + upgrade.percentBonus);
                currentHealth += maxHealth - prevMaxHealth;

                maxHealth = Mathf.Round(maxHealth * 10f) / 10f;
                currentHealth = Mathf.Min(currentHealth, maxHealth);
                break;

            case PlayerStatType.MoveSpeed:
                moveSpeed += upgrade.flatBonus;
                moveSpeed *= (1 + upgrade.percentBonus);
                moveSpeed = Mathf.Round(moveSpeed * 10f) / 10f;
                break;

            case PlayerStatType.DamageMultiplier:
                damageMultiplier += upgrade.flatBonus;
                damageMultiplier *= (1 + upgrade.percentBonus);
                break;

            case PlayerStatType.HealthRegen:
                regenAmount += upgrade.flatBonus;
                regenAmount *= (1 + upgrade.percentBonus);
                break;
        }
    }

    private void Update()
    {
        if (!dead) RegenHealth();
    }

   private void RegenHealth()
    {
        regenTimer += Time.deltaTime;

        if (regenTimer >= regenInterval && currentHealth < maxHealth && regenAmount != 0)
        {
            currentHealth += regenAmount;
            currentHealth = Mathf.Round(currentHealth * 10f) / 10f;

            if (currentHealth > maxHealth) { currentHealth = maxHealth; }

            regenTimer = 0;
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0f)
        {
            moveSpeed = 0f;
            dead = true;
        }
    }
}
