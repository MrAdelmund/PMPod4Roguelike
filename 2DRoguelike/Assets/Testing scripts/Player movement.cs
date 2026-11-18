using JetBrains.Annotations;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    [SerializeField] private float moveSpeed = 5f; // Adjustable speed in Inspector

    private Rigidbody2D rb;
    private Vector2 movement;

    private void Start()
    {
        // Get the Rigidbody2D component attached to the player
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // 1. Gather player input (WASD or Arrow keys)
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // 2. Normalize the vector to prevent faster diagonal movement
        movement = movement.normalized;
    }

    private void FixedUpdate()
    {
        // 3. Move the player using physics (runs on a fixed timer)
        rb.velocity = movement * moveSpeed;
    }
    
}
