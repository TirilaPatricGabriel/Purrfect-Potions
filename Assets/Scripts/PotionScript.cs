using UnityEngine;

public class PotionScript : MonoBehaviour
{
    [SerializeField]
    private float decayTime = 10.0f; 

    [SerializeField]
    private GameObject spoiledPotionPrefab; 

    private float currentDecayTime;
    private bool isTimerActive = false;

    void Start()
    {
        
        currentDecayTime = decayTime;
        isTimerActive = true;
    }

    void Update()
    {
        if (isTimerActive)
        {
            currentDecayTime -= Time.deltaTime; 

            if (currentDecayTime <= 0)
            {
                TransformToSpoiledPotion();
            }
        }
    }

    public void StartDecay()
    {
        isTimerActive = true;
    }

    private void TransformToSpoiledPotion()
    {
        if (spoiledPotionPrefab != null)
        {
            Instantiate(spoiledPotionPrefab, transform.position, transform.rotation);
        }

        Destroy(gameObject); 
    }
}
