using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement2 : MonoBehaviour
{
    public float movementSpeed = 20f;
    public float jumpPower = 10f;
    private Rigidbody rigidBody;
    private bool onGround;
    private Animator animator;
    private float movementThreshold = 0.1f;
    private float rotationSpeed = 10f; 

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

        if (animator == null)
        {
            Debug.LogWarning("Animator component is missing on Player or its children.");
        }

        rigidBody.freezeRotation = true;
        rigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void FixedUpdate()
    {
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

        Vector3 moveDirection = new Vector3(horizontalMovement, 0.0f, verticalMovement).normalized * movementSpeed;

        if (moveDirection.magnitude > movementThreshold)
        {
            Vector3 newVelocity = new Vector3(moveDirection.x, rigidBody.velocity.y, moveDirection.z);
            rigidBody.velocity = newVelocity;

            if (animator != null)
            {
                animator.SetBool("isWalking", true);
            }

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
        else
        {
            rigidBody.velocity = new Vector3(0, rigidBody.velocity.y, 0);

            if (animator != null) 
            {
                animator.SetBool("isWalking", false);
            }
        }
    }

    void Update()
    {
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
            onGround = true; 
        }
    }
}
