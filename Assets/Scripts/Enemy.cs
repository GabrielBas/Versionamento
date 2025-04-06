using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Animator animator;
    public Rigidbody2D rb;
    public SpriteRenderer sprite;
    public float speed;
    public float damage;
    public float hitWaitTime = 0.2f;
    private float hitCounter;

    public float health = 5f;

    // PetAbility
    public Color originalColor; // Armazena a cor original do inimigo

    private bool isConfused = false; // Se o inimigo est� confuso
    private float confuseCounter; // Contador de tempo para o efeito de confus�o

    public float knockBackTime = .5f;
    private float knockBackCounter;

    public int expToGive = 1;

    public int coinValue = 1;
    public float coinDropRate = .5f;

    private GameObject player; // O alvo padr�o � o jogador
    private GameObject currentTarget; // Alvo atual (jogador ou outro inimigo)
    private Vector2 enemyDirection;

    // Nome do trigger de ataque no Animator
    public string attackTriggerName = "Attack";

    public GameObject deathEffect;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        currentTarget = player; // Define o alvo inicial como o jogador

        originalColor = sprite.color; // Armazena a cor original do sprite
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTarget != null)
        {
            enemyDirection = currentTarget.transform.position - transform.position;

            if (enemyDirection.x > 0)
            {
                sprite.flipX = false;
            }
            else if (enemyDirection.x < 0)
            {
                sprite.flipX = true;
            }

            // Ativa a anima��o de caminhada quando o inimigo est� se movendo
            if (!isConfused && knockBackCounter <= 0)
            {
                animator.SetBool("IsWalking", true);
            }
            else
            {
                animator.SetBool("IsWalking", false);
            }
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }
    }

    private void FixedUpdate()
    {
        if (currentTarget != null && Player.instance.gameObject.activeSelf)
        {
            if (knockBackCounter > 0)
            {
                knockBackCounter -= Time.deltaTime;

                if (speed > 0)
                {
                    speed = -speed * 2f;
                }

                if (knockBackCounter <= 0)
                {
                    speed = Mathf.Abs(speed * .5f);
                }
            }

            rb.MovePosition(rb.position + enemyDirection.normalized * speed * Time.deltaTime);

            if (hitCounter > 0f)
            {
                hitCounter -= Time.deltaTime;
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && hitCounter <= 0f)
        {
            PlayerHealthController.instance.TakeDamage(damage);
            hitCounter = hitWaitTime;
        }
        else if (isConfused && collision.gameObject.CompareTag("Enemy") && hitCounter <= 0f)
        {
            Enemy enemyTarget = collision.gameObject.GetComponent<Enemy>();
            if (enemyTarget != null)
            {
                enemyTarget.TakeDamage(damage);
                AttackAnimation(); // Executa a anima��o de ataque
                hitCounter = hitWaitTime;
            }
        }
    }

    public void TakeDamage(float damageToTake)
    {
        health -= damageToTake;

        if (health <= 0f)
        {
            Destroy(gameObject);
            ExperienceLevelController.instance.SpawnExp(transform.position, expToGive);

            if (Random.value <= coinDropRate)
            {
                CoinController.instance.DropCoin(transform.position, coinValue);
            }
        }

        // Gera o efeito de morte
        Instantiate(deathEffect, transform.position, transform.rotation);

        DamageNumberController.instance.SpawnDamage(damageToTake, transform.position);
    }

    public void TakeDamage(float damageToTake, bool shouldKnockBack)
    {
        TakeDamage(damageToTake);

        if (shouldKnockBack)
        {
            knockBackCounter = knockBackTime;
        }
    }

    // Pet Ability: Diminui a velocidade do inimigo por um per�odo de tempo
    public void Slow(float duration, float slowPercentage, Color slowColor)
    {
        StartCoroutine(SlowCoroutine(duration, slowPercentage, slowColor));
    }

    private IEnumerator SlowCoroutine(float duration, float slowPercentage, Color slowColor)
    {
        float originalSpeed = speed; // Armazena a velocidade original
        speed *= (1 - slowPercentage); // Diminui a velocidade
        SetColor(slowColor); // Altera a cor do inimigo

        yield return new WaitForSeconds(duration); // Aguarda a dura��o do efeito

        speed = originalSpeed; // Restaura a velocidade original
        ResetColor(); // Restaura a cor original
    }

    // M�todos para alterar e restaurar a cor do inimigo
    public void SetColor(Color newColor)
    {
        sprite.color = newColor;
    }

    public void ResetColor()
    {
        sprite.color = originalColor;
    }

    // M�todo para confundir o inimigo e for��-lo a atacar outros inimigos
    public void Confuse(float duration)
    {
        if (!isConfused) // Se o inimigo j� est� confuso, n�o fa�a nada
        {
            StartCoroutine(ConfuseCoroutine(duration));
        }
    }

    private IEnumerator ConfuseCoroutine(float duration)
    {
        isConfused = true;
        confuseCounter = duration;

        Color originalColor = sprite.color; // Armazena a cor original
        sprite.color = Color.magenta; // Muda a cor para indicar confus�o

        // Seleciona outro inimigo como alvo
        Enemy closestEnemy = FindClosestEnemy();
        if (closestEnemy != null)
        {
            currentTarget = closestEnemy.gameObject; // Define o inimigo mais pr�ximo como alvo
        }

        while (confuseCounter > 0)
        {
            confuseCounter -= Time.deltaTime;

            yield return null;
        }

        // Restaura o comportamento normal
        isConfused = false;
        sprite.color = originalColor; // Restaura a cor original
        currentTarget = player; // Redefine o alvo como o jogador
    }

    // M�todo para encontrar o inimigo mais pr�ximo usando a Tag
    private Enemy FindClosestEnemy()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        Enemy closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (Enemy enemy in enemies)
        {
            if (enemy != this && enemy.CompareTag("Enemy")) // Considera apenas inimigos com a tag "Enemy"
            {
                float distance = Vector2.Distance(transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy;
                }
            }
        }

        return closestEnemy;
    }

    // M�todo para executar a anima��o de ataque
    private void AttackAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");
            animator.SetBool("isWalking", false); // Desativa a anima��o de caminhada durante o ataque
        }
    }
}
