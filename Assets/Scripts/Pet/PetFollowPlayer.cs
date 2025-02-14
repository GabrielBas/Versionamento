using System.Collections;
using UnityEngine;

public class PetFollowPlayer : MonoBehaviour
{
    public Transform player; // Referência ao transform do jogador
    public float followSpeed = 5f; // Velocidade de seguir o jogador
    public float followDistance = 2f; // Distância mínima para seguir o jogador

    private GameObject playerObject;
    public Animator animator;
    public SpriteRenderer sprite;
    public Rigidbody2D rb;

    Vector2 petDirection;
    public float movementTolerance;

    private bool isCollidingWithPlayer = false; // Controle de estado de colisão

    private void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
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
        if (!isCollidingWithPlayer && animator.GetBool("Walk")) // Move apenas se não estiver colidindo e estiver caminhando
        {
            rb.MovePosition(rb.position + petDirection.normalized * followSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isCollidingWithPlayer = true; // Marca como em colisão
            animator.SetBool("Idle", true); // Força a animação Idle
            animator.SetBool("Walk", false); // Garante que a animação de caminhada seja desativada
            //Debug.Log("OnTriggerEnter");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isCollidingWithPlayer = false; // Sai do estado de colisão
            animator.SetBool("Idle", false); // Retorna ao comportamento normal
            //Debug.Log("OnTriggerExit");
        }
    }
}
