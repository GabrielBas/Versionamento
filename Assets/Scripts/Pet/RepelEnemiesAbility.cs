using System.Collections;
using UnityEngine;

public class RepelEnemiesAbility : PetAbility
{
    public float repelForce = 5f; // For�a da repuls�o
    public float repelRadius = 5f; // Raio de efeito da habilidade
    public float repelInterval = 2f; // Intervalo entre as repuls�es

    private void Start()
    {
        abilityName = "Repel Enemies";
        StartCoroutine(RepelEnemiesRoutine());
    }

    public override void Activate()
    {
        // A habilidade � ativada automaticamente no in�cio
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
        Debug.Log("RepelEnemies: Ativando repuls�o em inimigos pr�ximos."); // Depura��o

        // Obt�m todos os inimigos dentro do raio
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, repelRadius, LayerMask.GetMask("Enemy"));

        if (hitEnemies.Length == 0)
        {
            Debug.LogWarning("Nenhum inimigo encontrado no raio de repuls�o."); // Depura��o
            return;
        }

        foreach (Collider2D enemy in hitEnemies)
        {
            Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 repelDirection = (enemy.transform.position - transform.position).normalized;
                rb.AddForce(repelDirection * repelForce, ForceMode2D.Impulse);

                // Depura��o: Mostra mensagem no console para cada inimigo repelido
                Debug.Log($"RepelEnemies: Aplicando repuls�o ao inimigo {enemy.name}.");
            }
            else
            {
                // Depura��o: informa se um inimigo n�o tem Rigidbody2D
                Debug.LogWarning($"RepelEnemies: O inimigo {enemy.name} n�o tem um Rigidbody2D.");
            }
        }
    }

    // M�todo para visualizar o raio da habilidade no Editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, repelRadius);
    }
}
