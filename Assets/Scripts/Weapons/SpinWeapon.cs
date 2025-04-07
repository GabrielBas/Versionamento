using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinWeapon : Weapon
{
    public float rotateSpeed;

    public Transform holder, fireballToSpawn;

    public float timeBetweenSpawn;
    private float spawnCounter;

    public EnemyDamage damage;

    [Header("Fireball Orbit Settings")]
    public float orbitRadius = 2f;  // Controle do raio da órbita

    private List<Transform> spawnedFireballs = new List<Transform>();


    void Start()
    {
        SetStats();
    }

    void Update()
    {
        // Faz as fireballs girarem
        holder.rotation = Quaternion.Euler(0f, 0f, holder.rotation.eulerAngles.z + (rotateSpeed * Time.deltaTime * stats[weaponLevel].speed));

        spawnCounter -= Time.deltaTime;
        if (spawnCounter <= 0)
        {
            spawnCounter = timeBetweenSpawn;

            SpawnFireballsAroundPlayer();
        }

        if (statsUpdated)
        {
            statsUpdated = false;
            SetStats();
            SpawnFireballsAroundPlayer(); // Atualiza o círculo de fireballs
        }
    }

    void SpawnFireballsAroundPlayer()
    {
        int amount = Mathf.RoundToInt(stats[weaponLevel].amount);
        float radius = orbitRadius * stats[weaponLevel].range;

        // Instancia fireballs adicionais se necessário
        while (spawnedFireballs.Count < amount)
        {
            Transform fireball = Instantiate(fireballToSpawn, transform.position, Quaternion.identity, holder);
            fireball.gameObject.SetActive(true);
            spawnedFireballs.Add(fireball);
        }

        // Se houver mais do que o necessário, desativa ou remove extras
        while (spawnedFireballs.Count > amount)
        {
            Destroy(spawnedFireballs[spawnedFireballs.Count - 1].gameObject);
            spawnedFireballs.RemoveAt(spawnedFireballs.Count - 1);
        }

        // Posiciona todas as fireballs igualmente ao redor do player
        for (int i = 0; i < spawnedFireballs.Count; i++)
        {
            float angle = (360f / spawnedFireballs.Count) * i;
            float angleRad = angle * Mathf.Deg2Rad;

            Vector3 offset = new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad), 0f) * radius;
            Vector3 spawnPos = transform.position + offset;

            spawnedFireballs[i].position = spawnPos;
        }
    }



    public void SetStats()
    {
        damage.damageAmount = stats[weaponLevel].damage;
        transform.localScale = Vector3.one * stats[weaponLevel].range;
        timeBetweenSpawn = stats[weaponLevel].timeBetweenAttacks;
        damage.lifeTime = stats[weaponLevel].duration;
        spawnCounter = 0f;
    }
}
