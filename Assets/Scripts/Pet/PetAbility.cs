using UnityEngine;

public abstract class PetAbility : MonoBehaviour
{
    public string abilityName; // Nome da habilidade
    public Sprite icon; // Ícone da habilidade

    // Método para ativar a habilidade
    public abstract void Activate();
}
