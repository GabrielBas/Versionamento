using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class EnemyInfo
{
    public Enemy enemyPrefab;
    public int minSpawnAmount;
    public int maxSpawnAmount;
    public float spawnRadius;
}

[System.Serializable]
public class Wave
{
    public List<EnemyInfo> enemies;
    public float waveInterval;
}

public class Spawn : MonoBehaviour
{
    public List<Wave> waves;
    public int maxEnemiesInScene = 50; // Limite de inimigos ativos

    private float timeCount;
    private int currentWaveIndex = 0;
    private GameObject player;

    private List<Enemy> activeEnemies = new List<Enemy>();

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(SpawnWaves());
    }

    void Update()
    {
        timeCount += Time.deltaTime;

        // Limpa inimigos mortos da lista
        activeEnemies.RemoveAll(enemy => enemy == null);
    }

    IEnumerator SpawnWaves()
    {
        while (currentWaveIndex < waves.Count)
        {
            Wave currentWave = waves[currentWaveIndex];
            Debug.Log("Wave " + (currentWaveIndex + 1) + " started!");

            yield return StartCoroutine(SpawnEnemies(currentWave));

            yield return new WaitForSeconds(currentWave.waveInterval);

            currentWaveIndex++;
        }

        Debug.Log("All waves completed!");
    }

    IEnumerator SpawnEnemies(Wave wave)
    {
        foreach (var enemyInfo in wave.enemies)
        {
            int enemiesToSpawn = Random.Range(enemyInfo.minSpawnAmount, enemyInfo.maxSpawnAmount);

            for (int i = 0; i < enemiesToSpawn; i++)
            {
                if (activeEnemies.Count >= maxEnemiesInScene)
                {
                    Debug.LogWarning("Número máximo de inimigos atingido! Aguardando espaço para spawnar mais...");
                    yield return new WaitUntil(() => activeEnemies.Count < maxEnemiesInScene);
                }

                Vector2 spawnPosition = (Vector2)player.transform.position + Random.insideUnitCircle * enemyInfo.spawnRadius;
                Enemy newEnemy = Instantiate(enemyInfo.enemyPrefab, spawnPosition, Quaternion.identity);
                activeEnemies.Add(newEnemy);

                yield return null; // Aguarda 1 frame para dar respiro ao sistema
            }
        }
    }
}
