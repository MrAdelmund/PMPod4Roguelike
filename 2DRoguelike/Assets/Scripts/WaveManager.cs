using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("Wave Settings")]
    public float roundDuration = 30f;
    public int currentWave = 1;

    [Header("References")]
    public ShopManager shopManager;

    private float roundTimer;
    private bool roundActive = true;

    private EnemySpawner spawner;

    void Start()
    {
        spawner = gameObject.GetComponent<EnemySpawner>();
    }

    void Update()
    {
        if (!roundActive) return;

        roundTimer += Time.deltaTime;

        if (roundTimer >= roundDuration && !spawner.boss)
        {
            EndRound();
        }
    }

    public void EndRound()
    {
        roundActive = false;
        spawner.boss = false;

        // Clear all remaining enemies
        foreach (Enemy e in new System.Collections.Generic.List<Enemy>(Enemy.ActiveEnemies))
        {
            if (e != null)
                Destroy(e.gameObject);
        }

        // Pause the game
        Time.timeScale = 0f;

        // Open the shop
        shopManager.OpenShop();
    }

    public void OnShopClosed()
    {
        // Resume next round
        roundTimer = 0f;
        roundActive = true;
        currentWave++;
    }
}
