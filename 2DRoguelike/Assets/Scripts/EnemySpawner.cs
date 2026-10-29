using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public float spawnRate = 1f;
    public float spawnRadius = 10f;

    [Header("Enemy Tiers")]
    public List<EnemySO> tier1Enemies;
    public List<EnemySO> tier2Enemies;
    public List<EnemySO> tier3Enemies;

    [Header("Bosses")]
    public EnemySO orkBossSO;
    public EnemySO orkBossSO_V2;

    [Header("Wave Scaling")]
    public int currentWave = 1;

    [Tooltip("Wave where tier 2 starts appearing")]
    public int tier2StartWave = 3;

    [Tooltip("Wave where tier 3 starts appearing")]
    public int tier3StartWave = 7;

    [Tooltip("Unfinished")]
    public List<int> bossWaves;

    [Header("References")]
    public GameObject player;
    public Transform spawnPos;

    private float spawnTimer;
    public bool boss = false;

    void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= 1f / spawnRate && !boss)
        {
            SpawnEnemy();
            spawnTimer = 0f;
        }
    }

    void SpawnEnemy()
    {
        EnemySO enemyData = GetEnemyForCurrentWave();

        Vector2 spawnPosition =
            (Vector2)spawnPos.position +
            Random.insideUnitCircle.normalized * spawnRadius;

        GameObject currentPlayer = GameObject.FindGameObjectWithTag("Player");

        if (!currentPlayer) return; //SAFETY NET

        Enemy enemy = Instantiate(
            enemyData.enemyPrefab,
            spawnPosition,
            Quaternion.identity
        );

        enemy.Initialize(enemyData, currentPlayer);
    }

    EnemySO GetEnemyForCurrentWave()
    {
        currentWave = GetComponent<WaveManager>().currentWave;

        if (bossWaves.Contains(currentWave))
        {
            boss = true;
            switch (currentWave)
            {
                case 10:
                    return orkBossSO;

                case 20:
                    return orkBossSO_V2;
            }
        }

        // Determine tier weights based on wave
        float tier1Weight = 1f;
        float tier2Weight = 0f;
        float tier3Weight = 0f;

        if (currentWave >= tier2StartWave)
            tier2Weight = Mathf.Clamp01((currentWave - tier2StartWave + 1) * 0.25f);

        if (currentWave >= tier3StartWave)
            tier3Weight = Mathf.Clamp01((currentWave - tier3StartWave + 1) * 0.2f);

        float totalTierWeight = tier1Weight + tier2Weight + tier3Weight;
        float roll = Random.Range(0f, totalTierWeight);

        if (roll < tier1Weight || tier2Enemies.Count == 0)
            return GetWeightedEnemy(tier1Enemies);

        roll -= tier1Weight;

        if (roll < tier2Weight || tier3Enemies.Count == 0)
            return GetWeightedEnemy(tier2Enemies);

        return GetWeightedEnemy(tier3Enemies);
    }

    EnemySO GetWeightedEnemy(List<EnemySO> pool)
    {
        float totalWeight = 0f;
        foreach (var enemy in pool)
            totalWeight += enemy.spawnWeight;

        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var enemy in pool)
        {
            cumulative += enemy.spawnWeight;
            if (roll <= cumulative)
                return enemy;
        }

        return pool[0];
    }
}
