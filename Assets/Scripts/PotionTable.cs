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
            return;
        }

        SpawnPotion();
    }

    void Update()
    {
        // if potion moved, spawn another
        if (currentPotion != null && currentPotion.transform.parent != tablePotionPlace)
        {
            SpawnPotion(); 
        }
    }

    void SpawnPotion()
    {
        if (potionPrefab == null || tablePotionPlace == null)
        {
            return;
        }

        // spawn new potion and make it current potion
        currentPotion = Instantiate(potionPrefab, tablePotionPlace.position, tablePotionPlace.rotation, tablePotionPlace);
    }
}
