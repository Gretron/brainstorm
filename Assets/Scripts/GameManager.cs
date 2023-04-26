using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Game Manager Singleton
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Game Manager Instance
    /// </summary>
    public static GameManager Instance { get; private set; }

    public UnityEvent TerminalAlertEvent;

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
                TerminalAlertEvent = new UnityEvent();
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
    void Start() { }

    /// <summary>
    /// Called Once per Frame
    /// </summary>
    void Update() { }
}
