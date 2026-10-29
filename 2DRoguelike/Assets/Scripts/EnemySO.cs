using UnityEngine;

[CreateAssetMenu(menuName = "Roguelite/Enemy")]
public class EnemySO : ScriptableObject
{
    [Header("Identity")]
    public string enemyName;
    public Sprite sprite;

    [Header("Stats")]
    public float maxHealth = 20f;
    public float moveSpeed = 2f;
    public float damage = 10f;

    [Header("Spawn")]
    public float spawnWeight = 1f;

    [Header("Prefabs")]
    public Enemy enemyPrefab;
    public EnemyBehavior behaviorPrefab;
}
