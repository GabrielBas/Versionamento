using System.Collections;
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
    public GameObject confirmationPanel; // Painel de confirmação
    public Button yesButton; // Botão "Sim"
    public Button noButton;  // Botão "Não"

    void Start()
    {
        maxHealth = PlayerStatController.instance.health[0].value;

        currentHealth = maxHealth;

        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;

        if (gameOverButton != null)
        {
            gameOverButton.onClick.AddListener(OpenConfirmationPanel);
        }

        if (yesButton != null)
        {
            yesButton.onClick.AddListener(ConfirmGameOver);
        }

        if (noButton != null)
        {
            noButton.onClick.AddListener(CancelGameOver);
        }

        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(false);
        }
    }

    public void TakeDamage(float damageToTake)
    {
        currentHealth -= damageToTake;

        if (currentHealth <= 0)
        {
            HandlePlayerDeath();
        }

        healthSlider.value = currentHealth;
    }

    public void Heal(float healAmount)
    {
        currentHealth += healAmount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        healthSlider.value = currentHealth;
    }

    private void OpenConfirmationPanel()
    {
        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(true);
            Time.timeScale = 0f; // Pausa o jogo
        }
    }

    private void ConfirmGameOver()
    {
        
        currentHealth = 0;
        confirmationPanel.SetActive(false);
        HandlePlayerDeath();
    }

    private void CancelGameOver()
    {
        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(false);
            
        }
    }

    private void HandlePlayerDeath()
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Weapon"))
            {
                child.SetParent(null);
                TrailRenderer trail = child.GetComponentInChildren<TrailRenderer>();
                if (trail != null)
                {
                    trail.emitting = false;
                }
            }
        }

        gameObject.SetActive(false);

        LevelManager.instance.EndLevel();

        Instantiate(deathEffect, transform.position, transform.rotation);
    }
}
