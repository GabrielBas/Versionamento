using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public float damageAmount;

    public float lifeTime, growSpeed = 4f;
    private Vector3 targetSize;

    public bool sholdKnockBack;

    public bool destroyParent;

    public bool damageOverTime;
    public float timeBetweenDamage;
    public float damageCounter;

    private List<Enemy> enemiesInRange = new List<Enemy>();

    public bool destroyOnImpact;

    void Start()
    {
        targetSize = transform.localScale;
        transform.localScale = Vector3.zero;
    }

    void Update()
    {
        transform.localScale = Vector3.MoveTowards(transform.localScale, targetSize, growSpeed * Time.deltaTime);

        lifeTime -= Time.deltaTime;

        if (lifeTime <= 0)
        {
            DestroyObject();
        }

        if (damageOverTime)
        {
            damageCounter -= Time.deltaTime;

            if (damageCounter <= 0)
            {
                damageCounter = timeBetweenDamage;

                for (int i = 0; i < enemiesInRange.Count; i++)
                {
                    if (enemiesInRange[i] != null)
                    {
                        float finalDamage = damageAmount * BerserkBuffAbility.GlobalDamageMultiplier;
                        enemiesInRange[i].TakeDamage(finalDamage, sholdKnockBack);
                    }
                    else
                    {
                        enemiesInRange.RemoveAt(i);
                        i--;
                    }
                }
            }
        }
    }

    private void DestroyObject()
    {
        // Destroi o pai se `destroyParent` for verdadeiro
        if (destroyParent && transform.parent != null)
        {
            Destroy(transform.parent.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!damageOverTime)
        {
            if (collision.tag == "Enemy")
            {
                float finalDamage = damageAmount * BerserkBuffAbility.GlobalDamageMultiplier;
                collision.GetComponent<Enemy>().TakeDamage(finalDamage, sholdKnockBack);

                if (destroyOnImpact)
                {
                    DestroyObject();
                }
            }
            else if (collision.tag == "Chest")
            {
                float finalDamage = damageAmount * BerserkBuffAbility.GlobalDamageMultiplier;
                collision.GetComponent<Chest>().TakeDamage(finalDamage, sholdKnockBack);

                if (destroyOnImpact)
                {
                    DestroyObject();
                }
            }
        }
        else
        {
            if (collision.tag == "Enemy")
            {
                enemiesInRange.Add(collision.GetComponent<Enemy>());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (damageOverTime && collision.tag == "Enemy")
        {
            enemiesInRange.Remove(collision.GetComponent<Enemy>());
        }
    }

}
