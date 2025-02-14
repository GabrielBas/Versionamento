using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombWeapon : Weapon
{
    public EnemyDamage damager;
    public GameObject weaponPrefab; // Prefab da arma a ser lançada

    private float throwCounter;

    // Start is called before the first frame update
    void Start()
    {
        SetStats();
    }

    // Update is called once per frame
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
                    // Se um prefab de arma estiver definido, instancie-o
                    GameObject thrownWeapon = Instantiate(weaponPrefab, transform.position, transform.rotation);
                    thrownWeapon.SetActive(true);
                }
                else
                {
                    // Caso contrário, use o comportamento padrão com damager
                    Instantiate(damager, damager.transform.position, damager.transform.rotation).gameObject.SetActive(true);
                }
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
