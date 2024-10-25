using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 20f; 
    public float jumpForce = 12f;  
    private Rigidbody rb;  
    private bool isGrounded;  

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Freeze X and Z - player upright
        rb.freezeRotation = true;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    // Physic movement
    void FixedUpdate()
    {
        // Input - horizontal & vertical
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Movement vector
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        // Movement
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
