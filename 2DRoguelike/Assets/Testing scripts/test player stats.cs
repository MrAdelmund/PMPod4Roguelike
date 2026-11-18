using UnityEngine;

public class testplayerstats : MonoBehaviour
{
    public PlayerClassSO playerClass;

    [Header("Runtime Stats (Read Only)")]
    public float currentHealth;
    public float maxHealth;
    public float moveSpeed;

  
  

    
   
    public void Initialize(PlayerClassSO newClass)
    {
        playerClass = newClass;

        maxHealth = playerClass.maxHealth;
        currentHealth = maxHealth;
       
        
        
        

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

           

            

            
        }
    }

   

    

  
}