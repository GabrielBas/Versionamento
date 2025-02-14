using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float explosionDelay = 2f; // Tempo antes da bomba explodir
    public GameObject explosionEffect; // Efeito de explosão (opcional)
    public float explosionRadius = 5f; // Raio da explosão
    public float explosionDamage = 50f; // Dano da explosão (ajuste conforme necessário)

    private void Start()
    {
        // Aplica a rotação inicial de -90 graus no eixo Z
        transform.rotation = Quaternion.Euler(0f, 0f, -90f);

        // Inicia a contagem regressiva para a explosão
        StartCoroutine(ExplosionCountdown());
    }

    private IEnumerator ExplosionCountdown()
    {
        yield return new WaitForSeconds(explosionDelay);

        // Executa a explosão
        Explode();
    }

    private void Explode()
    {
        // Instancia o efeito de explosão se estiver definido
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // Dano aos inimigos próximos
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D enemy in hitEnemies)
        {
            // Supondo que os inimigos tenham um componente que pode receber dano
            var enemyHealth = enemy.GetComponent<Enemy>(); // Ajuste conforme necessário
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(explosionDamage);
            }
        }

        // Destrói a bomba após a explosão
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        // Visualiza o raio da explosão no editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
