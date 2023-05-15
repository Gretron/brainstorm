using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    Vector3 startPosition;
    float movementFactor;

    [Header("Parameters")]
    [SerializeField]
    Vector3 movementVector;

    MeshRenderer mesh;

    [SerializeField]
    float cycleTime = 5f;

    [SerializeField]
    private MonoBehaviour[] behaviours;

    [SerializeField]
    private bool flag = true;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        mesh = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (behaviour.enabled != flag)
            {
                mesh.enabled = false;
                return;
            }
        }

        mesh.enabled = true;

        if (cycleTime <= 0f)
            return;

        float cycles = Time.time / cycleTime;
        float tau = Mathf.PI * 2;
        float sinWave = Mathf.Sin(cycles * tau);

        movementFactor = sinWave / 2f + 0.5f;

        Vector3 offset = movementVector * movementFactor;
        transform.position = startPosition + offset;
    }
}
