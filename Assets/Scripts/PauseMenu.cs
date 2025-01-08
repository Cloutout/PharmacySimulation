using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Pause Menu UI")]
    public GameObject pauseMenuUI;

    [Header("Pause Menu Buttons")]
    public Button resumeButton;
    public Button settingsButton;
    public Button saveGameButton;
    public Button backToMainMenuButton;
    public Button quitButton;

    [Header("Settings Panel")]
    public GameObject settingsPanel;

    private bool _isPaused;

    private void Start()
    {
        
        resumeButton.onClick.AddListener(Resume);
        settingsButton.onClick.AddListener(OpenSettings);
        saveGameButton.onClick.AddListener(SaveGame);
        backToMainMenuButton.onClick.AddListener(BackToMainMenu);
        quitButton.onClick.AddListener(QuitGame);

        
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        
        HideCursor();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
        Time.timeScale = 1f;
        HideCursor();
        _isPaused = false;
    }

    public void Pause()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
        }
        Time.timeScale = 0f;
        ShowCursor();
        _isPaused = true;
    }

    public void SaveGame()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SaveGame();
            Debug.Log("Oyun kaydedildi.");
        }
        else
        {
            Debug.LogError("SaveManager bulunamadı!");
        }
    }

    public void BackToMainMenu()
    {
        
        SaveManager.Instance.SaveGame();

        Time.timeScale = 1f;
        HideCursor();
        SceneManager.LoadScene("MainMenu"); 
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            ShowCursor();

            
            if (pauseMenuUI != null)
            {
                pauseMenuUI.SetActive(false);
            }
        }
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Oyun kapatıldı.");
    }


    private void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


    private void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public bool IsPaused()
    {
        return _isPaused;
    }

    
    public void ShowPauseMenu()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
            ShowCursor();
            _isPaused = true;
        }
    }
}
