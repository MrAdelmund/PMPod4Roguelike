using System.Collections;
using UnityEngine;

public class EnemyStompAttack : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private Transform player;
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float attackCooldown = 3f;

    [Header("Stomp Phases")]
    [SerializeField] private float windUpTime = 0.5f;   // Time spent hovering/shaking before slam
    [SerializeField] private float jumpHeight = 3f;     // How high it rises before slamming
    [SerializeField] private float slamSpeed = 25f;     // How fast it crashes down

    private Rigidbody2D rb;
    private bool isAttacking = false;
    private float cooldownTimer = 0f;
    private Vector2 originalPosition;

    enum StompState { Idle, WindUp, Slamming, Recovering }
    private StompState currentState = StompState.Idle;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }
    }

    void Update()
    {
        if (cooldownTimer > 0) cooldownTimer -= Time.deltaTime;

        // Check if player is close enough to trigger the stomp
        if (!isAttacking && cooldownTimer <= 0 && player != null)
        {
            if (Vector2.Distance(transform.position, player.position) <= attackRange)
            {
                StartCoroutine(PerformStomp());
            }
        }
    }

    private IEnumerator PerformStomp()
    {
        isAttacking = true;
        rb.velocity = Vector2.zero;

        // --- PHASE 1: WIND UP (Rise and Hover) ---
        currentState = StompState.WindUp;
        rb.isKinematic = true; // Stop gravity during wind-up

        Vector2 targetWindUpPos = new Vector2(player.position.x, transform.position.y + jumpHeight);
        float elapsed = 0f;
        Vector2 startPos = transform.position;

        // Move above the player's current X position
        while (elapsed < windUpTime)
        {
            transform.position = Vector2.Lerp(startPos, targetWindUpPos, elapsed / windUpTime);
            // Visual feedback: slight shake effect
            transform.position += new Vector3(Random.Range(-0.05f, 0.05f), 0, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // --- PHASE 2: THE SLAM ---
        currentState = StompState.Slamming;
        rb.isKinematic = false;

        // Apply a massive downward force for the smash
        rb.velocity = Vector2.down * slamSpeed;

        // Wait until the enemy hits something or stops moving downward
        while (rb.velocity.y < -0.1f)
        {
            yield return null;
        }

        // --- PHASE 3: RECOVERY ---
        currentState = StompState.Recovering;
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(0.5f); // Stunned briefly on the ground

        // Reset
        cooldownTimer = attackCooldown;
        isAttacking = false;
        currentState = StompState.Idle;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Only deal damage if we hit the player while slamming down
        if (currentState == StompState.Slamming && collision.gameObject.CompareTag("Player"))
        {
            // Try to damage the player
            Debug.Log("Player Stomped!");

            // Example of pushing the player away or dealing damage:
            // collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(1);
        }
    }

    // Visualizes the attack trigger radius in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
