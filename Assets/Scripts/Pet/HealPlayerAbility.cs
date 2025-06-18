using System.Collections;
using UnityEngine;

public class HealPlayerAbility : PetAbility
{
    public float healAmount = 2f; // Quantidade de cura por intervalo
    public float healInterval = 5f; // Intervalo entre as curas

    public GameObject healTextPrefab; // Prefab do texto flutuante

    private void Start()
    {
        abilityName = "Heal Player";
    }

    public override void Activate()
    {
        StartCoroutine(HealPlayer());
    }

    private IEnumerator HealPlayer()
    {
        while (true)
        {
            PlayerHealthController.instance.Heal(healAmount);

            // Instancia o texto flutuante na posição do jogador
            if (healTextPrefab != null)
            {
                Vector3 spawnPosition = PlayerHealthController.instance.transform.position + new Vector3(Random.Range(-0.5f, 0.5f), 1.5f, 0f);
                GameObject healText = Instantiate(healTextPrefab, spawnPosition, Quaternion.identity);

                // Define o texto para exibir a quantidade de cura
                FloatingText floatingText = healText.GetComponent<FloatingText>();
                if (floatingText != null)
                {
                    floatingText.SetText("+" + healAmount.ToString("F0"));
                }
            }

            yield return new WaitForSeconds(healInterval);
        }
    }
}
