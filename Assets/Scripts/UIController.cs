using System.Collections;
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
    private PlayerInput playerInput; // ✅ Referência separada

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

        // ✅ Pega referência do PlayerInput antes de desativar o Player
        if (Player != null)
        {
            playerInput = Player.GetComponent<PlayerInput>();
        }
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
        if (levelEndScreen.activeSelf) return;

        if (!pauseScreen.activeSelf)
        {
            pauseScreen.SetActive(true);
            Time.timeScale = 0f;

            if (Player != null) Player.SetActive(false); // desativa só visualmente
            if (playerInput != null) playerInput.enabled = true; // ✅ mantém controle ativo

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
                if (Player != null) Player.SetActive(true);
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
        pauseAction.Enable();
        SceneManager.LoadScene(0);
        Time.timeScale = 1f;
    }

    public void Restart()
    {
        pauseAction.Enable();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ShowLevelEndScreen(float finalTime)
    {
        levelEndScreen.SetActive(true);

        endTimeText.text = "Time: " + Mathf.FloorToInt(finalTime / 60f) + ":" +
                           Mathf.FloorToInt(finalTime % 60f).ToString("00");

        // Cursor só se mouse foi usado
        bool isMouse = Mouse.current != null && Mouse.current.wasUpdatedThisFrame;
        Cursor.visible = isMouse;
        Cursor.lockState = isMouse ? CursorLockMode.None : CursorLockMode.Locked;

        // Desativa pausa
        pauseAction.Disable();

        // 🔁 força seleção após liberar clique anterior
        StartCoroutine(ForceFocusOnLevelEndButton());
    }


    private IEnumerator ForceFocusOnLevelEndButton()
    {
        // Espera até o final do frame atual para que o clique anterior termine
        yield return new WaitForEndOfFrame();
        yield return null; // força mais um frame de espera

        if (levelEndFirstButton != null && levelEndFirstButton.activeInHierarchy)
        {
            EventSystem.current.SetSelectedGameObject(null);
            yield return null;

            Button button = levelEndFirstButton.GetComponent<Button>();
            if (button != null && button.interactable)
            {
                button.Select(); // visualmente
                EventSystem.current.SetSelectedGameObject(levelEndFirstButton); // logicamente
                Debug.Log("✅ Botão selecionado no LevelEndScreen: " + button.name);
            }
            else
            {
                Debug.LogWarning("⚠️ Botão inativo ou não interagível");
            }
        }
        else
        {
            Debug.LogWarning("❌ levelEndFirstButton está nulo ou inativo");
        }
    }

}
