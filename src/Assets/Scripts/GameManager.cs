using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// Game Manager Singleton
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Game Manager Instance
    /// </summary>
    public static GameManager Instance { get; private set; }

    /// <summary>
    /// Event for Terminal Alert
    /// </summary>
    public UnityEvent<Vector3> TerminalAlertEvent;

    /// <summary>
    /// Player Game Object
    /// </summary>
    public GameObject player;

    [SerializeField]
    /// <summary>
    /// Graphical User Interface
    /// </summary>
    private GameObject GUI;

    [SerializeField]
    /// <summary>
    /// Health Slider UI Component
    /// </summary>
    private Slider healthSlider;

    /// <summary>
    /// Brain Power Slider UI Component
    /// </summary>
    private Slider brainPowerSlider;

    [SerializeField]
    /// <summary>
    /// Enemy Health Slider UI Component
    /// </summary>
    private Slider hostHealthSlider;

    [SerializeField]
    /// <summary>
    /// Enemy Gun Slider UI Component
    /// </summary>
    private Slider gunSlider;

    [SerializeField]
    /// <summary>
    /// Enemy Gun Icon
    /// </summary>
    private GameObject gunIcon;

    /// <summary>
    /// Enemy Syringe Icon
    /// </summary>
    private GameObject syringeIcon;

    /// <summary>
    /// Enemy Keycard Icon
    /// </summary>
    private GameObject keycardIcon;

    /// <summary>
    /// Player Health Component
    /// </summary>
    public PlayerHealth playerHealth;

    /// <summary>
    /// Health of Current Host
    /// </summary>
    public EnemyHealth hostHealth;

    /// <summary>
    /// Host Shooting Component
    /// </summary>
    public EnemyShoot hostShoot;

    /// <summary>
    /// Host Assassination Component
    /// </summary>
    public EnemyAssassination hostAssassination;

    /// <summary>
    /// Host Suspicion Component
    /// </summary>
    public EnemySuspicion hostSuspicion;

    /// <summary>
    /// Current Brain Power Value
    /// </summary>
    public float BrainPower
    {
        get { return brainPowerSlider.value; }
    }
  
  PauseMenu pause;

  string lastSceneName;
  int loopCheck;

    /// <summary>
    /// Called When Script Instance Is Loaded
    /// </summary>
    void Awake()
    {
        
        // If Instance Is Missing...
        if (Instance == null)
        {
            
            //DontDestroyOnLoad(gameObject);
            Instance = this;

            // If Terminal Alert Event Is Null...
            if (TerminalAlertEvent == null)
            {
                TerminalAlertEvent = new UnityEvent<Vector3>();
            }
        }
        else
        {
            Debug.Log("Instance is missing");
            Destroy(gameObject);
            
        }
    }

    /// <summary>
    /// Called Before First Frame Update
    /// </summary>
    void Start()
    {
        loopCheck = 0;
        pause = GetComponent<PauseMenu>();
        Time.timeScale = 1f;
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            player = GameObject.FindGameObjectWithTag("Player");
            GameObject.Instantiate(GUI);
            healthSlider = GameObject.Find("PlayerHealthSlider").GetComponent<Slider>();
            brainPowerSlider = GameObject.Find("BrainPowerSlider").GetComponent<Slider>();
            hostHealthSlider = GameObject.Find("HostHealthSlider").GetComponent<Slider>();
            gunIcon = GameObject.Find("GunIcon");
            gunSlider = GameObject.Find("GunSlider").GetComponent<Slider>();
            syringeIcon = GameObject.Find("SyringeIcon");
            keycardIcon = GameObject.Find("KeycardIcon");
        }

         lastSceneName= PlayerPrefs.GetString("lastSceneName");
        
    }

    /// <summary>
    /// Called Once per Frame
    /// </summary>
    void Update()
    {

    if(lastSceneName == "Options" && lastSceneName!= "MainMenu" && loopCheck == 0){
                pause.TogglePause();
                loopCheck++;
            }
    GUI.SetActive(true);
           
        if (hostHealth && hostHealth.Health > 0)
        {
            brainPowerSlider.value += Time.deltaTime;
        }
        else
        {
            TakePlayerDamage(-Time.deltaTime * 2);
        }

        if (hostHealth)
        {
            hostHealthSlider.maxValue = hostHealth.MaxHealth;
            hostHealthSlider.value = hostHealth.Health;
        }
        else
        {
            hostHealthSlider.value = 0;
        }

        if (hostShoot && hostHealth.Health > 0)
        {
            gunSlider.maxValue = hostShoot.MaxAmmo;
            gunSlider.value = hostShoot.Ammo;

            gunIcon.SetActive(true);
        }
        else
        {
            gunIcon.SetActive(false);
        }

        if (hostAssassination && hostAssassination.hasSyringe && hostHealth.Health > 0)
        {
            syringeIcon.SetActive(true);
        }
        else
        {
            syringeIcon.SetActive(false);
        }

        if (hostSuspicion && hostSuspicion.keycardHolder && hostHealth.Health > 0)
        {
            keycardIcon.SetActive(true);
        }
        else
        {
            keycardIcon.SetActive(false);
        }
    }

    /// <summary>
    /// Take Player Damage
    /// </summary>
    /// <param name="value"> Damage Value</param>
    public void TakePlayerDamage(float value)
    {
        healthSlider.value += value;

        if (healthSlider.value <= healthSlider.minValue)
        {
                
                SceneManager.LoadScene("LoseScene", LoadSceneMode.Single);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

        }
    }

    /// <summary>
    /// Use Brain Power
    /// </summary>
    /// <param name="value"> Power Value</param>
    public void UseBrainPower(float value)
    {
        brainPowerSlider.value += value;
    }

    private void OnDestroy()
{
    // Save the name of the current scene before switching to the next scene
    string currentSceneName = SceneManager.GetActiveScene().name;
    PlayerPrefs.SetString("lastSceneName", currentSceneName);
}
}
