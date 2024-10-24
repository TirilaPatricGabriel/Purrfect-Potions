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
    }

    void Update()
    {
        // Input - horizontal & vertical
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Movement vector
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        // Modify sphere position using movement
        transform.Translate(movement * speed * Time.deltaTime, Space.World);

        // Check if space pressed and player grounded
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
