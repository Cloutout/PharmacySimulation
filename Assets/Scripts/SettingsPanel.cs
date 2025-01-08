using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SettingsPanel : MonoBehaviour
{
    [Header("Settings UI Elements")]
    public Slider volumeSlider;
    public Dropdown resolutionDropdown;
    public Dropdown qualityDropdown;
    public Button closeButton;

    private Resolution[] _resolutions;
    private PauseMenu _pauseMenu;

    private void Start()
    {
        
        _resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < _resolutions.Length; i++)
        {
            string option = _resolutions[i].width + " x " + _resolutions[i].height;
            options.Add(option);

            if (_resolutions[i].width == Screen.currentResolution.width &&
                _resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = SaveManager.Instance.resolutionIndex;
        resolutionDropdown.RefreshShownValue();

        
        qualityDropdown.ClearOptions();
        List<string> qualityOptions = new List<string>();
        foreach (var name in QualitySettings.names)
        {
            qualityOptions.Add(name);
        }
        qualityDropdown.AddOptions(qualityOptions);
        qualityDropdown.value = SaveManager.Instance.qualityLevel;
        qualityDropdown.RefreshShownValue();

        
        volumeSlider.value = SaveManager.Instance.volume;

       
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
        closeButton.onClick.AddListener(CloseSettings);

        
        GameObject pm = GameObject.Find("PauseMenu");
        if (pm != null)
        {
            _pauseMenu = pm.GetComponent<PauseMenu>();
        }
        else
        {
            Debug.LogError("PauseMenu bulunamadı!");
        }
    }

    private void OnVolumeChanged(float value)
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.volume = value;
            AudioListener.volume = value;
            SaveManager.Instance.SaveGame();
        }
    }

    private void OnResolutionChanged(int index)
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.resolutionIndex = index;
            Resolution resolution = _resolutions[index];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
            SaveManager.Instance.SaveGame();
        }
    }

    private void OnQualityChanged(int index)
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.qualityLevel = index;
            QualitySettings.SetQualityLevel(index);
            SaveManager.Instance.SaveGame();
        }
    }

    public void CloseSettings()
    {
        gameObject.SetActive(false);

        
        if (_pauseMenu != null && _pauseMenu.IsPaused())
        {
            _pauseMenu.ShowPauseMenu();
        }
        else
        {
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
