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

    private bool isConfused = false; // Se o inimigo está confuso
    private float confuseCounter; // Contador de tempo para o efeito de confusão

    public float knockBackTime = .5f;
    private float knockBackCounter;

    public int expToGive = 1;

    public int coinValue = 1;
    public float coinDropRate = .5f;

    private GameObject player; // O alvo padrão é o jogador
    private GameObject currentTarget; // Alvo atual (jogador ou outro inimigo)
    private Vector2 enemyDirection;

    // Nome do trigger de ataque no Animator
    public string attackTriggerName = "Attack";

    public GameObject deathEffect;

    [Header("Som de dano")]
    public AudioClip hitSound;

    //[Header("Som de morte")]
    //public AudioClip deathSound;

    private AudioSource audioSource;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        currentTarget = player;

        originalColor = sprite.color;

        // 🔊 Inicializa AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
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

            // Ativa a animação de caminhada quando o inimigo está se movendo
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
                AttackAnimation(); // Executa a animação de ataque
                hitCounter = hitWaitTime;
            }
        }
    }

    public void TakeDamage(float damageToTake)
    {
        health -= damageToTake;

        GetComponent<EnemyColorEffect>()?.FlashWhite(); // Efeito visual de dano

        // 🔊 Toca som de impacto
        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        if (health <= 0f)
        {
            // 🔊 Melhor forma: garante que o som toque mesmo que o Enemy seja destruído
            //if (deathSound != null)
            //{
            //    AudioSource.PlayClipAtPoint(deathSound, transform.position);
            //}

            Instantiate(deathEffect, transform.position, transform.rotation);

            Destroy(gameObject);

            ExperienceLevelController.instance.SpawnExp(transform.position, expToGive);

            if (Random.value <= coinDropRate)
            {
                CoinController.instance.DropCoin(transform.position, coinValue);
            }
        }


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

    // Pet Ability: Diminui a velocidade do inimigo por um período de tempo
    public void Slow(float duration, float slowPercentage, Color slowColor)
    {
        StartCoroutine(SlowCoroutine(duration, slowPercentage, slowColor));
    }

    private IEnumerator SlowCoroutine(float duration, float slowPercentage, Color slowColor)
    {
        float originalSpeed = speed; // Armazena a velocidade original
        speed *= (1 - slowPercentage); // Diminui a velocidade
        SetColor(slowColor); // Altera a cor do inimigo

        yield return new WaitForSeconds(duration); // Aguarda a duração do efeito

        speed = originalSpeed; // Restaura a velocidade original
        ResetColor(); // Restaura a cor original
    }

    // Métodos para alterar e restaurar a cor do inimigo
    public void SetColor(Color newColor)
    {
        sprite.color = newColor;
    }

    public void ResetColor()
    {
        sprite.color = originalColor;
    }

    // Método para confundir o inimigo e forçá-lo a atacar outros inimigos
    public void Confuse(float duration)
    {
        if (!isConfused) // Se o inimigo já está confuso, não faça nada
        {
            StartCoroutine(ConfuseCoroutine(duration));
        }
    }

    private IEnumerator ConfuseCoroutine(float duration)
    {
        isConfused = true;
        confuseCounter = duration;

        Color originalColor = sprite.color; // Armazena a cor original
        sprite.color = Color.magenta; // Muda a cor para indicar confusão

        // Seleciona outro inimigo como alvo
        Enemy closestEnemy = FindClosestEnemy();
        if (closestEnemy != null)
        {
            currentTarget = closestEnemy.gameObject; // Define o inimigo mais próximo como alvo
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

    // Método para encontrar o inimigo mais próximo usando a Tag
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

    // Método para executar a animação de ataque
    private void AttackAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");
            animator.SetBool("isWalking", false); // Desativa a animação de caminhada durante o ataque
        }
    }
}
