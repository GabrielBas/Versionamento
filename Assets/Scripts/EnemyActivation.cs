using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyActivation : MonoBehaviour
{
    public GameObject enemy; // The enemy GameObject to activate
    public float activationRadius = 5f; // The radius within which the enemy will be activated
    private bool isActive = false; // To track if the enemy is already active
    

    private void Start()
    {
        // Ensure the enemy is initially deactivated
        enemy.SetActive(true);
    }

    private void Update()
    {
        // Check the distance between the player and this object
        if (!isActive)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, Player.instance.transform.position);
            if (distanceToPlayer <= activationRadius)
            {
                ActivateEnemy();
            }
        }
    }

    private void ActivateEnemy()
    {
        // Activate the enemy and set isActive to true
        enemy.SetActive(true);
        isActive = true;
        
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a red sphere at the transform's position to show the activation radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, activationRadius);
    }
}

