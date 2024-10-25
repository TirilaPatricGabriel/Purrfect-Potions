using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public float pickupRange = 2.0f;  // Distance within which player can pick up potion
    public Transform holdPosition;    // Position to hold the potion (set in Inspector)
    private GameObject heldPotion;    // Reference to the potion being held
    private int originalLayer; // Store the original layer of the potion
    private bool canPickup = true; // To manage pickup timing

    public GameObject HeldPotion => heldPotion;  // Public property to access heldPotion

    void Update()
    {
        // Check for "E" key press
        if (Input.GetKeyDown(KeyCode.E) && canPickup)
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
            // Check if the object has the tag "Potion"
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

                Debug.Log("Picked up potion: " + heldPotion.name);
                break;
            }
        }
    }

    public void DropPotion()
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

            Debug.Log("Dropped potion: " + heldPotion.name);
            heldPotion = null;  // Clear reference
            StartCoroutine(PickupCooldown()); // Start the cooldown
        }
    }

    private IEnumerator PickupCooldown()
    {
        canPickup = false; // Prevent picking up immediately
        yield return new WaitForSeconds(0.5f); // Adjust time as needed
        canPickup = true; // Allow pickup again
    }
}
