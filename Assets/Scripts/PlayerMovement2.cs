using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement2 : MonoBehaviour
{
    public float movementSpeed = 20f;
    public float jumpPower = 10f;
    private Rigidbody rigidBody;
    private bool onGround;
    private Animator animator; // Reference to the Animator
    private float movementThreshold = 0.1f; // Minimum movement to trigger walking animation
    private float rotationSpeed = 10f; // Speed of rotation to match the movement direction

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>(); // Get Animator component from child (if exists)

        // Verify if Animator component exists
        if (animator == null)
        {
            Debug.LogWarning("Animator component is missing on Player or its children.");
        }

        // Freeze rotation to keep player upright
        rigidBody.freezeRotation = true;
        rigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void FixedUpdate()
    {
        // Input for arrow keys
        float horizontalMovement = 0f;
        float verticalMovement = 0f;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            verticalMovement = 1f;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            verticalMovement = -1f;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            horizontalMovement = -1f;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            horizontalMovement = 1f;
        }

        // Movement vector
        Vector3 moveDirection = new Vector3(horizontalMovement, 0.0f, verticalMovement).normalized * movementSpeed;

        // Move player
        if (moveDirection.magnitude > movementThreshold)
        {
            Vector3 newVelocity = new Vector3(moveDirection.x, rigidBody.velocity.y, moveDirection.z);
            rigidBody.velocity = newVelocity;

            if (animator != null) // Check if animator exists before using it
            {
                animator.SetBool("isWalking", true); // Trigger walking animation
            }

            // Rotate player to face the movement direction
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
        else
        {
            // Stop movement when there is no input
            rigidBody.velocity = new Vector3(0, rigidBody.velocity.y, 0);

            if (animator != null) // Check if animator exists before using it
            {
                animator.SetBool("isWalking", false); // Trigger idle animation when not moving
            }
        }
    }

    void Update()
    {
        // Jumping logic
        if (Input.GetKeyDown(KeyCode.Space) && onGround)
        {
            rigidBody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            onGround = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            onGround = true; // Player is back on the ground
        }
    }
}
