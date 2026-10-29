using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    public static List<Enemy> ActiveEnemies = new List<Enemy>();

    private EnemySO data;
    private EnemyBehavior behavior;
    public float currentHealth;
    private GameObject player;
    private Rigidbody2D rb;

    [Header("XP")]
    public GameObject xpOrbPrefab;
    public int xpValue = 1;
    public int goldValue = 1;
    public GameObject goldOrbPrefab;

    [HideInInspector] public GameObject GetPlayerObject() => player;
    [HideInInspector] public Rigidbody2D GetRigidbody() => rb;
    [HideInInspector] public EnemySO GetData() => data;

    public void Initialize(EnemySO enemyData, GameObject playerTransform)
    {
        data = enemyData;
        currentHealth = data.maxHealth;
        player = playerTransform;

        GetComponent<SpriteRenderer>().sprite = data.sprite;

        // Attach behavior if it exists
        if (data.behaviorPrefab != null)
        {
            behavior = Instantiate(data.behaviorPrefab, transform);
            behavior.Initialize(this);
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        ActiveEnemies.Add(this);
    }

    void OnDestroy()
    {
        ActiveEnemies.Remove(this);
    }

    void FixedUpdate()
    {
        if (behavior != null)
        {
            behavior.Tick();
            return;
        }
        if (!player)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        DefaultMovement();
    }

    void DefaultMovement()
    {
        if (player == null) return;

        Vector2 direction = (player.transform.position - transform.position).normalized;
        rb.MovePosition(rb.position + direction * data.moveSpeed * Time.fixedDeltaTime);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0f)
            Die();
    }

    void Die()
    {
        if (xpOrbPrefab != null)
        {
            Instantiate(
                xpOrbPrefab,
                (transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0)),
                Quaternion.identity
            );
        }
        // Drop Gold
        if (goldOrbPrefab != null)
            Instantiate(
                goldOrbPrefab, 
                (transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0)), 
                Quaternion.identity);

        if (behavior != null)
        {
            WaveManager wave = GameObject.FindGameObjectWithTag("GameManager").GetComponent<WaveManager>();

            if (wave != null)
            {
                wave.EndRound();
            }
        }

        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerStats player = other.GetComponent<PlayerStats>();

        if (player != null && behavior == null)
        {
            player.TakeDamage(data.damage);
        }
    }
}
