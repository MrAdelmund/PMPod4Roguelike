using UnityEngine;

public class EnemyFollow2D : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float speed = 3.0f;
    [SerializeField] private float stoppingDistance = 0.5f;

    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Move towards player if outside stopping distance
        if (distanceToPlayer > stoppingDistance)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                player.position,
                speed * Time.deltaTime
            );
        }
    }
}
