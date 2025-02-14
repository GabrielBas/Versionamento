using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float explosionDelay = 2f; // Tempo antes da bomba explodir
    public GameObject explosionEffect; // Efeito de explos�o (opcional)
    public float explosionRadius = 5f; // Raio da explos�o
    public float explosionDamage = 50f; // Dano da explos�o (ajuste conforme necess�rio)

    private void Start()
    {
        // Aplica a rota��o inicial de -90 graus no eixo Z
        transform.rotation = Quaternion.Euler(0f, 0f, -90f);

        // Inicia a contagem regressiva para a explos�o
        StartCoroutine(ExplosionCountdown());
    }

    private IEnumerator ExplosionCountdown()
    {
        yield return new WaitForSeconds(explosionDelay);

        // Executa a explos�o
        Explode();
    }

    private void Explode()
    {
        // Instancia o efeito de explos�o se estiver definido
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // Dano aos inimigos pr�ximos
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D enemy in hitEnemies)
        {
            // Supondo que os inimigos tenham um componente que pode receber dano
            var enemyHealth = enemy.GetComponent<Enemy>(); // Ajuste conforme necess�rio
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(explosionDamage);
            }
        }

        // Destr�i a bomba ap�s a explos�o
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        // Visualiza o raio da explos�o no editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
