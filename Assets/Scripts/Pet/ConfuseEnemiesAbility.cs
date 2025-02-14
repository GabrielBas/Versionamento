using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfuseEnemiesAbility : PetAbility
{
    public float confuseDuration = 5f; // Dura��o da confus�o
    public float confuseRadius = 5f; // Raio de efeito da habilidade
    public float confuseInterval = 10f; // Intervalo entre as ativa��es da habilidade
    private bool isAbilityActive = false; // Flag para verificar se a habilidade foi selecionada

    public SpriteRenderer abilityIndicator; // Sprite para indicar a habilidade ativada

    private void Start()
    {
        abilityName = "Confuse Enemies";

        if (abilityIndicator != null)
        {
            abilityIndicator.gameObject.SetActive(false); // Certifique-se de que o sprite come�a desativado
        }
    }

    public override void Activate()
    {
        if (!isAbilityActive) // Ativa a habilidade somente se n�o estiver ativa
        {
            isAbilityActive = true; // Marca como ativa

            if (abilityIndicator != null)
            {
                abilityIndicator.gameObject.SetActive(true); // Ativa o sprite ao ativar a habilidade
            }

            StartCoroutine(ConfuseEnemiesRoutine());
        }
    }

    private IEnumerator ConfuseEnemiesRoutine()
    {
        while (isAbilityActive) // Somente ativa quando a habilidade foi selecionada
        {
            yield return new WaitForSeconds(confuseInterval);
            ConfuseEnemies();
        }
    }

    private void ConfuseEnemies()
    {
        Debug.Log("ConfuseEnemies: Confundindo inimigos pr�ximos."); // Depura��o

        // Obt�m todos os inimigos dentro do raio
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, confuseRadius, LayerMask.GetMask("Enemy"));

        if (hitEnemies.Length == 0)
        {
            Debug.LogWarning("Nenhum inimigo encontrado no raio de confus�o."); // Depura��o
            return;
        }

        foreach (Collider2D enemy in hitEnemies)
        {
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.Confuse(confuseDuration); // Ativa o efeito de confus�o no inimigo
                Debug.Log($"ConfuseEnemies: Confundindo o inimigo {enemy.name}.");
            }
            else
            {
                Debug.LogWarning($"ConfuseEnemies: O objeto {enemy.name} n�o possui um script Enemy.");
            }
        }
    }

    // M�todo para visualizar o raio da habilidade no Editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, confuseRadius);
    }
}
