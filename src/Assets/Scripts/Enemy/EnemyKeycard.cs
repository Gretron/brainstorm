using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKeycard : MonoBehaviour
{
    private HostPossession possession;
    private EnemyHealth health;
    private EnemySuspicion suspicion;
    private MeshRenderer cardMesh;
    private MeshRenderer slitMesh;

    // Start is called before the first frame update
    void Start()
    {
        possession = GameObject.FindGameObjectWithTag("Player").GetComponent<HostPossession>();
        suspicion = GetComponentInParent<EnemySuspicion>();
        cardMesh = GetComponent<MeshRenderer>();
        slitMesh = transform.Find("Slit").GetComponent<MeshRenderer>();
        health = GetComponentInParent<EnemyHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        if (suspicion.keycardHolder)
        {
            cardMesh.enabled = true;
            slitMesh.enabled = true;

            if (health.Health < 1 && possession.enemy != null)
            {
                var hostPosition = possession.enemy.transform.position;

                var enemyToHost = hostPosition - suspicion.transform.position;

                Debug.Log(enemyToHost.sqrMagnitude);

                if (enemyToHost.sqrMagnitude < 10)
                {
                    Debug.Log("deadahh");

                    possession.enemy.GetComponent<EnemySuspicion>().keycardHolder = true;
                    suspicion.keycardHolder = false;
                }
            }
        }
        else
        {
            cardMesh.enabled = false;
            slitMesh.enabled = false;
        }
    }
}
