using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMusic : MonoBehaviour
{
    public AudioClip musicClip;
    private AudioSource musicSource;
     int lastSceneIndex;
     
     string scene;
    
    void Start()
    {
          musicSource = GetComponent<AudioSource>();
        DontDestroyOnLoad(this.gameObject);
         lastSceneIndex = PlayerPrefs.GetInt("lastSceneIndex");
         scene = SceneManager.GetSceneByBuildIndex(lastSceneIndex).name;
        
    }

     private void OnDestroy()
    {
        // Save the build index of the current scene before switching to the next scene
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        PlayerPrefs.SetInt("lastSceneIndex", currentSceneIndex);
    }

    void Update() {
 
    // Check if the current scene is the specific scene you want to continue playing the audio in
    if (scene == "MainMenu") {
      
        // Check if the audio is not already playing
        if (!GetComponent<AudioSource>().isPlaying) {
           
          
        musicSource.clip = musicClip;
        musicSource.Play();
        }
    } else {
        // If not in the specific scene, stop playing the audio clip
        GetComponent<AudioSource>().Stop();
    }
}
}
