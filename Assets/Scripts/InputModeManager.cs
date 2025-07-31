using UnityEngine;
using UnityEngine.InputSystem;

public class InputModeManager : MonoBehaviour
{
    public static InputModeManager instance;

    public PlayerInput playerInput;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void SwitchToUI()
    {
        playerInput.SwitchCurrentActionMap("UI");
    }

    public void SwitchToGameplay()
    {
        playerInput.SwitchCurrentActionMap("Gameplay");
    }
}
