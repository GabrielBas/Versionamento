using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;

[RequireComponent(typeof(PlayerInput))]
public class InputDeviceDetector : MonoBehaviour
{
    private PlayerInput playerInput;
    private string currentScheme = "";

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        InputSystem.onEvent += OnInputEvent;
    }

    private void OnDestroy()
    {
        InputSystem.onEvent -= OnInputEvent;
    }

    private void OnInputEvent(InputEventPtr eventPtr, InputDevice device)
    {
        if (!eventPtr.IsA<StateEvent>() && !eventPtr.IsA<DeltaStateEvent>())
            return;

        if (device is Gamepad && currentScheme != "Gamepad")
        {
            Debug.Log("🔁 Switching to Gamepad");
            playerInput.SwitchCurrentControlScheme("Gamepad", Gamepad.current);
            currentScheme = "Gamepad";

            Cursor.visible = false;              // Oculta o cursor
            Cursor.lockState = CursorLockMode.Locked;  // (opcional) bloqueia o cursor no centro
        }
        else if ((device is Keyboard || device is Mouse) && currentScheme != "Keyboard&Mouse")
        {
            Debug.Log("🔁 Switching to Keyboard&Mouse");
            playerInput.SwitchCurrentControlScheme("Keyboard&Mouse", Keyboard.current, Mouse.current);
            currentScheme = "Keyboard&Mouse";

            Cursor.visible = true;               // Mostra o cursor
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
