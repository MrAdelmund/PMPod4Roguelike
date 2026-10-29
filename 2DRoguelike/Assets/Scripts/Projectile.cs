using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage;
    private float speed;
    private float range;
    private Vector2 startPosition;

    public void Initialize(float damage, float speed, float range, Vector2 direction)
    {
        this.damage = damage;
        this.speed = speed;
        this.range = range;
        startPosition = transform.position;

        GetComponent<Rigidbody2D>().velocity = direction.normalized * speed;
    }

    void Update()
    {
        if (Vector2.Distance(startPosition, transform.position) >= range)
        {
            Destroy(gameObject);
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
        }
    }

}
