using System.Collections;
using UnityEngine;

public class RepelEnemiesAbility : PetAbility
{
    public float repelForce = 5f; // Força da repulsão
    public float repelRadius = 5f; // Raio de efeito da habilidade
    public float repelInterval = 2f; // Intervalo entre as repulsões

    private void Start()
    {
        abilityName = "Repel Enemies";
        StartCoroutine(RepelEnemiesRoutine());
    }

    public override void Activate()
    {
        // A habilidade é ativada automaticamente no início
    }

    private IEnumerator RepelEnemiesRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(repelInterval);
            RepelEnemies();
        }
    }

    private void RepelEnemies()
    {
        Debug.Log("RepelEnemies: Ativando repulsão em inimigos próximos."); // Depuração

        // Obtém todos os inimigos dentro do raio
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, repelRadius, LayerMask.GetMask("Enemy"));

        if (hitEnemies.Length == 0)
        {
            Debug.LogWarning("Nenhum inimigo encontrado no raio de repulsão."); // Depuração
            return;
        }

        foreach (Collider2D enemy in hitEnemies)
        {
            Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 repelDirection = (enemy.transform.position - transform.position).normalized;
                rb.AddForce(repelDirection * repelForce, ForceMode2D.Impulse);

                // Depuração: Mostra mensagem no console para cada inimigo repelido
                Debug.Log($"RepelEnemies: Aplicando repulsão ao inimigo {enemy.name}.");
            }
            else
            {
                // Depuração: informa se um inimigo não tem Rigidbody2D
                Debug.LogWarning($"RepelEnemies: O inimigo {enemy.name} não tem um Rigidbody2D.");
            }
        }
    }

    // Método para visualizar o raio da habilidade no Editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, repelRadius);
    }
}
