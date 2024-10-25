using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public float pickupRange = 2.0f;  // Distance within which player can pick up potion
    public Transform holdPosition;    // Position to hold the potion (set in Inspector)
    private GameObject heldPotion;    // Reference to the potion being held
    private int originalLayer; // Store the original layer of the potion

    void Start()
    {

    }

    void Update()
    {
        // Check for "E" key press
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldPotion == null)
            {
                // Try to pick up a potion
                TryPickupPotion();
            }
            else
            {
                // Drop the potion
                DropPotion();
            }
        }
    }

    void TryPickupPotion()
    {
        // Find all colliders within pickupRange
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, pickupRange);

        foreach (var collider in hitColliders)
        {
            // Check if the object has the tag "potion"
            if (collider.CompareTag("Potion"))
            {
                // Pick up the potion
                heldPotion = collider.gameObject;
                heldPotion.transform.position = holdPosition.position;
                heldPotion.transform.parent = holdPosition;  // Attach to hold position

                // Store the original layer and set to IgnorePickup layer
                originalLayer = heldPotion.layer;
                heldPotion.layer = LayerMask.NameToLayer("IgnorePickup");

                // Disable potion's physics by making Rigidbody kinematic
                Rigidbody potionRb = heldPotion.GetComponent<Rigidbody>();
                if (potionRb != null)
                {
                    potionRb.isKinematic = true;
                }

                break;
            }
        }
    }

    void DropPotion()
    {
        if (heldPotion != null)
        {
            // Detach the potion and enable physics again
            heldPotion.transform.parent = null;
            Rigidbody potionRb = heldPotion.GetComponent<Rigidbody>();
            if (potionRb != null)
            {
                potionRb.isKinematic = false;  // Enable physics
            }

            // Reset the layer back to original
            heldPotion.layer = originalLayer;

            heldPotion = null;  // Clear reference
        }
    }
}
