using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SuspicionMeter : MonoBehaviour
{
    [SerializeField]
    private Sprite[] meterSprites;

    public EnemySuspicion es;

    public Image img;

    // Start is called before the first frame update
    void Start()
    {
        es = GetComponentInParent<EnemySuspicion>();
        img = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        int index = Mathf.Min(
            (int)(es.GetPercentageDetected() * meterSprites.Length),
            meterSprites.Length - 1
        );

        img.sprite = meterSprites[index];
    }
}
