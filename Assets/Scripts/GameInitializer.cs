using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [Header("Main Menu UI")]
    public GameObject mainMenuUI;

    private void Start()
    {
        
        if (mainMenuUI != null)
        {
            mainMenuUI.SetActive(true);
            Time.timeScale = 0f; 
            ShowCursor();
        }
    }

    public void StartGame()
    {
        if (mainMenuUI != null)
        {
            mainMenuUI.SetActive(false);
        }
        Time.timeScale = 1f;
        HideCursor();
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