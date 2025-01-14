using UnityEngine;

public class ResolutionManager : MonoBehaviour
{
    private const string ResolutionKey = "ResolutionIndex";  

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Debug.Log("ResolutionManager Start called");
        ApplySavedResolution();
    }

    public void ApplySavedResolution()
    {
        int savedResolutionIndex = PlayerPrefs.GetInt(ResolutionKey, -1);
        Debug.Log($"Saved Resolution Index: {PlayerPrefs.GetInt("ResolutionIndex", -1)}");
        if (savedResolutionIndex != -1)
        {
            Resolution[] resolutions = Screen.resolutions;

            if (savedResolutionIndex >= 0 && savedResolutionIndex < resolutions.Length)
            {
                Resolution resolution = resolutions[savedResolutionIndex];
                Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode, resolution.refreshRateRatio);

                Debug.Log($"Applied saved resolution: {resolution.width} x {resolution.height} @ {resolution.refreshRateRatio.numerator}/{resolution.refreshRateRatio.denominator}Hz");
            }
        }
        else
        {
            Debug.LogWarning("No saved resolution found. Using default resolution.");
        }
    }
}
