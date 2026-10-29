using System.Collections.ObjectModel;
using UnityEngine;

public class BossAxe : MonoBehaviour
{
    public float speed = 10f;
    public float airtime = 1.5f;
    public float lifetime = 5f;

    public Vector2 minBounds;
    public Vector2 maxBounds;
    private bool oob;

    private Animator anim;
    private Rigidbody2D rb;

    private Vector2 direction;
    private float timer;

    public void Initialize(Vector2 dir)
    {
        direction = dir.normalized;

        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        rb.velocity = direction * speed; // key change
        Debug.Log("DIR: " + direction + " | VELOCITY: " + rb.velocity);
        oob = false;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (!(rb.position.x >= Mathf.Min(minBounds.x, maxBounds.x) && rb.position.x <= Mathf.Max(minBounds.x, maxBounds.x)
            && rb.position.y >= Mathf.Min(minBounds.y, maxBounds.y) && rb.position.y <= Mathf.Max(minBounds.y, maxBounds.y)))
        {
            oob = true;
            EmbedAxe();
        }

        if (timer >= lifetime)
            Destroy(gameObject);
    }

    public Vector2 GetPosition()
    {
        return rb.position;
    }

    public void EmbedAxe()
    {
        if (timer >= airtime || oob)
        {
            anim.SetBool("embed", true);

            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic; // stops physics movement
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Boss")) EmbedAxe();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player")) collision.gameObject.GetComponent<PlayerStats>().TakeDamage(25);

    }
}