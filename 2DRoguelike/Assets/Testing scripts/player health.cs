using UnityEngine;

public class Playerhealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Head Stomp Detection")]
    public Transform headPoint;        // Create an empty GameObject at the player's head area
    public float headRadius = 0.3f;    // Size of the detection circle on the player's head
    public LayerMask enemyLayer;       // Set this to your "Enemy" layer in the Inspector
    public int stompDamage = 20;       // How much damage the player takes when stomped on

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void FixedUpdate()
    {
        // Continuously check if an enemy is landing on the player's head
        CheckForBeingStomped();
    }

    /// <summary>
    /// Standard function called by enemies to deal damage to the player.
    /// </summary>
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log($"Player took {damageAmount} damage. Health remaining: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Detects if an enemy is landing on top of the player's head.
    /// </summary>
    private void CheckForBeingStomped()
    {
        if (headPoint == null) return;

        // Check if anything on the Enemy layer is overlapping the player's head zone
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(headPoint.position, headRadius, enemyLayer);

        foreach (Collider2D enemyCollider in hitEnemies)
        {
            Rigidbody2D enemyRb = enemyCollider.GetComponent<Rigidbody2D>();

            // Unity 2022 Physics Check: Only count it as a stomp if the enemy is actively falling down
            if (enemyRb != null && enemyRb.velocity.y < -0.1f)
            {
                Debug.Log($"Player was stomped on by {enemyCollider.gameObject.name}!");

                // Damage the player
                TakeDamage(stompDamage);

                // Make the enemy bounce cleanly off the player's head using Unity 2022 physics standard
                enemyRb.velocity = new Vector2(enemyRb.velocity.x, 8f);

                break; // Exit loop so we don't take multiple hits frame-by-frame from the same enemy
            }
        }
    }

    private void Die()
    {
        Debug.Log("Player has died!");
        gameObject.SetActive(false);
    }

    // Visualizes the head stomp zone in the Scene View (drawn as a cyan circle)
    private void OnDrawGizmosSelected()
    {
        if (headPoint == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(headPoint.position, headRadius);
    }
}
