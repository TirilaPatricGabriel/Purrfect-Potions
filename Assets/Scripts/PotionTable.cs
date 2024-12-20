using UnityEngine;

public class PotionTable : MonoBehaviour
{
    public GameObject potionPrefab; // potion to spawn
    private Transform tablePotionPlace; 
    private GameObject currentPotion; // currently spawned potion

    void Start()
    {
        // find TablePotionPlace child
        foreach (Transform child in transform)
        {
            if (child.CompareTag("TablePotionPlace"))
            {
                tablePotionPlace = child;
                break;
            }
        }

        if (tablePotionPlace == null)
        {
            Debug.LogError("TablePotionPlace not found in " + gameObject.name);
            return;
        }

        SpawnPotion();
    }

    void Update()
    {
        // if potion moved, spawn another
        if (currentPotion != null && currentPotion.transform.parent != tablePotionPlace)
        {
            Debug.Log("Potion picked up. Spawning a new one.");
            SpawnPotion(); 
        }
    }

    void SpawnPotion()
    {
        if (potionPrefab == null || tablePotionPlace == null)
        {
            Debug.LogError("PotionPrefab or TablePotionPlace not set.");
            return;
        }

        // spawn new potion and make it current potion
        currentPotion = Instantiate(potionPrefab, tablePotionPlace.position, tablePotionPlace.rotation, tablePotionPlace);
        Debug.Log($"Spawned potion {currentPotion.name} at {tablePotionPlace.position}");
    }
}
