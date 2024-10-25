using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlacePotion : MonoBehaviour
{
    public float pickupRange = 2.0f;  // Distance within which player can place potion
    private PlayerPickup playerPickup; // Reference to PlayerPickup script

    void Start()
    {
        // Get the PlayerPickup component from the player GameObject
        playerPickup = GetComponent<PlayerPickup>();
    }

    void Update()
    {
        // Check for "E" key press
        if (Input.GetKeyDown(KeyCode.E) && playerPickup.HeldPotion != null)
        {
            // Try to place the potion on a nearby table
            PlacePotionOnTable();
        }
    }

    void PlacePotionOnTable()
    {
        // Find all colliders within pickupRange
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, pickupRange);

        foreach (var collider in hitColliders)
        {
            // Check if the object has the tag "Table"
            if (collider.CompareTag("Table"))
            {
                Transform potionPlace = collider.transform.Find("PotionPlace");
                if (potionPlace != null)
                {
                    // Debugging logs to see the positions
                    Debug.Log("Placing potion on " + collider.gameObject.name);
                    Debug.Log("PotionPlace position: " + potionPlace.position);
                    Debug.Log("HeldPotion position before placing: " + playerPickup.HeldPotion.transform.position);

                    // Place the held potion on the potionPlace position
                    playerPickup.HeldPotion.transform.position = potionPlace.position;
                    playerPickup.HeldPotion.transform.rotation = potionPlace.rotation;  // Match the rotation

                    // Detach the potion and enable physics again
                    playerPickup.DropPotion(); // Call DropPotion to detach and reset the potion

                    Debug.Log("Potion placed successfully."); // Confirm placement
                    break; // Exit the loop after placing the potion
                }
                else
                {
                    Debug.LogError("PotionPlace not found in table: " + collider.gameObject.name);
                }
            }
        }
    }
}
