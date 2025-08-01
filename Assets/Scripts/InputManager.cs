using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    public bool MenuOpenCloseInput { get; private set; }

    private PlayerInput playerInput;
    private Player player;

    private InputAction _menuOpenCloseAction;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        playerInput = GetComponent<PlayerInput>();
        player = GetComponent<Player>();
        _menuOpenCloseAction = playerInput.actions["MenuOpenClose"];

    }

    private void Update()
    {
        MenuOpenCloseInput = _menuOpenCloseAction.WasPressedThisFrame();
    }

    public void OnMove(CallbackContext context)
    {
        Debug.Log(context.ReadValue<Vector2>());
    }

}
