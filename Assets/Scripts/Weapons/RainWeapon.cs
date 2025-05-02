using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainWeapon : Weapon
{
    public EnemyDamage damager;
    public GameObject weaponPrefab; // Prefab da arma a ser lançada
    public GameObject daggerTrailPrefab; // <<< Novo: Prefab do trail visual

    private float throwCounter;

    private class TrailFollowData
    {
        public Transform trailObject;
        public Transform targetToFollow;

        public TrailFollowData(Transform trail, Transform target)
        {
            trailObject = trail;
            targetToFollow = target;
        }
    }

    private List<TrailFollowData> trails = new List<TrailFollowData>();

    void Start()
    {
        SetStats();
    }

    void Update()
    {
        if (statsUpdated == true)
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
                if (weaponPrefab != null)
                {
                    GameObject thrownWeapon = Instantiate(weaponPrefab, transform.position, transform.rotation);
                    thrownWeapon.SetActive(true);

                    // Instancia o trail e configura para seguir o objeto
                    if (daggerTrailPrefab != null)
                    {
                        GameObject trail = Instantiate(daggerTrailPrefab, thrownWeapon.transform.position, thrownWeapon.transform.rotation);
                        trails.Add(new TrailFollowData(trail.transform, thrownWeapon.transform));
                    }
                }
                else
                {
                    GameObject dmg = Instantiate(damager, damager.transform.position, damager.transform.rotation).gameObject;
                    dmg.SetActive(true);

                    // Instancia o trail para o damager se necessário
                    if (daggerTrailPrefab != null)
                    {
                        GameObject trail = Instantiate(daggerTrailPrefab, dmg.transform.position, dmg.transform.rotation);
                        trails.Add(new TrailFollowData(trail.transform, dmg.transform));
                    }
                }
            }
        }

        // Atualiza a posição dos trails
        for (int i = 0; i < trails.Count; i++)
        {
            if (trails[i].trailObject != null && trails[i].targetToFollow != null)
            {
                trails[i].trailObject.position = trails[i].targetToFollow.position;
                trails[i].trailObject.rotation = trails[i].targetToFollow.rotation;
            }
        }
    }

    void SetStats()
    {
        damager.damageAmount = stats[weaponLevel].damage;
        damager.lifeTime = stats[weaponLevel].duration;
        damager.transform.localScale = Vector3.one * stats[weaponLevel].range;
        throwCounter = 0f;
    }
}
