using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestSpawner : MonoBehaviour
{
    public GameObject chestPrefab; // O prefab do baú
    public GameObject arrowPrefab; // O prefab da seta
    public float spawnInterval = 30f; // Intervalo de spawn em segundos
    public float minDistanceFromPlayer = 10f; // Distância mínima do jogador para spawnar o baú
    public float maxDistanceFromPlayer = 50f; // Distância máxima do jogador para spawnar o baú
    public int maxChests = 5; // Número máximo de baús que podem estar presentes ao mesmo tempo

    private GameObject player;
    private List<GameObject> spawnedChests = new List<GameObject>();
    private List<GameObject> spawnedArrows = new List<GameObject>();

    private Vector2 minBounds;
    private Vector2 maxBounds;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        // Obtém os limites do mapa
        BoxCollider2D boundsCollider = GameObject.FindGameObjectWithTag("MapBounds").GetComponent<BoxCollider2D>();
        minBounds = boundsCollider.bounds.min;
        maxBounds = boundsCollider.bounds.max;

        // Inicia a rotina de spawn
        StartCoroutine(SpawnChestsRoutine());
    }

    private IEnumerator SpawnChestsRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (spawnedChests.Count < maxChests)
            {
                SpawnChest();
            }
        }
    }

    private void SpawnChest()
    {
        Vector2 spawnPosition = GetRandomPositionWithinBounds();

        GameObject newChest = Instantiate(chestPrefab, spawnPosition, Quaternion.identity);
        spawnedChests.Add(newChest);

        GameObject newArrow = Instantiate(arrowPrefab, spawnPosition, Quaternion.identity);
        spawnedArrows.Add(newArrow);

        OffScreenTargetIndicator.RegisterTarget(newChest, newArrow);

        chestPrefab.SetActive(true); // Ativa o baú ao ser spawnado
    }

    private Vector2 GetRandomPositionWithinBounds()
    {
        Vector2 spawnPosition;
        int attempts = 10; // Número máximo de tentativas para encontrar uma posição válida

        do
        {
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            float randomDistance = Random.Range(minDistanceFromPlayer, maxDistanceFromPlayer);
            spawnPosition = (Vector2)player.transform.position + randomDirection * randomDistance;

            // Garante que o spawn está dentro dos limites do mapa
            spawnPosition.x = Mathf.Clamp(spawnPosition.x, minBounds.x, maxBounds.x);
            spawnPosition.y = Mathf.Clamp(spawnPosition.y, minBounds.y, maxBounds.y);

            attempts--;
        }
        while (attempts > 0 && !IsPositionValid(spawnPosition));

        return spawnPosition;
    }

    private bool IsPositionValid(Vector2 position)
    {
        // Checa se há obstáculos na posição escolhida (opcional)
        Collider2D hit = Physics2D.OverlapCircle(position, 1f, LayerMask.GetMask("Obstacles"));
        return hit == null; // Retorna true se não houver colisão
    }
}
