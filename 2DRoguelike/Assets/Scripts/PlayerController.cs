using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerStats stats;

    [Header("Movement Bounds")]
    public Vector2 minBounds;
    public Vector2 maxBounds;

    private Vector2 movementInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<PlayerStats>();
    }

    void Update()
    {
        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");
        movementInput = movementInput.normalized;
    }

    void FixedUpdate()
    {
        Vector2 targetPosition = rb.position + movementInput * stats.moveSpeed * Time.fixedDeltaTime;

        targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y);

        rb.MovePosition(targetPosition);
    }
}
