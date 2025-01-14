using UnityEngine;
using UnityEngine.UI;

public class FullscreenSettings : MonoBehaviour
{
    [SerializeField] private Toggle fullscreenToggle;

    private const string FullscreenKey = "Fullscreen";

    private void Start()
    {
        bool isFullscreen = PlayerPrefs.GetInt(FullscreenKey, 1) == 1;
        Screen.fullScreen = isFullscreen;
        fullscreenToggle.isOn = isFullscreen;

        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;

        PlayerPrefs.SetInt(FullscreenKey, isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }
}
