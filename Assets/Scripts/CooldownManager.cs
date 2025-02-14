using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CooldownManager : MonoBehaviour
{
    public static CooldownManager instance;

    private Dictionary<string, Coroutine> activeCooldowns = new Dictionary<string, Coroutine>();

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

    // Inicia o cooldown para um ID específico
    public void StartCooldown(string cooldownID, float cooldownTime)
    {
        if (activeCooldowns.ContainsKey(cooldownID))
        {
            Debug.LogWarning("Cooldown já em andamento para o ID: " + cooldownID);
            return;
        }

        Coroutine cooldownRoutine = StartCoroutine(CooldownRoutine(cooldownID, cooldownTime));
        activeCooldowns[cooldownID] = cooldownRoutine;
    }

    // Verifica se o ID específico está em cooldown
    public bool IsOnCooldown(string cooldownID)
    {
        return activeCooldowns.ContainsKey(cooldownID);
    }

    // Coroutine que lida com o cooldown
    private IEnumerator CooldownRoutine(string cooldownID, float cooldownTime)
    {
        yield return new WaitForSeconds(cooldownTime);

        activeCooldowns.Remove(cooldownID);
        Debug.Log("Cooldown terminado para o ID: " + cooldownID);
    }

    // Cancela o cooldown para um ID específico, se necessário
    public void CancelCooldown(string cooldownID)
    {
        if (activeCooldowns.ContainsKey(cooldownID))
        {
            StopCoroutine(activeCooldowns[cooldownID]);
            activeCooldowns.Remove(cooldownID);
            Debug.Log("Cooldown cancelado para o ID: " + cooldownID);
        }
    }
}
