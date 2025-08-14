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
        if (playerInput != null && playerInput.user.valid)
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
        if (!isInitialized || device == null || !device.added)
            return;

        if (!eventPtr.IsA<StateEvent>() && !eventPtr.IsA<DeltaStateEvent>())
            return;

        // Switch para Gamepad
        if (device is Gamepad && currentScheme != "Gamepad")
        {
            Debug.Log("🔁 Switching to Gamepad");

            if (!playerInput.user.valid)
                return;

            if (!IsDevicePaired(device))
                InputUser.PerformPairingWithDevice(device, playerInput.user);

            playerInput.SwitchCurrentControlScheme("Gamepad", device);
            currentScheme = "Gamepad";

            if (!IsAnyMenuOpen())
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
        // Switch para Teclado & Mouse
        else if ((device is Keyboard || device is Mouse) && currentScheme != "Keyboard&Mouse")
        {
            Debug.Log("🔁 Switching to Keyboard&Mouse");

            if (!playerInput.user.valid)
                return;

            if (Keyboard.current != null && !IsDevicePaired(Keyboard.current))
                InputUser.PerformPairingWithDevice(Keyboard.current, playerInput.user);

            if (Mouse.current != null && !IsDevicePaired(Mouse.current))
                InputUser.PerformPairingWithDevice(Mouse.current, playerInput.user);

            playerInput.SwitchCurrentControlScheme("Keyboard&Mouse", Keyboard.current, Mouse.current);
            currentScheme = "Keyboard&Mouse";

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private bool IsDevicePaired(InputDevice device)
    {
        foreach (var paired in playerInput.user.pairedDevices)
        {
            if (paired == device)
                return true;
        }
        return false;
    }

    private bool IsAnyMenuOpen()
    {
        return UIController.instance != null && (
            UIController.instance.pauseScreen.activeSelf ||
            UIController.instance.levelEndScreen.activeSelf ||
            UIController.instance.levelUpPanel.activeSelf
        );
    }

    private void Update()
    {
        if (IsAnyMenuOpen())
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else if (currentScheme == "Gamepad")
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
