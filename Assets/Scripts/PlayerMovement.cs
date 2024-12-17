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
    private float movementThreshold = 0.1f;
    private float rotationSpeed = 10f; 

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>(); 

        if (animator == null)
        {
            Debug.LogWarning("Animator component missing.");
        }

        // freeze X and Z 
        rb.freezeRotation = true;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void FixedUpdate()
    {
        // WASD
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

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical).normalized * speed;

        if (movement.magnitude > movementThreshold)
        {
            Vector3 newVelocity = new Vector3(movement.x, rb.velocity.y, movement.z);
            rb.velocity = newVelocity;

            if (animator != null)
            {
                animator.SetBool("isWalking", true);
            }

            // rotation
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
        else
        {
            // stop movement when there is no input
            rb.velocity = new Vector3(0, rb.velocity.y, 0);

            if (animator != null) 
            {
                animator.SetBool("isWalking", false); 
            }
        }
    }

    void Update()
    {
        // jump
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
