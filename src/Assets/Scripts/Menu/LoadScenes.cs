using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScenes : MonoBehaviour
{
    string currentSceneName;
    int lastSceneIndex;

    void Start()
    {

lastSceneIndex = PlayerPrefs.GetInt("lastSceneIndex");
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
        
        SceneManager.LoadScene(lastSceneIndex);
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
       SceneManager.LoadScene("MainMenu");
    }

    public void LoadRestart()
    {

        SceneManager.LoadScene(lastSceneIndex);
            
    }

       private void OnDestroy()
    {
        // Save the build index of the current scene before switching to the next scene
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        PlayerPrefs.SetInt("lastSceneIndex", currentSceneIndex);
    }
}
