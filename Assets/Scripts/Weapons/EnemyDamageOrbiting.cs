using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageOrbiting : MonoBehaviour
{
    public float damageAmount;
    public float lifeTime = 99999f;
    public float growSpeed;
    private Vector3 targetSize;

    public bool sholdKnockBack;
    public bool destroyParent;
    public bool damageOverTime;
    public float timeBetweenDamage;
    public float damageCounter;
    private List<Enemy> enemiesInRange = new List<Enemy>();
    public bool destroyOnImpact;

    public int currentLevel;
    public Sprite[] levelSprites;

    private PlanetsWeapon parentWeapon;  // Referência ao script pai

    void Start()
    {
        targetSize = transform.localScale;
        transform.localScale = Vector3.zero;

        // Obtém referência ao objeto pai PlanetsWeapon
        parentWeapon = GetComponentInParent<PlanetsWeapon>();

        if (parentWeapon != null)
        {
            currentLevel = parentWeapon.weaponLevel;  // Sincroniza o nível
            UpdateSpriteForLevel();
        }
    }

    void Update()
    {
        transform.localScale = Vector3.MoveTowards(transform.localScale, targetSize, growSpeed * Time.deltaTime);

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
                        enemiesInRange[i].TakeDamage(damageAmount, sholdKnockBack);
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

    // Atualiza o sprite com base no nível específico do projétil
    public void UpdateSpriteForLevel()
    {
        if (currentLevel >= 0 && currentLevel < levelSprites.Length)
        {
            GetComponent<SpriteRenderer>().sprite = levelSprites[currentLevel];
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!damageOverTime && collision.tag == "Enemy")
        {
            collision.GetComponent<Enemy>().TakeDamage(damageAmount, sholdKnockBack);
            if (destroyOnImpact) Destroy(gameObject);
        }
        else if (collision.tag == "Enemy")
        {
            enemiesInRange.Add(collision.GetComponent<Enemy>());
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
