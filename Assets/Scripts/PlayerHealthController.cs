using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

    public Button gameOverButton;
    public GameObject confirmationPanel;
    public GameObject pauseScreen;
    public Button yesButton;
    public Button noButton;
    public Button resumeButton;

    public GameObject levelEndFirstButton;

    void Start()
    {
        maxHealth = PlayerStatController.instance.health[0].value;
        currentHealth = maxHealth;

        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;

        if (gameOverButton != null)
            gameOverButton.onClick.AddListener(OpenConfirmationPanel);

        if (yesButton != null)
            yesButton.onClick.AddListener(ConfirmGameOver);

        if (noButton != null)
            noButton.onClick.AddListener(CancelGameOver);

        if (confirmationPanel != null)
            confirmationPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            CancelGameOver();
    }

    public void TakeDamage(float damageToTake)
    {
        currentHealth -= damageToTake;

        if (currentHealth <= 0)
            HandlePlayerDeath();

        healthSlider.value = currentHealth;
    }

    public void Heal(float healAmount)
    {
        currentHealth += healAmount;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        healthSlider.value = currentHealth;
    }

    private void OpenConfirmationPanel()
    {
        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(true);
            Time.timeScale = 0f;

            if (pauseScreen != null)
                pauseScreen.SetActive(false);

            EventSystem.current.SetSelectedGameObject(null);

            if (noButton != null)
                EventSystem.current.SetSelectedGameObject(noButton.gameObject);
        }
    }

    private void ConfirmGameOver()
    {
        UIController.instance.BlockPause(); // ⛔ Bloqueia o pause
        currentHealth = 0;
        confirmationPanel.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);

        if (pauseScreen != null)
            pauseScreen.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(levelEndFirstButton);

        // 🔁 Em vez de rodar coroutine aqui, chamamos pela UIController
        UIController.instance.StartCoroutine(UIController.instance.DelayedHandleDeath());
    }

    private void CancelGameOver()
    {
        if (confirmationPanel != null)
            confirmationPanel.SetActive(false);

        if (pauseScreen != null)
            pauseScreen.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);

        if (resumeButton != null)
            EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);
    }

    public void HandlePlayerDeath()
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Weapon"))
            {
                child.SetParent(null);
                TrailRenderer trail = child.GetComponentInChildren<TrailRenderer>();
                if (trail != null)
                    trail.emitting = false;
            }
        }

        gameObject.SetActive(false);

        LevelManager.instance.EndLevel();

        Instantiate(deathEffect, transform.position, transform.rotation);
    }
}
