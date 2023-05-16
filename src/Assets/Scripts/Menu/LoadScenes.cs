using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScenes : MonoBehaviour
{
    string currentSceneName;
    int lastSceneIndex;
    PauseMenu pauseMenu;

    void Start()
    {

        currentSceneName = SceneManager.GetActiveScene().name;

        
    }

    void Update()
    {
        
    }

    public void LoadLevel1()
    {
        SceneManager.LoadScene("level1");
    }

    public void Back()
    {
       
    string lastSceneName = PlayerPrefs.GetString("lastSceneName");
    SceneManager.LoadScene(lastSceneName);

    }
    public void LoadOptions()
    {

        SceneManager.LoadScene("Options");
        
    }


    public void LoadExit()
    {
        Application.Quit();
    }

     public void MainMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
       SceneManager.LoadScene("MainMenu");
    }

    public void LoadRestart()
    {

      
    SceneManager.LoadScene(currentSceneName);
            
    }

    public void LoadRestartLose()
    {

      
   string lastSceneName = PlayerPrefs.GetString("lastSceneName");
    SceneManager.LoadScene(lastSceneName);
            
    }

       private void OnDestroy()
{
    // Save the name of the current scene before switching to the next scene
    string currentSceneName = SceneManager.GetActiveScene().name;
    PlayerPrefs.SetString("lastSceneName", currentSceneName);
}

}
