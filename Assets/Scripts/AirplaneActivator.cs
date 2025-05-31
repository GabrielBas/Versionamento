using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirplaneActivator : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            AirplaneController controller = FindObjectOfType<AirplaneController>();
            if (controller != null)
            {
                controller.ActivateAirplane();
                Destroy(gameObject); // Remove o ativador após o uso
            }
        }
    }
}
