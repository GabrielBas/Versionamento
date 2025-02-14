using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float speed = 2f; // Velocidade de movimento do inimigo

    private Transform player; // Refer�ncia ao jogador
    private Rigidbody2D rb;
    private Vector2 movement;

    void Start()
    {
        player = Player.instance.transform; // Encontra o jogador na cena
        rb = GetComponent<Rigidbody2D>(); // Pega o Rigidbody2D do inimigo
    }

    void Update()
    {
        Vector2 direction = player.position - transform.position; // Calcula a dire��o para o jogador
        direction.Normalize();
        movement = direction; // Define a dire��o do movimento
    }

    private void FixedUpdate()
    {
        MoveEnemy();
    }

    void MoveEnemy()
    {
        rb.MovePosition((Vector2)transform.position + (movement * speed * Time.deltaTime)); // Move o inimigo em dire��o ao jogador
    }
}
