using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EnemyGrayscale : MonoBehaviour
{
    public Animator animator;
    public Rigidbody2D rb;
    public SpriteRenderer sprite;
    public float speed;
    public float damage;
    public float grayscaleDuration = 5f; // Duração do efeito preto e branco
    private GameObject player;
    private Vector2 enemyDirection;

    public string attackTriggerName = "Attack";

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (player != null)
        {
            enemyDirection = player.transform.position - transform.position;

            if (enemyDirection.x > 0) sprite.flipX = false;
            else if (enemyDirection.x < 0) sprite.flipX = true;

            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealthController.instance.TakeDamage(damage);
            AttackAnimation();
            StartCoroutine(TriggerGrayscaleEffect());
        }
    }

    private IEnumerator TriggerGrayscaleEffect()
    {
        GrayscaleEffectController.Instance.SetGrayscale(true);
        yield return new WaitForSeconds(grayscaleDuration);
        GrayscaleEffectController.Instance.SetGrayscale(false);
    }

    private void AttackAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");
            animator.SetBool("isWalking", false);
        }
    }
}

