using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponThrower : Weapon
{
    public EnemyDamage damager;
    private float throwCounter;

    [Header("Trail Settings")]
    public GameObject daggerTrailPrefab; // Prefab com TrailRenderer

    void Start()
    {
        SetStats();
    }

    void Update()
    {
        if (statsUpdated)
        {
            statsUpdated = false;
            SetStats();
        }

        throwCounter -= Time.deltaTime;
        if (throwCounter <= 0)
        {
            throwCounter = stats[weaponLevel].timeBetweenAttacks;

            for (int i = 0; i < stats[weaponLevel].amount; i++)
            {
                // Instancia o projétil (Axe)
                EnemyDamage newDamager = Instantiate(damager, damager.transform.position, damager.transform.rotation);
                newDamager.gameObject.SetActive(true);

                // Instancia o trail como objeto separado
                if (daggerTrailPrefab != null)
                {
                    GameObject trailInstance = Instantiate(daggerTrailPrefab, newDamager.transform.position, Quaternion.identity);

                    TrailRenderer trailRenderer = trailInstance.GetComponent<TrailRenderer>();
                    if (trailRenderer != null)
                    {
                        trailRenderer.emitting = true;
                    }

                    StartCoroutine(FollowProjectileUntilDestroyed(newDamager.transform, trailInstance.transform));
                }
            }
        }
    }

    IEnumerator FollowProjectileUntilDestroyed(Transform target, Transform trail)
    {
        // Enquanto o projétil existir, o trail o seguirá
        while (target != null)
        {
            trail.position = target.position;
            yield return null;
        }

        // Assim que o projétil for destruído, para de seguir, mas não destrói o trail
        yield break;
    }

    void SetStats()
    {
        damager.damageAmount = stats[weaponLevel].damage;
        damager.lifeTime = stats[weaponLevel].duration;
        damager.transform.localScale = Vector3.one * stats[weaponLevel].range;
        throwCounter = 0f;
    }
}
