using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitingWeapon : Weapon
{
    public float rotateSpeed;
    public Transform holder, fireballToSpawn;
    public EnemyDamageOrbiting damagePrefab;  // Alterado para EnemyDamageOrbiting para acessar currentLevel
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

    private List<ProjectileData> activeProjectiles = new List<ProjectileData>();

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

        if (statsUpdated == true)
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

        // Ajusta a posição do holder com base na distância definida no nível atual
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

            // Define currentLevel para o projétil específico
            EnemyDamageOrbiting projDamage = newProjectile.GetComponent<EnemyDamageOrbiting>();
            if (projDamage != null)
            {
                projDamage.currentLevel = weaponLevel;
                projDamage.UpdateSpriteForLevel();  // Atualiza sprite de acordo com o nível
                projDamage.damageAmount = stats[weaponLevel].damage;
                projDamage.lifeTime = Mathf.Infinity;
            }

            var projData = new ProjectileData(newProjectile, range: stats[weaponLevel].range,
                                              damage: stats[weaponLevel].damage, speed: stats[weaponLevel].speed,
                                              duration: Mathf.Infinity);

            activeProjectiles.Add(projData);
            newProjectile.transform.localScale = Vector3.one * projData.range;
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
