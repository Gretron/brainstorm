using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuPanel;
    public Canvas overlayCanvas;
    public FollowPlayer follow;
    private bool isPaused = false;

    void Start()
    {
        pauseMenuPanel.SetActive(false);
        overlayCanvas.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
        pauseMenuPanel.SetActive(true);
        follow.SetPaused(true);
        overlayCanvas.enabled = true; 
        
        Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1f;
        pauseMenuPanel.SetActive(false);
        follow.SetPaused(false);
        overlayCanvas.enabled = false; 

        // Cursor.lockState = CursorLockMode.Locked;
        //     Cursor.visible = false;
        }
    }

    public void Resume()
    {
        TogglePause();
    }

    // private void OnDisable()
    // {
    //     pauseMenuPanel.SetActive(false);
    //     overlayCanvas.enabled = false;
    // }
}

