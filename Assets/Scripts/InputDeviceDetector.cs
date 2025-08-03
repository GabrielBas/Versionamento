using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;

[RequireComponent(typeof(PlayerInput))]
public class InputDeviceDetector : MonoBehaviour
{
    private PlayerInput playerInput;
    private string currentScheme = "";
    private bool isInitialized = false;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        StartCoroutine(DelayInitialize());
    }

    private System.Collections.IEnumerator DelayInitialize()
    {
        yield return new WaitForSeconds(0.1f);
        if (playerInput.user.valid)
        {
            isInitialized = true;
            InputSystem.onEvent += OnInputEvent;
        }
        else
        {
            Debug.LogWarning("⚠️ InputUser ainda não está válido.");
        }
    }

    private void OnDestroy()
    {
        if (isInitialized)
            InputSystem.onEvent -= OnInputEvent;
    }

    private void OnInputEvent(InputEventPtr eventPtr, InputDevice device)
    {
        if (!isInitialized) return;
        if (!eventPtr.IsA<StateEvent>() && !eventPtr.IsA<DeltaStateEvent>())
            return;

        // 🔒 Se uma UI estiver aberta, não mude o esquema
        bool isAnyMenuOpen = UIController.instance != null && (
            UIController.instance.pauseScreen.activeSelf ||
            UIController.instance.levelEndScreen.activeSelf ||
            UIController.instance.levelUpPanel.activeSelf
        );

        if (isAnyMenuOpen)
        {
            // Mantém o cursor visível durante menus
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            return;
        }

        // Switch to Gamepad
        if (device is Gamepad && currentScheme != "Gamepad")
        {
            Debug.Log("🔁 Switching to Gamepad");
            if (!playerInput.user.valid) return;

            InputUser.PerformPairingWithDevice(device, playerInput.user);
            playerInput.SwitchCurrentControlScheme("Gamepad", device);
            currentScheme = "Gamepad";

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        // Switch to Keyboard & Mouse
        else if ((device is Keyboard || device is Mouse) && currentScheme != "Keyboard&Mouse")
        {
            Debug.Log("🔁 Switching to Keyboard&Mouse");
            if (!playerInput.user.valid) return;

            InputUser.PerformPairingWithDevice(Keyboard.current, playerInput.user);
            InputUser.PerformPairingWithDevice(Mouse.current, playerInput.user);
            playerInput.SwitchCurrentControlScheme("Keyboard&Mouse", Keyboard.current, Mouse.current);
            currentScheme = "Keyboard&Mouse";

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
