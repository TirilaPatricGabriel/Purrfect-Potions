using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 20f;  // speed
    public float jumpForce = 12f;  // Force applied for jumping
    private Rigidbody rb;  // Reference to the Rigidbody component
    private bool isGrounded;  // Check if the player is on the ground

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Freeze X and Z rotation to keep the player upright
        rb.freezeRotation = true;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    // Use FixedUpdate for physics-based movement
    void FixedUpdate()
    {
        // Input - horizontal & vertical
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Movement vector
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        // Apply force-based movement (physics simulation)
        rb.AddForce(movement * speed);
    }

    void Update()
    {
        // Check if space is pressed and player is grounded (jumping)
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true; // back on ground
        }
    }
}
