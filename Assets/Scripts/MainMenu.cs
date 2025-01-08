using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Menu Buttons")]
    public Button newGameButton;
    public Button loadGameButton;
    public Button settingsButton;
    public Button quitButton;

    [Header("Settings Panel")]
    public GameObject settingsPanel;

    private void Start()
    {
        
        if (SaveManager.Instance != null)
        {
            
            loadGameButton.gameObject.SetActive(SaveManager.Instance.HasSave());
        }
        else
        {
            Debug.LogError("SaveManager bulunamadı!");
        }

       
        newGameButton.onClick.AddListener(OnNewGame);
        loadGameButton.onClick.AddListener(OnLoadGame);
        settingsButton.onClick.AddListener(OnSettings);
        quitButton.onClick.AddListener(OnQuit);

        
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        
        ShowCursor();
    }

    public void OnNewGame()
    {
        SaveManager.Instance.NewGame();
        HideCursor();
        SceneManager.LoadScene("GameScene"); 
    }

    public void OnLoadGame()
    {
        if (SaveManager.Instance.HasSave())
        {
            bool loadSuccess = SaveManager.Instance.LoadGame();
            if (loadSuccess)
            {
                HideCursor();
                SceneManager.LoadScene("GameScene");
            }
            else
            {
                Debug.LogWarning("Oyun yüklenirken bir hata oluştu.");
            }
        }
        else
        {
            Debug.LogWarning("Yükleme için save dosyası bulunamadı.");
        }
    }

    public void OnSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            ShowCursor();
        }
    }

    public void OnQuit()
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
}
