using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;  // Importa o namespace TextMeshPro

public class PetAbilityManager : MonoBehaviour
{
    public static PetAbilityManager instance;

    [Header("Habilidades do Pet")]
    public List<PetAbility> availableAbilities; // Lista de habilidades dispon�veis
    private PetAbility selectedAbility; // Habilidade atualmente selecionada

    [Header("UI da Habilidade Ativa")]
    public Image activeAbilityIcon; // �cone da habilidade ativa na UI

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        AssignAndActivateAbility(); // Atribui e ativa a habilidade automaticamente
    }

    private void AssignAndActivateAbility()
    {
        if (availableAbilities.Count == 0)
        {
            Debug.LogWarning("Nenhuma habilidade dispon�vel para o pet!");
            return;
        }

        // Seleciona uma habilidade aleat�ria para o pet
        selectedAbility = availableAbilities[Random.Range(0, availableAbilities.Count)];
        selectedAbility.Activate(); // Ativa a habilidade automaticamente

        // Atualiza o �cone na UI
        if (activeAbilityIcon != null)
        {
            activeAbilityIcon.sprite = selectedAbility.icon;
            activeAbilityIcon.enabled = true;
        }

        Debug.Log($"Habilidade atribu�da ao pet: {selectedAbility.abilityName}");
    }
}
