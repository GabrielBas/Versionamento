using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoundary : MonoBehaviour
{
    private Vector2 minBounds;
    private Vector2 maxBounds;

    private void Start()
    {
        // Obtém o Collider do MapBounds
        BoxCollider2D boundsCollider = GameObject.FindGameObjectWithTag("MapBounds").GetComponent<BoxCollider2D>();

        // Define os limites com base no Collider do mapa
        minBounds = boundsCollider.bounds.min;
        maxBounds = boundsCollider.bounds.max;
    }

    private void LateUpdate()
    {
        // Pega a posição do Player
        Vector3 playerPosition = transform.position;

        // Restringe o Player dentro dos limites do mapa
        playerPosition.x = Mathf.Clamp(playerPosition.x, minBounds.x, maxBounds.x);
        playerPosition.y = Mathf.Clamp(playerPosition.y, minBounds.y, maxBounds.y);

        // Aplica a posição corrigida
        transform.position = playerPosition;
    }
}
