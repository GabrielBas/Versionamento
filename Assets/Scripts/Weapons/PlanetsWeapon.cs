using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetsWeapon : Weapon
{
    public float rotateSpeed;
    public Transform holder, fireballToSpawn;
    public EnemyDamageOrbiting damagePrefab;
    public GameObject planetsTrailPrefab;
    public float timeBetweenSpawn;
    private float spawnCounter;

    private class ProjectileData
    {
        public GameObject projectileObject;
        public float range;
        public float damage;
        public float speed;
        public float duration;

        public ProjectileData(GameObject obj, float range, float damage, float speed, float duration)
        {
            projectileObject = obj;
            this.range = range;
            this.damage = damage;
            this.speed = speed;
            this.duration = duration;
        }
    }

    private class TrailFollowData
    {
        public Transform trailObject;
        public Transform dummyFollower;

        public TrailFollowData(Transform trail, Transform dummy)
        {
            trailObject = trail;
            dummyFollower = dummy;
        }
    }

    private List<ProjectileData> activeProjectiles = new List<ProjectileData>();
    private List<TrailFollowData> trails = new List<TrailFollowData>();

    void Start()
    {
        SetStats();
        SpawnProjectilesForLevel();
    }

    void Update()
    {
        holder.rotation = Quaternion.Euler(0f, 0f, holder.rotation.eulerAngles.z + (rotateSpeed * Time.deltaTime));

        foreach (var projData in activeProjectiles)
        {
            projData.projectileObject.transform.RotateAround(transform.position, Vector3.forward, projData.speed * Time.deltaTime);
        }

        // Atualiza posição dos dummies e trails
        foreach (var trailData in trails)
        {
            if (trailData.trailObject != null && trailData.dummyFollower != null)
            {
                trailData.trailObject.position = trailData.dummyFollower.position;
                trailData.trailObject.rotation = trailData.dummyFollower.rotation;
            }
        }

        if (statsUpdated)
        {
            statsUpdated = false;
            SetStats();
            SpawnProjectilesForLevel();
        }
    }

    public void SetStats()
    {
        spawnCounter = 0f;
        timeBetweenSpawn = stats[weaponLevel].timeBetweenAttacks;

        if (fireballToSpawn != null)
        {
            fireballToSpawn.localPosition = new Vector3(stats[weaponLevel].holderDistance, fireballToSpawn.localPosition.y, fireballToSpawn.localPosition.z);
        }
    }

    private void SpawnProjectilesForLevel()
    {
        for (int i = 0; i < stats[weaponLevel].amount; i++)
        {
            float angle = (360f / stats[weaponLevel].amount) * i;
            Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

            GameObject newProjectile = Instantiate(fireballToSpawn, fireballToSpawn.position, rotation, holder).gameObject;
            newProjectile.SetActive(true);

            EnemyDamageOrbiting projDamage = newProjectile.GetComponent<EnemyDamageOrbiting>();
            if (projDamage != null)
            {
                projDamage.currentLevel = weaponLevel;
                projDamage.UpdateSpriteForLevel();
                projDamage.damageAmount = stats[weaponLevel].damage;
                projDamage.lifeTime = Mathf.Infinity;
            }

            // Encontra visual chamado "Planets"
            Transform planetVisual = newProjectile.transform.Find("Planets");
            if (planetsTrailPrefab != null && planetVisual != null)
            {
                // Cria dummy para seguir o visual
                GameObject dummy = new GameObject("TrailDummy");
                dummy.transform.position = planetVisual.position;
                dummy.transform.rotation = planetVisual.rotation;

                // Cria trail
                GameObject trail = Instantiate(planetsTrailPrefab, dummy.transform.position, dummy.transform.rotation);
                trail.transform.SetParent(null);

                // Adiciona para seguir com Update
                trails.Add(new TrailFollowData(trail.transform, dummy.transform));

                // Coroutine para manter dummy sincronizado
                StartCoroutine(FollowVisual(planetVisual, dummy.transform));
            }

            var projData = new ProjectileData(newProjectile,
                                              range: stats[weaponLevel].range,
                                              damage: stats[weaponLevel].damage,
                                              speed: stats[weaponLevel].speed,
                                              duration: Mathf.Infinity);

            activeProjectiles.Add(projData);
            newProjectile.transform.localScale = Vector3.one * projData.range;
        }
    }

    private IEnumerator FollowVisual(Transform target, Transform follower)
    {
        while (target != null && follower != null)
        {
            follower.position = target.position;
            follower.rotation = target.rotation;
            yield return null;
        }
    }

    public override void LevelUp()
    {
        if (weaponLevel < stats.Count - 1)
        {
            weaponLevel++;
            statsUpdated = true;

            foreach (var projData in activeProjectiles)
            {
                EnemyDamageOrbiting projDamage = projData.projectileObject.GetComponent<EnemyDamageOrbiting>();
                if (projDamage != null)
                {
                    projDamage.currentLevel = weaponLevel;
                    projDamage.UpdateSpriteForLevel();
                }
            }

            if (weaponLevel >= stats.Count - 1)
            {
                Player.instance.fullyLevelledWeapons.Add(this);
                Player.instance.assignedWeapons.Remove(this);
            }
        }
    }
}
