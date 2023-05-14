using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScenes : MonoBehaviour
{
    string currentSceneName;

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

         if(currentSceneName == "level1"){
            
            SceneManager.LoadScene("level1");
         }

         if(currentSceneName == "level2"){
            
            SceneManager.LoadScene("level2");
         }

         if(currentSceneName == "Level3"){
            
            SceneManager.LoadScene("Level3");
         }
            
    }
}
