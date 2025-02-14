using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{


    public Rigidbody2D rb;
    public SpriteRenderer sprite;
    public float speed;
    public Animator anim;


    GameObject player;
    Vector2 dogDirection;

    public float movementTolerance;

    // Start is called before the first frame update
    void Start()
    {

        player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {


        dogDirection = player.transform.position - transform.position;

        if (dogDirection.x > 0)
        {
            sprite.flipX = false;
        }

        if (dogDirection.x < 0)
        {
            sprite.flipX = true;
        }

        if (dogDirection.sqrMagnitude > movementTolerance)
        {
            anim.SetBool("Walk", true);
        }
        else
        {
            anim.SetBool("Walk", false);
        }


    }
    private void FixedUpdate()
    {
        if (!anim.GetBool("Idle"))
        {
            rb.MovePosition(rb.position + dogDirection.normalized * speed * Time.deltaTime);
        }
            

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Se o pet colidir com o jogador, executar a animação "idle"
            anim.SetBool("Idle", true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Se o pet sair da colisão com o jogador, voltar à animação "walk"
            anim.SetBool("Idle", false);
        }
    }


}
