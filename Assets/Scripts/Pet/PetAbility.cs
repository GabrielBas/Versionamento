using UnityEngine;

public abstract class PetAbility : MonoBehaviour
{
    public string abilityName; // Nome da habilidade
    public Sprite icon; // �cone da habilidade

    // M�todo para ativar a habilidade
    public abstract void Activate();
}
