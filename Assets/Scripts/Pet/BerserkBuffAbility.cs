using System.Collections;
using UnityEngine;

public class BerserkBuffAbility : PetAbility
{
    [Header("Buff Settings")]
    public float buffDuration = 10f; // Duração do buff
    public float damageMultiplier = 1.5f; // Multiplicador de dano
    public float cooldown = 15f; // Tempo total de cooldown

    [Header("Visuals")]
    public SpriteRenderer buffIndicator; // Sprite para indicar o buff ativo

    public static float GlobalDamageMultiplier = 1f; // Multiplicador de dano global (1 = normal)

    private void Start()
    {
        abilityName = "Berserk Buff";

        if (buffIndicator != null)
        {
            buffIndicator.gameObject.SetActive(false); // Certifique-se de que o sprite começa desativado
        }

        
    }


    public override void Activate()
    {
        
       
        StartCoroutine(BuffCycle());
       
    }


    private IEnumerator BuffCycle()
    {
        while (true) // Ciclo infinito
        {
            // Ativar o buff
            GlobalDamageMultiplier = damageMultiplier;

            if (buffIndicator != null)
            {
                buffIndicator.gameObject.SetActive(true); // Ativa o indicador visual
            }

            Debug.Log("Berserk Buff ativado!");
            yield return new WaitForSeconds(buffDuration); // Aguarda a duração do buff

            // Desativar o buff
            GlobalDamageMultiplier = 1f;

            if (buffIndicator != null)
            {
                buffIndicator.gameObject.SetActive(false); // Desativa o indicador visual
            }

            Debug.Log("Berserk Buff desativado. Cooldown iniciado.");
            yield return new WaitForSeconds(cooldown); // Aguarda o cooldown
        }
    }
}



