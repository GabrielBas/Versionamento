using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform target;
    public Collider2D cameraBounds; // Collider que define os limites do mapa
    private Vector3 minBounds, maxBounds;
    private float camHalfHeight, camHalfWidth;
    private Camera cam;

    void Start()
    {
        target = FindObjectOfType<Player>().transform;
        cam = Camera.main;

        // Obtém os limites do collider do mapa
        if (cameraBounds != null)
        {
            minBounds = cameraBounds.bounds.min;
            maxBounds = cameraBounds.bounds.max;
        }

        // Calcula metade da largura e altura da câmera
        camHalfHeight = cam.orthographicSize;
        camHalfWidth = camHalfHeight * cam.aspect;
    }

    void LateUpdate()
    {
        if (target == null)
            return;

        // Obtém a posição desejada da câmera
        Vector3 newPosition = new Vector3(target.position.x, target.position.y, transform.position.z);

        // Restringe a posição dentro dos limites do mapa
        if (cameraBounds != null)
        {
            newPosition.x = Mathf.Clamp(newPosition.x, minBounds.x + camHalfWidth, maxBounds.x - camHalfWidth);
            newPosition.y = Mathf.Clamp(newPosition.y, minBounds.y + camHalfHeight, maxBounds.y - camHalfHeight);
        }

        transform.position = newPosition;
    }
}
