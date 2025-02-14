using System.Collections;
using UnityEngine;

public class HealPlayerAbility : PetAbility
{
    public float healAmount = 2f; // Quantidade de cura por intervalo
    public float healInterval = 5f; // Intervalo entre as curas

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
            PlayerHealthController.instance.Heal(healAmount); // Cura o jogador
            yield return new WaitForSeconds(healInterval);
        }
    }
}
