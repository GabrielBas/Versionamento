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

    public GameObject pauseFirstButton;
    public GameObject levelEndFirstButton;

    private InputAction pauseAction;

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

    private void Awake()
    {
        instance = this;

        pauseAction = new InputAction(type: InputActionType.Button, binding: "<Gamepad>/start");
        pauseAction.Enable();
        pauseAction.performed += PauseAction_performed;
    }

    private void OnDestroy()
    {
        pauseAction.performed -= PauseAction_performed;
        pauseAction.Disable();
    }

    private void PauseAction_performed(InputAction.CallbackContext context)
    {
        PauseUnpause();
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            PauseUnpause();
        }
    }

    public void PauseUnpause()
    {
        // ✅ Impede abrir pause se estiver no level end
        if (levelEndScreen.activeSelf) return;

        if (!pauseScreen.activeSelf)
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
            if (!levelUpPanel.activeSelf)
            {
                Time.timeScale = 1f;
                Player.SetActive(true);
            }

            EventSystem.current.SetSelectedGameObject(null);
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
        pauseAction.Enable(); // reativa o pause ao sair do levelEnd
        SceneManager.LoadScene(0);
        Time.timeScale = 1f;
    }

    public void Restart()
    {
        pauseAction.Enable(); // reativa o pause ao reiniciar
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // ✅ Correção: ativa gamepad e cursor corretamente
    public void ShowLevelEndScreen(float finalTime)
    {
        levelEndScreen.SetActive(true);
        endTimeText.text = "Time: " + Mathf.FloorToInt(finalTime / 60f) + ":" + Mathf.FloorToInt(finalTime % 60f).ToString("00");

        // Detecta mouse
        bool isMouse = Mouse.current != null && Mouse.current.wasUpdatedThisFrame;

        Cursor.visible = isMouse;
        Cursor.lockState = isMouse ? CursorLockMode.None : CursorLockMode.Locked;

        // Desativa pausa enquanto está no painel de fim
        pauseAction.Disable();

        StartCoroutine(SelectLevelEndButtonNextFrame());
    }

    private IEnumerator SelectLevelEndButtonNextFrame()
{
    yield return null;

    if (levelEndFirstButton != null && levelEndFirstButton.activeInHierarchy)
    {
        // 🔁 Reset seleção
        EventSystem.current.SetSelectedGameObject(null);
        yield return null;

        // ✅ Força foco visual + navegação
        Button button = levelEndFirstButton.GetComponent<Button>();
        if (button != null)
        {
            button.Select(); // força foco visual
        }

        EventSystem.current.SetSelectedGameObject(levelEndFirstButton);

        Debug.Log("Selecionado: " + EventSystem.current.currentSelectedGameObject?.name);

        // ✅ Garante pareamento com controle
        if (Gamepad.current != null)
        {
            InputUser.PerformPairingWithDevice(Gamepad.current);
            InputSystem.Update();
        }
    }
}


}
