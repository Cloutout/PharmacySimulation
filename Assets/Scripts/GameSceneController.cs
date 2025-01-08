using UnityEngine;

public class GameSceneController : MonoBehaviour
{
    void Start()
    {
        
        HideCursor();
    }

    
    public void ShowCursor()
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