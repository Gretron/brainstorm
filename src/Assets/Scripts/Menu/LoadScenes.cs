using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScenes : MonoBehaviour
{
    string sceneName;
    string currentScene;

    void Start()
    {
        

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

    //    currentScene = GameManager.Instance.getCurrentScene();

        
    //      if(currentScene == "Level1"){
            
    //         SceneManager.LoadScene("Level1");
    //      }
            
    }
}
