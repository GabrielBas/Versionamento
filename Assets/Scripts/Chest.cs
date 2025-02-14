using System.Collections;
using UnityEngine;

public class Chest : MonoBehaviour
{
    // Características existentes
    public float followSpeed = 3f; // Velocidade de seguimento
    private Transform playerTransform;
    private bool isFollowing = false;
    public Animator animator;

    public Rigidbody2D rb;
    public SpriteRenderer sprite;

    public float health = 5f;

    public GameObject deathEffect;

    public bool shouldKnockBack; // Se deve aplicar knockback
    public float knockBackTime = .5f;
    private float knockBackCounter;

    // Características adicionadas do Enemy
    public float damage = 1f; // Dano base do baú
    public float hitWaitTime = 0.2f; // Tempo entre ataques
    private float hitCounter; // Contador de tempo entre ataques

    public int expToGive = 1; // Experiência dada ao destruir o baú
    public int coinValue = 1; // Valor da moeda
    public float coinDropRate = 0.5f; // Chance de dropar moedas

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (animator == null)
        {
            Debug.LogWarning("Nenhum componente Animator atribuído!");
        }
    }

    private void Update()
    {
        // Seguir o jogador se ativado
        if (isFollowing && playerTransform != null)
        {
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            transform.position += direction * followSpeed * Time.deltaTime;

            FlipSprite(direction);

            if (animator != null)
            {
                animator.SetBool("isWalking", true);
            }
        }
        else
        {
            if (animator != null)
            {
                animator.SetBool("isWalking", false);
            }
        }

        // Reduzir o contador de intervalo entre ataques
        if (hitCounter > 0f)
        {
            hitCounter -= Time.deltaTime;
        }
    }

    private void FlipSprite(Vector3 direction)
    {
        if (sprite != null)
        {
            // Verifica se deve virar o sprite horizontalmente ou verticalmente
            if (direction.x > 0) // Inverte no eixo X se o jogador estiver à esquerda
            {
                sprite.flipX = false;
            }
            else if (direction.x < 0)
            {
                sprite.flipX = true;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isFollowing = true; // Inicia o seguimento do jogador

            // Aplica dano ao jogador, respeitando o intervalo de ataques
            if (hitCounter <= 0f)
            {
                PlayerHealthController.instance.TakeDamage(damage);
                hitCounter = hitWaitTime; // Reseta o contador
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            animator.SetTrigger("Attack");
            animator.SetBool("isWalking", false);
        }
        else
        {
            animator.SetBool("isWalking", true);
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
}
