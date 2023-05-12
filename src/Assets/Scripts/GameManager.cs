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

    /// <summary>
    /// Health of Current Host
    /// </summary>
    public EnemyHealth hostHealth;

    /// <summary>
    /// Host Shooting Component
    /// </summary>
    public EnemyShoot hostShoot;

    /// <summary>
    /// Called When Script Instance Is Loaded
    /// </summary>
    void Awake()
    {
        // If Instance Is Missing...
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;

            // If Terminal Alert Event Is Null...
            if (TerminalAlertEvent == null)
            {
                TerminalAlertEvent = new UnityEvent<Vector3>();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Called Before First Frame Update
    /// </summary>
    void Start()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            GameObject.Instantiate(GUI);
            healthSlider = GameObject.Find("HealthSlider").GetComponent<Slider>();
            brainPowerSlider = GameObject.Find("BrainPowerSlider").GetComponent<Slider>();
            hostHealthSlider = GameObject.Find("HostHealthSlider").GetComponent<Slider>();
            gunSlider = GameObject.Find("GunSlider").GetComponent<Slider>();
        }
    }

    /// <summary>
    /// Called Once per Frame
    /// </summary>
    void Update()
    {
        brainPowerSlider.value -= Time.deltaTime / 2;

        if (hostHealth)
        {
            hostHealthSlider.maxValue = hostHealth.MaxHealth;
            hostHealthSlider.value = hostHealth.Health;
        }

        if (hostShoot)
        {
            gunSlider.maxValue = hostShoot.MaxAmmo;
            gunSlider.value = hostShoot.Ammo;
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
        }
    }
}
