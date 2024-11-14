using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 30f;
    public float jumpForce = 12f;
    private Rigidbody rb;
    private Animator animator;
    private bool isGrounded;
    private float movementThreshold = 0.1f; // Minimum movement to trigger walking
    private float rotationSpeed = 10f; // Speed of rotation to match the movement direction

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>(); // Use GetComponentInChildren to search in children as well

        // Verify if Animator component exists
        if (animator == null)
        {
            Debug.LogWarning("Animator component is missing on Player or its children.");
        }

        // Freeze X and Z - keep player upright
        rb.freezeRotation = true;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void FixedUpdate()
    {
        // Input for WASD only
        float moveHorizontal = 0f;
        float moveVertical = 0f;

        if (Input.GetKey(KeyCode.W))
        {
            moveVertical = 1f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            moveVertical = -1f;
        }

        if (Input.GetKey(KeyCode.A))
        {
            moveHorizontal = -1f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            moveHorizontal = 1f;
        }

        // Movement vector
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical).normalized * speed;

        // Set velocity directly for precise movement without sliding
        if (movement.magnitude > movementThreshold)
        {
            Vector3 newVelocity = new Vector3(movement.x, rb.velocity.y, movement.z);
            rb.velocity = newVelocity;

            if (animator != null) // Check if animator exists before using it
            {
                animator.SetBool("isWalking", true); // Only set to walking if actually moving
            }

            // Rotate the player to face the direction of movement
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
        else
        {
            // Stop movement when there is no input
            rb.velocity = new Vector3(0, rb.velocity.y, 0);

            if (animator != null) // Check if animator exists before using it
            {
                animator.SetBool("isWalking", false); // Set to idle when not moving
            }
        }
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
