using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPotionHandler : MonoBehaviour
{
    public float interactionRange = 2.0f;
    public Transform holdPosition;
    private GameObject heldPotion;
    private int originalLayer;
    private bool canPickup = true;         

    public GameObject potionPrefab;
    public GameObject potion_2Prefab;
    public GameObject potion_3Prefab;
    public GameObject potion_4Prefab;
    public GameObject potion_5Prefab;
    public GameObject potion_6Prefab;
    public GameObject potion_7Prefab;
    public GameObject potion_8Prefab;
    public GameObject potion_9Prefab;
    public GameObject potion_10Prefab;
    public GameObject potion_11Prefab;
    public GameObject potion_12Prefab;
    public GameObject potion_13Prefab;
    public GameObject potion_14Prefab;

    public KeyCode interactKey = KeyCode.E; // default E

    private static HashSet<GameObject> takenPotions = new HashSet<GameObject>();

    private Dictionary<(string, string), (GameObject, string)> potionCombinations;

    void Start()
    {
        InitializeCombinations();
    }

    void InitializeCombinations()
    {
        potionCombinations = new Dictionary<(string, string), (GameObject, string)>();

        for (int i = 0; i <= 14; i++)
        {
            for (int j = i + 1; j <= 14; j++)
            {
                string potion1 = i == 1 ? "Potion" : $"Potion_{i}";
                string potion2 = j == 1 ? "Potion" : $"Potion_{j}";

                int resultIndex = (i + j) % 14;
                if (resultIndex == 0) resultIndex = 14; 

                string resultTag = resultIndex == 1 ? "Potion" : $"Potion_{resultIndex}";
                GameObject resultPrefab = GetPotionPrefab(resultIndex);

                potionCombinations[(potion1, potion2)] = (resultPrefab, resultTag);
                potionCombinations[(potion2, potion1)] = (resultPrefab, resultTag);
            }
        }
    }

    GameObject GetPotionPrefab(int index)
    {
        switch (index)
        {
            case 1: return potionPrefab; 
            case 2: return potion_2Prefab;
            case 3: return potion_3Prefab;
            case 4: return potion_4Prefab;
            case 5: return potion_5Prefab;
            case 6: return potion_6Prefab;
            case 7: return potion_7Prefab;
            case 8: return potion_8Prefab;
            case 9: return potion_9Prefab;
            case 10: return potion_10Prefab;
            case 11: return potion_11Prefab;
            case 12: return potion_12Prefab;
            case 13: return potion_13Prefab;
            case 14: return potion_14Prefab;
            default: return null;
        }
    }


    void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            if (heldPotion == null && canPickup)
            {
                TryPickupPotionOrTablePotion();
            }
            else if (heldPotion != null)
            {
                PlacePotionOnTableOrCombine();
            }
        }
    }

    void TryPickupPotionOrTablePotion()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionRange);
        Transform closestPotionPlace = null;
        float closestDistance = Mathf.Infinity;
        GameObject potionOnTable = null;

        foreach (var collider in hitColliders)
        {
            if (collider.CompareTag("Table"))
            {
                Transform potionPlace = collider.transform.Find("PotionPlace");
                if (potionPlace != null)
                {
                    float distance = Vector3.Distance(transform.position, potionPlace.position);

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestPotionPlace = potionPlace;

                        foreach (Transform child in potionPlace)
                        {
                            if ((collider.CompareTag("Potion") || collider.tag.StartsWith("Potion_")) && !takenPotions.Contains(child.gameObject))
                            {
                                potionOnTable = child.gameObject;
                                break;
                            }
                        }
                    }
                }
            }
        }

        if (potionOnTable != null)
        {
            PickUpPotion(potionOnTable);
        }
        else
        {
            TryPickupPotion();
        }
    }

    void TryPickupPotion()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionRange);

        foreach (var collider in hitColliders)
        {
            if ((collider.CompareTag("Potion") || collider.tag.StartsWith("Potion_")) &&
                !takenPotions.Contains(collider.gameObject))
            {
                PickUpPotion(collider.gameObject);
                break;
            }
        }
    }

    private void PickUpPotion(GameObject potion)
    {
        heldPotion = potion;
        heldPotion.transform.position = holdPosition.position;
        heldPotion.transform.SetParent(holdPosition, true);

        originalLayer = heldPotion.layer;
        heldPotion.layer = LayerMask.NameToLayer("IgnorePickup");

        // disable physics
        Rigidbody potionRb = heldPotion.GetComponent<Rigidbody>();
        if (potionRb != null)
        {
            potionRb.isKinematic = true;
        }

        canPickup = false; // prevent another pickup
        StartCoroutine(ResetPickupCooldown());
    }

    private IEnumerator ResetPickupCooldown()
    {
        yield return new WaitForSeconds(1f);
        canPickup = true;
    }

    void PlacePotionOnTableOrCombine()
    {
        // check if there is a nearby NPC with an active order
        NPCBehavior npc = FindNearbyNPCWithActiveOrder();

        if (npc != null && npc.CheckIfOrderCompleted(heldPotion))
        {
            Destroy(heldPotion);
            heldPotion = null;
            return; // order completed
        }

        // check if player has spoiled potion and if so, check if cauldron is nearby
        if (IsCauldronNearby())
        {
            Destroy(heldPotion);
            heldPotion = null;
            return;
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionRange);
        Transform closestPotionPlace = null;
        float closestDistance = Mathf.Infinity;

        bool tableFound = false;
        GameObject potionOnTable = null;

        foreach (var collider in hitColliders)
        {
            if (collider.CompareTag("Table"))
            {
                Transform potionPlace = collider.transform.Find("PotionPlace");
                if (potionPlace != null)
                {
                    float distance = Vector3.Distance(transform.position, potionPlace.position);

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestPotionPlace = potionPlace;
                        tableFound = true;

                        foreach (Transform child in potionPlace)
                        {
                            if (child.CompareTag("Potion") || child.tag.StartsWith("Potion_"))
                            {
                                potionOnTable = child.gameObject;
                                break;
                            }
                        }
                    }
                }
            }
        }

        if (tableFound)
        {
            if (potionOnTable != null)
            {
                CombinePotions(potionOnTable, closestPotionPlace);
            }
            else if (closestPotionPlace != null)
            {
                PlacePotionOnTable(closestPotionPlace);
            }
        }
        else
        {
            DropPotion(0);
        }
    }

    void CombinePotions(GameObject potionOnTable, Transform potionPlace)
    {
        string heldTag = heldPotion.tag;
        string tableTag = potionOnTable.tag;

        if (potionCombinations.TryGetValue((heldTag, tableTag), out var result))
        {
            var (potionPrefab, newTag) = result;

            GameObject newPotion = Instantiate(potionPrefab, potionPlace.position, Quaternion.identity);
            newPotion.tag = newTag;

            Destroy(heldPotion);
            Destroy(potionOnTable);

            newPotion.transform.position = potionPlace.position;
            newPotion.transform.rotation = potionPlace.rotation;
            newPotion.transform.SetParent(potionPlace, true);
        }
        else
        {
            DropPotion(0);
        }
    }

    void PlacePotionOnTable(Transform closestPotionPlace)
    {
        heldPotion.transform.position = closestPotionPlace.position;
        heldPotion.transform.rotation = closestPotionPlace.rotation;

        heldPotion.transform.SetParent(closestPotionPlace, true);

        Rigidbody potionRb = heldPotion.GetComponent<Rigidbody>();
        if (potionRb != null)
        {
            potionRb.isKinematic = false;
        }

        DropPotion(1);
    }

    void DropPotion(int dontResetParent)
    {
        if (heldPotion != null)
        {
            if (dontResetParent == 0)
            {
                heldPotion.transform.parent = null;
            }
            Rigidbody potionRb = heldPotion.GetComponent<Rigidbody>();
            if (potionRb != null)
            {
                potionRb.isKinematic = false;
            }

            heldPotion.layer = originalLayer;

            takenPotions.Remove(heldPotion); // mark potion not taken
            heldPotion = null;
        }
    }

    NPCBehavior FindNearbyNPCWithActiveOrder()
    {
        NPCBehavior[] npcs = FindObjectsOfType<NPCBehavior>();

        foreach (var npc in npcs)
        {
            // calculate distance to see if nearby
            float distanceToNPC = Vector3.Distance(transform.position, npc.transform.position);

            if (npc != null && npc.OrderPlaced && distanceToNPC <= interactionRange)
            {
                return npc;  
            }
        }

        return null; 
    }

    bool IsCauldronNearby ()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionRange);
        foreach (var collider in hitColliders)
        {
            if (collider.CompareTag("Cauldron"))
            {
                return true;
            }
        }
        return false;
    }





    //void CombinePotions(GameObject potionOnTable, Transform potionPlace)
    //{
    //    string heldTag = heldPotion.tag;
    //    string tableTag = potionOnTable.tag;
    //    GameObject newPotion = null;

    //    if ((string.Equals(heldTag, "Potion") && string.Equals(tableTag, "Potion_2")) ||
    //        (string.Equals(heldTag, "Potion_2") && string.Equals(tableTag, "Potion")))
    //    {
    //        newPotion = Instantiate(potion_4Prefab, potionPlace.position, Quaternion.identity);
    //        newPotion.tag = "Potion_4";
    //    }
    //    else if ((string.Equals(heldTag, "Potion") && string.Equals(tableTag, "Potion_3")) ||
    //             (string.Equals(heldTag, "Potion_3") && string.Equals(tableTag, "Potion")))
    //    {
    //        newPotion = Instantiate(potion_5Prefab, potionPlace.position, Quaternion.identity);
    //        newPotion.tag = "Potion_5";
    //    }
    //    else if ((string.Equals(heldTag, "Potion_2") && string.Equals(tableTag, "Potion_3")) ||
    //             (string.Equals(heldTag, "Potion_3") && string.Equals(tableTag, "Potion_2")))
    //    {
    //        newPotion = Instantiate(potion_6Prefab, potionPlace.position, Quaternion.identity);
    //        newPotion.tag = "Potion_6";
    //    }
    //    else if ((string.Equals(heldTag, "Potion_4") && string.Equals(tableTag, "Potion_6")) ||
    //             (string.Equals(heldTag, "Potion_6") && string.Equals(tableTag, "Potion_4")))
    //    {
    //        newPotion = Instantiate(potion_7Prefab, potionPlace.position, Quaternion.identity);
    //        newPotion.tag = "Potion_7";
    //    }
    //    else
    //    {
    //        Debug.Log("No valid potion combination found. Throwing the potion.");
    //        DropPotion(0);
    //        return;
    //    }

    //    Destroy(heldPotion);
    //    Destroy(potionOnTable);

    //    newPotion.transform.position = potionPlace.position;
    //    newPotion.transform.rotation = potionPlace.rotation;
    //    newPotion.transform.SetParent(potionPlace, true);
    //}
}
