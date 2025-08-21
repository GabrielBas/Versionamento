using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ResolutionMenuManager : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;

    private Resolution[] resolutions;

    void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        var options = new System.Collections.Generic.List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        fullscreenToggle.isOn = Screen.fullScreen;

        // força o scroll quando abre
        resolutionDropdown.onValueChanged.AddListener(SetDropdownScroll);
    }

    void SetDropdownScroll(int index)
    {
        // quando abrir, pega o ScrollRect da lista
        var scroll = resolutionDropdown.transform.Find("Dropdown List/Viewport/Content")
                                                ?.GetComponentInParent<ScrollRect>();
        if (scroll != null)
        {
            float normalizedPos = 1f - ((float)index / (resolutions.Length - 1));
            scroll.verticalNormalizedPosition = normalizedPos;
        }
    }
}
