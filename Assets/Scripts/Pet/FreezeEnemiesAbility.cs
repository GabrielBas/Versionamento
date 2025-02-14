using System.Collections;
using UnityEngine;

public class FreezeEnemiesAbility : PetAbility
{
    public float freezeDuration = 3f; // Duração do efeito de congelamento
    public float freezeRadius = 5f; // Raio de congelamento
    public float freezeSlowPercentage = 0.5f; // Percentual de redução de velocidade (50%)
    public float effectInterval = 10f; // Intervalo entre as ativações do efeito (em segundos)
    public Color freezeColor = Color.cyan; // Cor dos inimigos quando estiverem "congelados"

    public SpriteRenderer abilityIndicator; // Sprite para indicar a habilidade ativada

    private Coroutine freezeCoroutine; // Armazena a coroutine para controle

    private void Start()
    {
        abilityName = "Freeze Enemies"; // Define o nome da habilidade

        if (abilityIndicator != null)
        {
            abilityIndicator.gameObject.SetActive(false); // Certifique-se de que o sprite começa desativado
        }
    }

    public override void Activate()
    {
        // Inicia a coroutine que aplica o efeito a cada x segundos
        if (freezeCoroutine == null)
        {
            freezeCoroutine = StartCoroutine(FreezeEnemiesPeriodically());

            if (abilityIndicator != null)
            {
                abilityIndicator.gameObject.SetActive(true); // Ativa o sprite ao ativar a habilidade
            }
        }
    }


    private IEnumerator FreezeEnemiesPeriodically()
    {
        while (true)
        {
            // Congela os inimigos próximos
            FreezeEnemies();

            // Espera pelo intervalo de tempo antes de aplicar o efeito novamente
            yield return new WaitForSeconds(effectInterval);
        }
    }

    private void FreezeEnemies()
    {
        // Encontra todos os inimigos dentro do raio de congelamento
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, freezeRadius, LayerMask.GetMask("Enemy"));

        foreach (Collider2D enemy in hitEnemies)
        {
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                // Aplica o efeito de redução de velocidade
                enemyScript.Slow(freezeDuration, freezeSlowPercentage, freezeColor);
            }
        }
    }

    // Método para visualizar o raio da habilidade no Editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, freezeRadius);
    }

}
