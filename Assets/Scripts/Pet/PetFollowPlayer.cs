using System.Collections;
using UnityEngine;

public class PetFollowPlayer : MonoBehaviour
{
    [Header("Configuração de Seguir")]
    public float followSpeed = 5f; // Velocidade de seguir o jogador
    public float followDistance = 2f; // Distância mínima para seguir o jogador
    public float movementTolerance = 0.1f;

    [Header("Componentes")]
    public Animator animator;
    public SpriteRenderer sprite;
    public Rigidbody2D rb;

    private Transform player; // Referência ao transform do jogador ativo
    private bool isCollidingWithPlayer = false; // Controle de estado de colisão
    private Vector2 petDirection;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

        // 🔹 Procura o Player ativo ao iniciar
        FindActivePlayer();
    }

    private void Update()
    {
        if (player == null)
        {
            FindActivePlayer();
            return;
        }

        if (isCollidingWithPlayer)
        {
            // Durante a colisão, força a animação Idle
            animator.SetBool("Idle", true);
            animator.SetBool("Walk", false);
            return; // Interrompe a execução de movimento e flip
        }

        // Calcula a direção em relação ao jogador
        petDirection = player.position - transform.position;

        // Controle de flip no eixo X
        sprite.flipX = petDirection.x < 0;

        // Atualiza o estado da animação "Walk" com base na distância ao jogador
        if (petDirection.sqrMagnitude > movementTolerance)
        {
            animator.SetBool("Walk", true);
            animator.SetBool("Idle", false);
        }
        else
        {
            animator.SetBool("Walk", false);
            animator.SetBool("Idle", true);
        }
    }

    private void FixedUpdate()
    {
        if (player == null) return;

        if (!isCollidingWithPlayer && animator.GetBool("Walk"))
        {
            rb.MovePosition(rb.position + petDirection.normalized * followSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isCollidingWithPlayer = true;
            animator.SetBool("Idle", true);
            animator.SetBool("Walk", false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isCollidingWithPlayer = false;
            animator.SetBool("Idle", false);
        }
    }

    /// <summary>
    /// Procura qual Player está ativo na cena.
    /// </summary>
    public void FindActivePlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject go in players)
        {
            if (go.activeInHierarchy) // 🔹 Pega o que está ativo
            {
                player = go.transform;
                break;
            }
        }
    }

    /// <summary>
    /// Permite setar manualmente o player (opcional).
    /// </summary>
    public void SetPlayer(Transform newPlayer)
    {
        player = newPlayer;
    }
}
