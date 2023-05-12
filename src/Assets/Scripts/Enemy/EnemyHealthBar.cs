using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    private EnemyHealth enemyHealth;
    private Slider healthSlider;

    // Start is called before the first frame update
    void Start()
    {
        enemyHealth = transform.parent.parent.GetComponent<EnemyHealth>();
        healthSlider = transform.Find("HealthBarBackground/HealthSlider").GetComponent<Slider>();
        healthSlider.maxValue = enemyHealth.MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        healthSlider.value = enemyHealth.Health;

        if (enemyHealth.Health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
