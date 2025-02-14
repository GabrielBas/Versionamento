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

    private float timeCount;
    private int currentWaveIndex = 0;
    private GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(SpawnWaves());
    }

    void Update()
    {
        timeCount += Time.deltaTime;
    }

    IEnumerator SpawnWaves()
    {
        while (currentWaveIndex < waves.Count)
        {
            Wave currentWave = waves[currentWaveIndex];
            Debug.Log("Wave " + (currentWaveIndex + 1) + " started!");

            SpawnEnemies(currentWave);
            yield return new WaitForSeconds(currentWave.waveInterval);

            currentWaveIndex++;
        }

        Debug.Log("All waves completed!");
    }

    void SpawnEnemies(Wave wave)
    {
        foreach (var enemyInfo in wave.enemies)
        {
            int enemiesToSpawn = Random.Range(enemyInfo.minSpawnAmount, enemyInfo.maxSpawnAmount);

            for (int i = 0; i < enemiesToSpawn; i++)
            {
                Vector2 spawnPosition = (Vector2)player.transform.position + Random.insideUnitCircle * enemyInfo.spawnRadius;
                Instantiate(enemyInfo.enemyPrefab, spawnPosition, Quaternion.identity);
            }
        }
    }
}