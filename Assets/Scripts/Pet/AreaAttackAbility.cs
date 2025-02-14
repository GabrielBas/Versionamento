using System.Collections;
using UnityEngine;

public class AreaAttackAbility : PetAbility
{
    public float damageAmount = 5f; // Dano por intervalo
    public float attackRadius = 5f; // Raio de ataque
    public float attackInterval = 1f; // Intervalo entre ataques

    public SpriteRenderer abilityIndicator; // Sprite para indicar a habilidade ativada

    private Coroutine attackRoutine;

    private void Start()
    {
        abilityName = "Area Attack";

        if (abilityIndicator != null)
        {
            abilityIndicator.gameObject.SetActive(false); // Certifique-se de que o sprite começa desativado
        }
    }

    public override void Activate()
    {
        if (attackRoutine == null) // Evitar múltiplas ativações
        {
            attackRoutine = StartCoroutine(AreaAttack());

            if (abilityIndicator != null)
            {
                abilityIndicator.gameObject.SetActive(true); // Ativa o sprite ao ativar a habilidade
            }
        }
    }


    private IEnumerator AreaAttack()
    {
        while (true)
        {
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRadius, LayerMask.GetMask("Enemy"));

            foreach (Collider2D enemy in hitEnemies)
            {
                enemy.GetComponent<Enemy>().TakeDamage(damageAmount);
            }

            yield return new WaitForSeconds(attackInterval);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }

}
