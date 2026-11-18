using UnityEngine;

public class EnemyMelee : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange = 1.2f;
    public float attackCooldown = 2f;
    public int attackDamage = 10;

    [Header("References")]
    public Transform attackPoint;
    public LayerMask playerLayer;

    //private Animator animator;
    private Transform player;
    private float cooldownTimer = Mathf.Infinity;

    void Start()
    {
        //animator = GetComponent<Animator>();
        // Find player by tag (make sure your Player GameObject is tagged "Player")
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        cooldownTimer += Time.deltaTime;

        if (player == null) return;

        // Check distance to player
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange && cooldownTimer >= attackCooldown)
        {
            cooldownTimer = 0f;
            //animator.SetTrigger("Attack");
        }
    }

    // This method is called by an Animation Event on the swing frame
    public void DealDamage()
    {
        // Detect player in range of the attack point
        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint.position, attackRange, playerLayer);

        if (hitPlayer != null)
        {
            // Call your player's health/damage method here
            hitPlayer.GetComponent<PlayerHealth>()?.TakeDamage(attackDamage);
            Debug.Log("Hit player for " + attackDamage + " damage!");
        }
    }

    // Draws the attack radius in the Scene view for easy adjustment
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}