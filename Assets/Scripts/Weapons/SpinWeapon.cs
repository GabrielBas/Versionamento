using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinWeapon : Weapon
{
    public float rotateSpeed;

    public Transform holder, fireballToSpawn;
    public GameObject fireballTrailPrefab; // <-- Trail prefab separado

    public float timeBetweenSpawn;
    private float spawnCounter;

    public EnemyDamage damage;

    [Header("Fireball Orbit Settings")]
    public float orbitRadius = 2f;

    private List<Transform> spawnedFireballs = new List<Transform>();
    private List<GameObject> spawnedTrails = new List<GameObject>();

    void Start()
    {
        SetStats();
    }

    void Update()
    {
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
            SpawnFireballsAroundPlayer();
        }
    }

    void SpawnFireballsAroundPlayer()
    {
        int amount = Mathf.RoundToInt(stats[weaponLevel].amount);
        float radius = orbitRadius * stats[weaponLevel].range;

        // Instanciar fireballs e trails se necessário
        while (spawnedFireballs.Count < amount)
        {
            Transform fireball = Instantiate(fireballToSpawn, transform.position, Quaternion.identity, holder);
            fireball.gameObject.SetActive(true);
            spawnedFireballs.Add(fireball);

            // Trail
            GameObject trail = Instantiate(fireballTrailPrefab, fireball.position, Quaternion.identity);
            trail.transform.SetParent(fireball); // Segue a fireball
            spawnedTrails.Add(trail);
        }

        // Remover extras
        while (spawnedFireballs.Count > amount)
        {
            Destroy(spawnedFireballs[^1].gameObject);
            spawnedFireballs.RemoveAt(spawnedFireballs.Count - 1);

            Destroy(spawnedTrails[^1]);
            spawnedTrails.RemoveAt(spawnedTrails.Count - 1);
        }

        // Posicionar fireballs em órbita
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
