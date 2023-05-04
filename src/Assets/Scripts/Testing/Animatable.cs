using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Sprite Animatable Base Class
/// </summary>
public abstract class AnimatableImage : MonoBehaviour
{
    /// <summary>
    /// Image Component
    /// </summary>
    protected Image img;

    /// <summary>
    /// Called Before First Frame Update
    /// </summary>
    protected void Start()
    {
        img = GetComponent<Image>();
    }

    /// <summary>
    /// Get Current Sprite in Animation
    /// </summary>
    /// <param name="frames"> All Sprites in Animation </param>
    /// <param name="value"> Value in Range </param>
    /// <param name="maxValue"> Max Value in Range </param>
    /// <param name="minValue"> Min Value in Range </param>
    /// <returns> Current Sprite </returns>
    public void Play(SpriteFrames sprites, float value, float maxValue, float minValue = 0)
    {
        // Get Percentage of Value Between Range
        double range = maxValue - minValue;
        double percentage = (value - minValue) / range;

        // Set Image to Appropriate Frame Within Range
        int index = Mathf.Min((int)(percentage * sprites.frames.Length), sprites.frames.Length - 1);
        img.sprite = sprites.frames[index];
    }
}
