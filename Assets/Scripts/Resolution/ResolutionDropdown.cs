using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

public class ResolutionDropdown : MonoBehaviour
{
    public Dropdown resolutionDropdown;
    private Resolution[] uniqueResolutions; 
    private const string ResolutionKey = "ResolutionIndex";

    private void Start()
    {
        // dublicates
        uniqueResolutions = Screen.resolutions
            .GroupBy(r => new { r.width, r.height })
            .Select(g => g.First())
            .ToArray();

        resolutionDropdown.ClearOptions();

        var options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < uniqueResolutions.Length; i++)
        {
            Resolution res = uniqueResolutions[i];
            string resolutionOption = $"{res.width} x {res.height}";

            options.Add(resolutionOption);

            if (res.width == Screen.currentResolution.width &&
                res.height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);

        int savedResolutionIndex = PlayerPrefs.GetInt(ResolutionKey, currentResolutionIndex);
        resolutionDropdown.value = savedResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        SetResolution(savedResolutionIndex);

        resolutionDropdown.onValueChanged.AddListener(SetResolution); 
    }

    public void SetResolution(int resolutionIndex)
    {
        if (resolutionIndex < 0 || resolutionIndex >= uniqueResolutions.Length)
        {
            Debug.LogWarning("Resolution index out of bounds");
            return;
        }

        Resolution resolution = uniqueResolutions[resolutionIndex];

        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode, resolution.refreshRate);

        Debug.Log($"Applying resolution: {resolution.width} x {resolution.height} @ {resolution.refreshRate}Hz");

        PlayerPrefs.SetInt(ResolutionKey, resolutionIndex);
        PlayerPrefs.Save();
    }
}
