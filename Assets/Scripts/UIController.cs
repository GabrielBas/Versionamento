using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    public GameObject Player;

    public GameObject pauseFirstButton; // Botão inicial no pause
    public GameObject levelEndFirstButton; // 🔴 Botão inicial no levelEndScreen

    private InputAction pauseAction;

    private void Awake()
    {
        instance = this;

        pauseAction = new InputAction(type: InputActionType.Button, binding: "<Gamepad>/start");
        pauseAction.Enable();
        pauseAction.performed += ctx => PauseUnpause();
    }

    private void OnDestroy()
    {
        pauseAction.Disable();
        pauseAction.performed -= ctx => PauseUnpause();
    }

    public Slider explvlSlider;
    public TMP_Text expLvlText;

    public LevelUpSelectionButton[] levelUpButtons;
    public GameObject levelUpPanel;

    public TMP_Text coinText;

    public PlayerStatUpgradeDisplay moveSpeedUpgradeDisplay, healthUpgradeDisplay, pickupRangeUpgradeDisplay, maxWeaponsUpgradeDisplay;

    public TMP_Text timeText;

    public GameObject levelEndScreen;
    public TMP_Text endTimeText;

    public GameObject pauseScreen;

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            PauseUnpause();
        }
    }

    public void UpdateExperience(int currentExp, int levelExp, int currentlvl)
    {
        explvlSlider.maxValue = levelExp;
        explvlSlider.value = currentExp;
        expLvlText.text = currentlvl.ToString();
    }

    public void SkipLevelUp()
    {
        levelUpPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void UpdateCoins()
    {
        coinText.text = " " + CoinController.instance.currentCoins;
    }

    public void PurchaseMoveSpeed()
    {
        PlayerStatController.instance.PurchaseMoveSpeed();
        SkipLevelUp();
    }

    public void PurchaseHealth()
    {
        PlayerStatController.instance.PurchaseHealth();
        SkipLevelUp();
    }

    public void PurchasePickupRange()
    {
        PlayerStatController.instance.PurchasePickupRange();
        SkipLevelUp();
    }

    public void PurchaseMaxWeapons()
    {
        PlayerStatController.instance.PurchaseMaxWeapons();
        SkipLevelUp();
    }

    public void UpdateTimer(float time)
    {
        float minutes = Mathf.FloorToInt(time / 60f);
        float seconds = Mathf.FloorToInt(time % 60);
        timeText.text = "Time: " + minutes + ":" + seconds.ToString("00");
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1f;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PauseUnpause()
    {
        if (pauseScreen.activeSelf == false)
        {
            pauseScreen.SetActive(true);
            Time.timeScale = 0f;
            Player.SetActive(false);

            if (pauseFirstButton != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(pauseFirstButton);
            }
        }
        else
        {
            pauseScreen.SetActive(false);
            if (levelUpPanel.activeSelf == false)
            {
                Time.timeScale = 1f;
                Player.SetActive(true);
            }

            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    // ✅ Corrigido com Coroutine
    public void ShowLevelEndScreen(float finalTime)
    {
        levelEndScreen.SetActive(true);
        endTimeText.text = "Time: " + Mathf.FloorToInt(finalTime / 60f) + ":" + Mathf.FloorToInt(finalTime % 60f).ToString("00");

        // 🔁 Força exibição do cursor se foi teclado/mouse
        if (Mouse.current != null && Mouse.current.wasUpdatedThisFrame)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        StartCoroutine(SelectLevelEndButtonNextFrame());
    }

    private IEnumerator SelectLevelEndButtonNextFrame()
    {
        yield return null;

        if (levelEndFirstButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(levelEndFirstButton);
        }
    }
}
