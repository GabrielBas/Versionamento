using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthController : MonoBehaviour
{
    public static PlayerHealthController instance;

    private void Awake()
    {
        instance = this;
    }

    public float currentHealth;
    public float maxHealth;

    public Slider healthSlider;

    public GameObject deathEffect;

    public Button gameOverButton; // Botão para disparar o Game Over manualmente

    void Start()
    {
        maxHealth = PlayerStatController.instance.health[0].value;

        currentHealth = maxHealth;

        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;

        if (gameOverButton != null)
        {
            gameOverButton.onClick.AddListener(TriggerGameOver); // Associa o botão ao método TriggerGameOver
        }
    }

    public void TakeDamage(float damageToTake)
    {
        currentHealth -= damageToTake;

        if (currentHealth <= 0)
        {
            HandlePlayerDeath();
        }

        healthSlider.value = currentHealth; // Atualiza a barra de vida
    }

    public void Heal(float healAmount)
    {
        currentHealth += healAmount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        healthSlider.value = currentHealth; // Atualiza a barra de vida na UI
    }

    private void TriggerGameOver()
    {
        currentHealth = 0; // Reduz a saúde a 0
        HandlePlayerDeath(); // Lida com a morte do jogador
    }

    private void HandlePlayerDeath()
    {
        // Desanexar armas antes de desativar o Player
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Weapon")) // Certifique-se de que as armas tenham a tag "Weapon"
            {
                child.SetParent(null); // Remove a arma como filha do Player

                // Para o Trail Renderer, mas mantém o rastro
                TrailRenderer trail = child.GetComponentInChildren<TrailRenderer>();
                if (trail != null)
                {
                    trail.emitting = false; // Para de emitir sem apagar o rastro
                }
            }
        }

        // Desativa o jogador
        gameObject.SetActive(false);

        // Finaliza o nível
        LevelManager.instance.EndLevel();

        // Gera o efeito de morte
        Instantiate(deathEffect, transform.position, transform.rotation);
    }
}
