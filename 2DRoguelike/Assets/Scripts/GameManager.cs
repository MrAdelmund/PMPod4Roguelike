using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerClassSO startingClass;
    public PlayerStats playerStats;
    public WeaponController weaponController;
    public WeaponInventory playerInventory;

    void Start()
    {
        playerStats.Initialize(startingClass);
        // Add the starting weapon to the player's inventory
        playerInventory.AddWeapon(startingClass.startingWeapon);

    }
}
