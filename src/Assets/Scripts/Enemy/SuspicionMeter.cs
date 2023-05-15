using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SuspicionMeter : AnimatableImage
{
    [SerializeField]
    private SpriteFrames spottedAnimation;

    [SerializeField]
    private SpriteFrames detectedAnimation;

    [SerializeField]
    private SpriteFrames alertedAnimation;

    private EnemySuspicion es;

    // private Image img;

    /// <summary>
    /// Called Before First Frame Update
    /// </summary>
    void Start()
    {
        base.Start();

        es = GetComponentInParent<EnemySuspicion>();
        // img = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (es.suspicion == Suspicion.Alerted)
        {
            Play(alertedAnimation, 0, 1);
        }
        else if (es.suspicion == Suspicion.Curious)
        {
            Play(detectedAnimation, es.AlertedCounter, es.AlertedThreshold);
        }
        else
        {
            Play(spottedAnimation, es.CuriousCounter, es.CuriousThreshold);
        }
    }
}
